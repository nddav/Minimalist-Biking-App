using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using static Lets_go_biking.Service1;
using static System.Net.WebRequestMethods;
using System.Runtime.CompilerServices;
using System.Text;
using System.Configuration.Assemblies;
using Lets_go_biking.Proxy;

namespace Lets_go_biking
{
    public class Service1 : Interface1
    {
        string apiKey = "&apiKey=05df5479a8e55d3e63a321f750fba9e5af6d43ef"; // apiKey for JCDecaux
        public string originCity = "";
        string destinationCity = "";


        static async Task<string> APICall(string queryURL)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(queryURL);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public GeoCoordinate convertAdressToGeocoord(string adress, Boolean isOrigincity)
        {
            string APIquery = "https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf624852a2d5cc11a04e6686e587375f80cd49&text=" + adress;
            string response = APICall(APIquery).Result;

            GeoJSON geoJSON = JsonSerializer.Deserialize<GeoJSON>(response);
  
            if (isOrigincity) { originCity = geoJSON.features[0].properties.county;
            } else { destinationCity = geoJSON.features[0].properties.county; }

            return new GeoCoordinate(geoJSON.features[0].geometry.coordinates[1], geoJSON.features[0].geometry.coordinates[0]);
        }

        
        public List<JCDStation> getStationsForCity(string city)
        {
            string queryURL = "https://api.jcdecaux.com/vls/v1/stations?contract=" + city + apiKey;
            string response = APICall(queryURL).Result;
            List<JCDStation> allStations = JsonSerializer.Deserialize<List<JCDStation>>(response);
            return allStations;
        }

        public JCDStation[] getStationsForCityfromProxy(string city)
        {
            ProxyServiceClient proxy = new ProxyServiceClient();
            JCDStation[] stations = proxy.getStationsFromProxy(city);
            
            return stations;
        }

        public JCDStation findClosestStation(GeoCoordinate orgn_or_dest, List<JCDStation> allStations, Boolean droppingOffBike)
        {
            Double minDistance = -1;
            JCDStation closestStation = allStations[0];

            foreach (JCDStation item in allStations)
            {
                // Find the current station's position.
                GeoCoordinate candidatePos = new GeoCoordinate(item.position.lat, item.position.lng);

                // And compare its distance to the chosen one to see if it is closer than the current closest.
                Double distanceToCandidate = candidatePos.GetDistanceTo(orgn_or_dest);


                if (!droppingOffBike)  // If we want to drop off the bike, ie we need a bike
                {   
                    if (distanceToCandidate != 0 && (minDistance == -1 || distanceToCandidate < minDistance) && item.available_bikes > 0 && item.status.Equals("OPEN"))
                    {
                        closestStation = item;
                        minDistance = distanceToCandidate;
                    }
                }
                else // if we want to drop off a bike, ie we want a stand to drop it off on
                {
                    if (distanceToCandidate != 0 && (minDistance == -1 || distanceToCandidate < minDistance) && (item.available_bike_stands - item.available_bikes > 0) && item.status.Equals("OPEN"))
                    {
                        closestStation = item;
                        minDistance = distanceToCandidate;
                    }
                }
            }
            allStations.Remove(closestStation);
            return closestStation;
        }


        // I decided that it is worth taking a bike if the additional distance to get to stations is less than half to the distance of the original itinerary 
        public Boolean areStationsCloseEnough(GeoCoordinate origin, GeoCoordinate dest, JCDStation closestStationOrigin, JCDStation closestStationDest)
        {
            GeoCoordinate originStationCoords = new GeoCoordinate(closestStationOrigin.position.lat, closestStationOrigin.position.lng);
            GeoCoordinate destStationCoords = new GeoCoordinate(closestStationDest.position.lat, closestStationDest.position.lng);
            return origin.GetDistanceTo(originStationCoords) + dest.GetDistanceTo(destStationCoords) < 0.5 * origin.GetDistanceTo(dest) ? true : false;

        }
        

        public List<List<double>> computeIninerary(GeoCoordinate start,GeoCoordinate end)
        {
            string APIquery, response;
            APIquery = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf624852a2d5cc11a04e6686e587375f80cd49&start=" + start.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + start.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "&end=" + end.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + end.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            response = APICall(APIquery).Result;

            GeoJSONItinerary geoJSONItinerary = JsonSerializer.Deserialize<GeoJSONItinerary>(response);

            return geoJSONItinerary.features[0].geometry.coordinates;
        }


        public List<Step> computeInineraryInstructions(GeoCoordinate start, GeoCoordinate end)
        {
            string APIquery, response;
            APIquery = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf624852a2d5cc11a04e6686e587375f80cd49&start=" + start.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + start.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "&end=" + end.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + end.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            response = APICall(APIquery).Result;

            List<Step> steps = new List<Step>();

            GeoJSONItinerary geoJSONItinerary = JsonSerializer.Deserialize<GeoJSONItinerary>(response);

            
            foreach(Step step in geoJSONItinerary.features[0].properties.segments[0].steps)
            {
                steps.Add(step);
            }
            

            return steps;
        }

        public List<Step> createSteps(GeoCoordinate originGeoCoord, GeoCoordinate destinationGeoCoord, GeoCoordinate orgnStationCoords, GeoCoordinate destStationCoords)
        {
            List<Step> steps = new List<Step>();

            List<Step> stepsOrgntoStation = computeInineraryInstructions(originGeoCoord, orgnStationCoords);
            List<Step> stepsOrgnStationToDestStation = computeInineraryInstructions(orgnStationCoords, destStationCoords);
            List<Step> stepsDestStationtoDest = computeInineraryInstructions(destStationCoords, destinationGeoCoord);

            steps.AddRange(stepsOrgntoStation);
            steps.AddRange(stepsOrgnStationToDestStation);
            steps.AddRange(stepsDestStationtoDest);

            return steps;
        }


        public List<Step> getItinerary(string originAdress, string destinationAddress)
        {

            // Conversion of adresses of departure and arrival into geoCoordinates
            GeoCoordinate originGeoCoord = convertAdressToGeocoord(originAdress,true);
            GeoCoordinate destinationGeoCoord = convertAdressToGeocoord(destinationAddress,false);

            // Looking for closest stations
            JCDStation closestStationToOrgn = findClosestStation(originGeoCoord, getStationsForCity(originCity), false);
            JCDStation closestStationToDest = findClosestStation(destinationGeoCoord, getStationsForCity(destinationCity), true);

            // Checking if using bikes is worth it
            
            if (areStationsCloseEnough(originGeoCoord, destinationGeoCoord, closestStationToOrgn, closestStationToDest))
            {
                Console.WriteLine("It is worth taking bikes to get to your destination");
            }
            else
            {
                Console.WriteLine("It is not worth taking a bike you must walk you loser");
                return null;
            }

            // Conversion of stations into GeoCoordinates
            GeoCoordinate orgnStationCoords = new GeoCoordinate(closestStationToOrgn.position.lat, closestStationToOrgn.position.lng);
            GeoCoordinate destStationCoords = new GeoCoordinate(closestStationToDest.position.lat, closestStationToDest.position.lng);

            // Creation of steps
            List<Step> steps = createSteps(originGeoCoord, destinationGeoCoord, orgnStationCoords, destStationCoords);
    
            Console.WriteLine("Fin du trajet");
            return steps;
        }





        // Classes below are the ones used in the methods above

        public class JCDContract
        {
            public string name { get; set; }
        }

       
        public class Position
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class GeoJSON
        {
            public List<Feature> features { get; set; }
        }
        public class Feature
        {
            public string type { get; set; }
            public Geometry geometry { get; set; }
            public Properties properties { get; set; }
        }
        public class Geometry
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
        }
        public class Properties
        {
            public string county { get; set; }
        }

    }
 
    // Classes for itinerary

    public class GeoJSONItinerary
    {
        public List<ItineraryFeature> features { get; set; }
    }

    public class ItineraryFeature
    {
        public string type { get; set; }
        public GeometryItinerary geometry { get; set; }
        public PropertiesItinerary properties { get; set; }
    }

    public class PropertiesItinerary
    {
        public List<Segment> segments { get; set; }
        
    }

    public class GeometryItinerary
    {
        public string type { get; set; }
        public List<List<double>> coordinates { get; set; }
    }

    public class Segment
    {
        public double distance { get; set; }
        public double duration { get; set; }
        public List<Step> steps { get; set; }
    }

    public class Step
    {
        public double distance { get; set; }
        public string instruction { get; set; }
    }
}
