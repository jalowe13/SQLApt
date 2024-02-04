using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SQLApt
{
    internal class Program
    {
        // Start the Program
        static void Main(string[] args)
        {
            Type keysType = Type.GetType("SQLApt.Keys");
            Keys.GrabKey(); // Grab Key from the Key text file
            string ApiKey = Keys.ApiKey;
            string ApiHost = Keys.ApiHost;
            if (keysType == null)
            {
                Console.WriteLine("Keys class not found. Please ensure you have a key.txt file.");
                throw new InvalidOperationException("Keys class is not defined.");
            }

            if (ApiKey == "" || ApiHost == "")
            {
                Console.WriteLine(
                    "Please ensure you have a valid API Key and Host you have a key.txt file."
                );
                throw new InvalidOperationException("API Key or Host is not defined.");
            }

            Console.WriteLine("Your API Key is...");
            Console.WriteLine(ApiKey);
            Console.WriteLine("Your API Host is...");
            Console.WriteLine(ApiHost);
            Console.WriteLine("Calling the API...");

            List<string> propertyIds = grabProperties();

            foreach (string id in propertyIds)
            {
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("Calling the API for property id...");
                Console.WriteLine(id);
                ApiCall(id).GetAwaiter().GetResult();
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }

            Console.WriteLine("Api Called.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        // Call the Api To Apartment.com
        static async Task ApiCall(string propertyId)
        {
            const string url = "https://apartment-list.p.rapidapi.com/properties/";
            // Create a New Http Client
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url + propertyId),
                Headers =
                {
                    { "X-RapidAPI-Key", Keys.ApiKey },
                    { "X-RapidAPI-Host", Keys.ApiHost },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                var outputProps = new HashSet<string>
                {
                    "name","availableUnits"
                };
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(body);
                
                var dataExists = json.RootElement.TryGetProperty("data", out JsonElement dataElement);
                if (dataExists) // Attempt to get the data property
                {
                    int iterations = 0;
                    int priceUpdates = 0;
                    KeyValuePair<string, Int64> lowestUnit = new KeyValuePair<string, Int64>("", Int64.MaxValue);
                    foreach (var property in dataElement.EnumerateObject())
                    {
                        bool propertyExists = outputProps.Contains(property.Name) && property.Name == "availableUnits";
                        if (property.Name == "name")
                        {
                            Console.WriteLine("Property Name: " + property.Value.GetString());
                        }
                        if (propertyExists)
                        {
                            {
                                foreach (var unit in property.Value.EnumerateArray())
                                {

                                    if (unit.TryGetProperty("price", out JsonElement priceElement))
                                    {
                                        if (priceElement.GetInt64() != 0) // If the price is not 0
                                        {
                                            var unitName = unit.GetProperty("name").GetString();
                                            var unitPrice = priceElement.GetInt64();
                                            if (unitPrice < lowestUnit.Value)
                                            {
                                                //Console.WriteLine("New lowest unit found: " + unitName + " at " + unitPrice + "!");
                                                lowestUnit = new KeyValuePair<string, Int64>(unitName, unitPrice);
                                                priceUpdates++;
                                            }
                                        }
                                    }
                                    
                                }
                            }             
                        }
                        iterations++;
                    }
                    //Console.WriteLine("Total price updates: " + priceUpdates);
                    Console.WriteLine("Lowest Unit: " + lowestUnit.Key + " at " + lowestUnit.Value + "!");
                    Console.WriteLine("Total iterations: " + iterations);
                }
                else
                {
                    Console.WriteLine("Data not found.");
                }
            }
        }
        static List<string> grabProperties()
        {
            List<string> propertyIds = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(@"..\..\properties.txt"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        propertyIds.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The properties file could not be read or does not exist:");
                Console.WriteLine(e.Message);
            }
            return propertyIds;
        }
    }
}
