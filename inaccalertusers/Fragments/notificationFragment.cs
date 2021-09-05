using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Places;
using inaccalertusers.LocateUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class notificationFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback // call back for google map
    {
        //Declare variable including google maps and Layouts
        public GoogleMap mainMap;
        TextView searchtext;
        LinearLayout searchbar;

        LocationRequest mylocationRequest;
        FusedLocationProviderClient locationclient;
        Android.Locations.Location mylastlocation;
        static int Update_interval = 5; //5 second
        static int Fastest_interval = 5;
        static int Displacement = 1;

        //Declare Locationupdate
        LocationCallbackUpdate locationCallbackupdate;
        Circle circle;

        //helper
        MapFunctionUpdate mapUpdate;
        LatLng currentlocationLatlng;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        //Fragment calling ID and Map Sync
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.notification, container, false);

            //btnsample = (Button)view.FindViewById(Resource.Id.btnsample);
            searchbar = (LinearLayout)view.FindViewById(Resource.Id.mylocationsearch);
            searchtext = (TextView)view.FindViewById(Resource.Id.searchtextbox);
            SupportMapFragment mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            initializeplaces();
            mapFragment.GetMapAsync(this);
            Connectcontrol();
            createlocationrequest();
            getmyCurrentLocation();
            locationUpdate();
            return view;
        }

        void Connectcontrol()
        {
            searchbar.Click += Searchbar_Click;
        }

        //Search bar layout click event
        private void Searchbar_Click(object sender, EventArgs e)
        {
            // Intent intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeOverlay)
            //    .Build(Activity);
            // StartActivityForResult(intent, 1);

            List<Place.Field> fields = new List<Place.Field>();
            fields.Add(Place.Field.Id);
            fields.Add(Place.Field.Name);
            fields.Add(Place.Field.LatLng);
            fields.Add(Place.Field.Address);

            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("PH")
                .Build(Activity);
            StartActivityForResult(intent, 1);
        }

        void initializeplaces()
        {
            string mapkey = Resources.GetString(Resource.String.mapkey);
            if (!PlacesApi.IsInitialized)
            {
                PlacesApi.Initialize(Activity, mapkey);
            }
        }

        //Map Styling
        public void OnMapReady(GoogleMap googleMap)
        {
            bool mapstyling = googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(Activity, Resource.Raw.mymapstyle));
            mainMap = googleMap;
            // this particular event will launch if the camera move and the coordinate change the place inside the search bar will throw the
            // particular address of the specific coordinate
            mainMap.CameraIdle += MainMap_CameraIdle;
            string mapkey = Resources.GetString(Resource.String.mapkey);
            mapUpdate = new MapFunctionUpdate(mapkey);
        }

        //Moving Screen update location function
        async private void MainMap_CameraIdle(object sender, EventArgs e)
        {
            if (circle == null)
            {
                currentlocationLatlng = mainMap.CameraPosition.Target;
                searchtext.Text = await mapUpdate.FindcoordinateAddress(currentlocationLatlng);
                DrawCircle(mainMap);
            }
            else
            {
                circle.Remove();
                currentlocationLatlng = mainMap.CameraPosition.Target;
                searchtext.Text = await mapUpdate.FindcoordinateAddress(currentlocationLatlng);
                DrawCircle(mainMap);
            }
        }

        // Creating Location request and setting it in high accuracy
        void createlocationrequest()
        {
            mylocationRequest = new LocationRequest();
            mylocationRequest.SetInterval(Update_interval);
            mylocationRequest.SetFastestInterval(Fastest_interval);
            mylocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mylocationRequest.SetSmallestDisplacement(Displacement);
            locationclient = LocationServices.GetFusedLocationProviderClient(Activity);
            locationCallbackupdate = new LocationCallbackUpdate();
            locationCallbackupdate.Mylocation += locationCallbackupdate_mylocation;
        }

        void locationCallbackupdate_mylocation(object sender, LocationCallbackUpdate.OnLocationCapturedEventArgs e)
        {
            mylastlocation = e.Location;
            LatLng mycurrentposition = new LatLng(mylastlocation.Latitude, mylastlocation.Longitude);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(mycurrentposition, 18)); // set the zoom
        }


        //checking first if its the permissiong granted, then using lat long for specific location, the zoom for view
        async void getmyCurrentLocation()
        {
            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }
            else
            {
                mylastlocation = await locationclient.GetLastLocationAsync();
                if (mylastlocation != null)
                {
                    LatLng myposition = new LatLng(mylastlocation.Latitude, mylastlocation.Longitude);
                    mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 18)); //set the zoom 
                }
            }
        }

        // Location update if the user move (live location update)
        void locationUpdate()
        {
            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted)
            {
                locationclient.RequestLocationUpdates(mylocationRequest, locationCallbackupdate, null);
            }
            else
            {
                return;
            }
        }

        // Stop update if the app is in background/ (locationupdate) method is for resume app background
        void stopelocationupdate()
        {
            if (locationclient != null && locationCallbackupdate != null)
            {
                locationclient.RemoveLocationUpdates(locationCallbackupdate);
            }
        }


        // Response for selected address in autocomplete search google place api
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                if (resultCode == (int)Android.App.Result.Ok)
                {
                    var place = Autocomplete.GetPlaceFromIntent(data);
                    searchtext.Text = place.Name.ToString();
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 18));
                }
            }
        }

        void FindCordinateAddress()
        {
            //var currentCulture = CultureInfo.DefaultThreadCurrentCulture;
            //CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");
            //CultureInfo.DefaultThreadCurrentCulture = currentCulture;
        }

        public void DrawCircle(GoogleMap gMap)
        {
            circle = mainMap.AddCircle(new CircleOptions()
                .InvokeCenter(currentlocationLatlng)
                .InvokeRadius(100)
                .InvokeStrokeWidth(4)
                .InvokeStrokeColor(Android.Graphics.Color.ParseColor("#e6d9534f"))
                .InvokeFillColor(Color.Argb(034, 209, 72, 54))); //Gmap Add Circle

        }

    }
}