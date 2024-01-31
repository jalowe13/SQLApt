using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Api for Apartments.com
// https://rapidapi.com/epctex-epctex-default/api/apartment-list

namespace SQLApt
{
    internal static class Keys
    {
        public static string ApiKey = "";
        public const string ApiHost = "apartment-list.p.rapidapi.com";

        public static void GrabKey()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"..\..\key.txt"))
                {
                    String line = sr.ReadToEnd();
                    Console.WriteLine(line);
                    ApiKey = line;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The key file could not be read or does not exist:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
