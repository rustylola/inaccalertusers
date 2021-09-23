﻿using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
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
        Button notifybtn;

        //layout
        ImageView centermarker;
        BottomSheetBehavior requestdeailbottomsheet;
        RelativeLayout locatemebtn;

        LocationRequest mylocationRequest;
        FusedLocationProviderClient locationclient;
        Android.Locations.Location mylastlocation;
        static int Update_interval = 3; //3 second replace update
        static int Fastest_interval = 3;
        static int Displacement = 1;

        //Declare Locationupdate
        LocationCallbackUpdate locationCallbackupdate;
        Circle circle;

        //helper
        MapFunctionUpdate mapUpdate;
        LatLng currentlocationLatlng;
        LatLng volunteerSampleLocation = new LatLng(14.6749, 120.9428);

        //flags
        bool circleMarkerFlags = true;
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
            //Layout Define and initalization
            centermarker = (ImageView)view.FindViewById(Resource.Id.centermarker);
            searchbar = (LinearLayout)view.FindViewById(Resource.Id.mylocationsearch);
            searchtext = (TextView)view.FindViewById(Resource.Id.searchtextbox);
            notifybtn = (Button)view.FindViewById(Resource.Id.sendnotification);
            locatemebtn = (RelativeLayout)view.FindViewById(Resource.Id.mylocationbtn);
            FrameLayout requestdetailsheets = (FrameLayout)view.FindViewById(Resource.Id.notifdetails_bottomsheets);
            //BottomSheet Initialization
            requestdeailbottomsheet = BottomSheetBehavior.From(requestdetailsheets);
            //Map Fragmet Initialization
            SupportMapFragment mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            //Method Initialization
            initializeplaces();
            mapFragment.GetMapAsync(this);
            Connectcontrol();
            createlocationrequest();
            getmyCurrentLocation();
            locationUpdate();
            return view;
        }

        //Click event method
        void Connectcontrol()
        {
            searchbar.Click += Searchbar_Click;
            locatemebtn.Click += Locatemebtn_Click;
            notifybtn.Click += Notifybtn_Click;
        }

        async void Notifybtn_Click(object sender, EventArgs e)
        {
            //notifybtn.Text = "Please Wait...";
            //notifybtn.Enabled = false;

            //Remeber : this method must call if the volunteer already accept the request
            // then run this code
            string json;
            json = await mapUpdate.GetDirectionJsonAsync(currentlocationLatlng, volunteerSampleLocation);

            if (!string.IsNullOrEmpty(json))
            {
                mapUpdate.DrawOnMap(json);
                requestdeailbottomsheet.State = BottomSheetBehavior.StateExpanded;
                //test
                RequestShow();
            }

        }

        //Trip Draw Request
        void RequestShow()
        {
            circleMarkerFlags = false; // to remove circle marker every move the camera
            centermarker.Visibility = ViewStates.Invisible;
            searchbar.Enabled = false;
        }

        private void Locatemebtn_Click(object sender, EventArgs e)
        {
            getmyCurrentLocation();
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
            mapUpdate = new MapFunctionUpdate(mapkey,mainMap);
        }

        //Moving Screen update location function and circle marker
        async private void MainMap_CameraIdle(object sender, EventArgs e)
        {
            if (circleMarkerFlags)
            {
                if (circle == null)
                {
                    currentlocationLatlng = mainMap.CameraPosition.Target; // NOTE : able to // for request
                    searchtext.Text = await mapUpdate.FindcoordinateAddress(currentlocationLatlng);
                    DrawCircle(mainMap);
                }
                else
                {
                    circle.Remove();
                    currentlocationLatlng = mainMap.CameraPosition.Target; // NOTE : able to // for request
                    searchtext.Text = await mapUpdate.FindcoordinateAddress(currentlocationLatlng);
                    DrawCircle(mainMap);
                }
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
            currentlocationLatlng = mycurrentposition; // for request
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(mycurrentposition, 18)); // set the zoom
            //notifybtn.Visibility = ViewStates.Visible;
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
                    currentlocationLatlng = myposition;// for request
                    mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 18)); //set the zoom 
                    notifybtn.Visibility = ViewStates.Visible;
                }
            }
        }

        // Location update if the user move (live location update)
        public void locationUpdate()
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
                    currentlocationLatlng = place.LatLng; // for request
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 18));
                    //notifybtn.Visibility = ViewStates.Visible;
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