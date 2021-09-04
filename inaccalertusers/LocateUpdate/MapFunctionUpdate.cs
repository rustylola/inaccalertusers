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

        public string GetGeoCodeUrl(double lat, double lng)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&key=" + mapkey;
            return url;
        }

        public async Task<string> GetGeojasonAsync(string url)
        {
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

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
    }

}