using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SQLApt
{
    internal class Program
    {
        // Start the Program
        static void Main(string[] args)
        {
            Type keysType = Type.GetType("SQLApt.Keys");
            if (keysType == null)
            {
                Console.WriteLine("Keys class not found. Please ensure you have a Keys.cs class");
                throw new InvalidOperationException("Keys class is not defined.");
            }
            Keys.GrabKey(); // Grab Key from the Key text file

            string ApiKey = Keys.ApiKey;
            string ApiHost = Keys.ApiHost;

            Console.WriteLine("Your API Key is...");
            Console.WriteLine(ApiKey);
            Console.WriteLine("Your API Host is...");
            Console.WriteLine(ApiHost);
            Console.WriteLine("Calling the API...");
            ApiCall().GetAwaiter().GetResult();
            Console.WriteLine("Api Called.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        // Call the Api To Apartment.com
        static async Task ApiCall()
        {
            // Create a New Http Client
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://apartment-list.p.rapidapi.com/properties/p153763"),
                Headers =
                {
                    { "X-RapidAPI-Key", Keys.ApiKey },
                    { "X-RapidAPI-Host", Keys.ApiHost },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
            }
        }
    }
}
