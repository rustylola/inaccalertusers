using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using yucee.Helpers;

namespace inaccalertusers.LocateUpdate
{
    public class MapFunctionUpdate
    {
        string mapkey;
        public MapFunctionUpdate(string key)
        {
            mapkey = key;
        }

        //setting url geocode
        public string GetGeoCodeUrl(double lat, double lng)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&key=" + mapkey;
            return url;
        }

        //creating task httphandler to enable return url
        public async Task<string> GetGeojasonAsync(string url)
        {
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        //creating task to enable return json
        public async Task<string> FindcoordinateAddress(LatLng position)
        {
            string url = GetGeoCodeUrl(position.Latitude, position.Longitude);
            string json = "";
            string placeAddress = "";

            json = await GetGeojasonAsync(url);

            if (!string.IsNullOrEmpty(json))
            {
                var geocodeData = JsonConvert.DeserializeObject<GeocodingParser>(json);
                if (!geocodeData.status.Contains("ZERO"))
                {
                    if (geocodeData.results[0] != null)
                    {
                        placeAddress = geocodeData.results[0].formatted_address;
                    }
                }
            }
            return placeAddress;
        }

        public async Task<string> GetDirectionJsonAsync(LatLng userlocation, LatLng volunteerlocation)
        {
            //user location as origin of route
            string str_orig = "origin=" + userlocation.Latitude + "," + userlocation.Longitude;

            //volunteer location as destination of route
            string str_destination = "destination=" + volunteerlocation.Latitude + "," + volunteerlocation.Longitude;

            //mode
            string mode = "mode=driving";

            //Building Parameters for url webservice
            string parameters = str_orig + "&" + str_destination + "&" + mode + "&";

            //output format
            string output = "json";

            //set map key
            string key = mapkey;

            //final url string
            string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" +parameters + "key=" + key;

            string json = "";
            json = await GetGeojasonAsync(url);
            return json;
        }
    }

}