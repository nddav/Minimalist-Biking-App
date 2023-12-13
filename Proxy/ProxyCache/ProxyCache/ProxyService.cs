using ProxyCache;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System;

namespace Proxy
{
    public class ProxyService : IProxyService
    {
        GenericProxyCache<JCDecauxItem> proxyCache = new GenericProxyCache<JCDecauxItem>();


        static async Task<string> APICall(string queryURL)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(queryURL);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public List<ProxyCache.JCDStation> getStationsFromProxy(string city)
        {
            JCDecauxItem JCDecauxItem = proxyCache.Get(city);
            return JCDecauxItem.stations;
        }


        // <--------------------------- CLASSES ------------------------------->


        public class JCDecauxItem
        {
            public List<ProxyCache.JCDStation> stations;

            public JCDecauxItem() { }
            public JCDecauxItem(String cityStations)
            {
                try
                {
                    this.stations = getStationsForCity(cityStations);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message + "\nVous avez entéune ville qui n'est pas prise en charge par le service de vélos JCDecaux");
                }

                List<ProxyCache.JCDStation> getStationsForCity(string city)
                {
                    string queryURL = "https://api.jcdecaux.com/vls/v1/stations?contract=" + city + "&apiKey=05df5479a8e55d3e63a321f750fba9e5af6d43ef";
                    string response = APICall(queryURL).Result;
                    Console.WriteLine(response);
                    List<ProxyCache.JCDStation> allStations = JsonSerializer.Deserialize<List<ProxyCache.JCDStation>>(response);
                    return allStations;
                }
            }
        }


        public class JCDStation
        {
            public int number { get; set; }
            public string name { get; set; }
            public Position position { get; set; }
            public int available_bikes { get; set; }
            public int available_bike_stands { get; set; }
            public string status { get; set; }
        }

        public class Position
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
    }
}
