using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Maps.Android;
using inaccalert.Helpers;
using Java.Util;
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
        GoogleMap map;
        //calculation
        bool isrequestDirection;
        public double distance;
        public double duration;
        public string distanceString;
        public string durationString;
        Marker VolunteerMarkers;

        public MapFunctionUpdate(string key, GoogleMap mmap)
        {
            mapkey = key;
            map = mmap;
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

        public void DrawOnMap(string json)
        {
            var directiondata = JsonConvert.DeserializeObject<DirectionParser>(json);
            //decode encoded routes
            var points = directiondata.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(points);
            ArrayList routeList = new ArrayList();
            foreach(LatLng item in line)
            {
                routeList.Add(item);
            }

            //draw polylines on map
            PolylineOptions drawpolylines = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(10)
                .InvokeColor(Color.OrangeRed)
                .InvokeStartCap(new SquareCap())
                .InvokeEndCap(new SquareCap())
                .InvokeJointType(JointType.Round)
                .Geodesic(true);

            Android.Gms.Maps.Model.Polyline myPolyline = map.AddPolyline(drawpolylines);

            //Get first point and Lastpoint
            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count-1]; // total item line minus one

            //create marker with title popup
            MarkerOptions userlocationOption = new MarkerOptions();
            userlocationOption.SetPosition(firstpoint);
            userlocationOption.SetTitle("Accident Location"); // title of the marker
            userlocationOption.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed)); // icon of the marker


            MarkerOptions volunteerlocationOption = new MarkerOptions();
            volunteerlocationOption.SetPosition(lastpoint);
            volunteerlocationOption.SetTitle("Volunteer First-aid Location");
            volunteerlocationOption.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue));

            Marker usermarker = map.AddMarker(userlocationOption);
            Marker volunteermarker = map.AddMarker(volunteerlocationOption);

            MarkerOptions VolunteerMarkerOptions = new MarkerOptions();
            VolunteerMarkerOptions.SetPosition(lastpoint);
            VolunteerMarkerOptions.SetTitle("Volunteer First-aid Location");
            VolunteerMarkerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_volunteeric));
            VolunteerMarkerOptions.Visible(false);

            VolunteerMarkers = map.AddMarker(VolunteerMarkerOptions);

            //Get Accident Bounds
            double southlng = directiondata.routes[0].bounds.southwest.lng;
            double southlat = directiondata.routes[0].bounds.southwest.lat;
            double northlng = directiondata.routes[0].bounds.northeast.lng;
            double northlat = directiondata.routes[0].bounds.northeast.lat;

            LatLng southwest = new LatLng(southlat, southlng);
            LatLng northeast = new LatLng(northlat, northlng);
            LatLngBounds bounds = new LatLngBounds(southwest, northeast);

            map.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 420));
            map.SetPadding(30,60,30,60);
            
            //show window marker
            usermarker.ShowInfoWindow();
            volunteermarker.ShowInfoWindow();

            duration = directiondata.routes[0].legs[0].duration.value; // gives seconds/miliseconds
            distance = directiondata.routes[0].legs[0].distance.value;
            durationString = directiondata.routes[0].legs[0].duration.text; // gives minutes/hour
            distanceString = directiondata.routes[0].legs[0].distance.text;
        }

        public async void UpdateVolunteerLocation(LatLng firstpoint, LatLng lastpoint)
        {
            VolunteerMarkers.Visible = true;
            VolunteerMarkers.Position = lastpoint;
            if (!isrequestDirection)
            {
                isrequestDirection = true;
                string json = await GetDirectionJsonAsync(firstpoint, lastpoint);
                var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);
                durationString = directionData.routes[0].legs[0].duration.text;
                distanceString = directionData.routes[0].legs[0].distance.text;
                VolunteerMarkers.Title = "Volunteer First-aid Location";
                VolunteerMarkers.Snippet = durationString + " / " + distanceString + " Away";
                VolunteerMarkers.ShowInfoWindow();
                isrequestDirection = false;
            }
        }
        public void UpdateArrive()
        {
            VolunteerMarkers.Title = "Volunteer First-aid Location";
            VolunteerMarkers.Snippet = "Volunteer was Arrive.";
            VolunteerMarkers.ShowInfoWindow();
        }

        
    }

}