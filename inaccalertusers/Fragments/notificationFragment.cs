using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using inaccalertusers.LocateUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class notificationFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback // call back for google map
    {
        //Declare variable including google maps
        public GoogleMap mainMap;
        LocationRequest mylocationRequest;
        FusedLocationProviderClient locationclient;
        Android.Locations.Location mylastlocation;
        static int Update_interval = 5; //5 second
        static int Fastest_interval = 5;
        static int Displacement = 1;

        //Declare Locationupdate
        LocationCallbackUpdate locationCallbackupdate;

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
            SupportMapFragment mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            createlocationrequest();
            getmyCurrentLocation();
            locationUpdate();
            return view;
        }

        //Map Styling
        public void OnMapReady(GoogleMap googleMap)
        {
            bool mapstyling = googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(Activity, Resource.Raw.mymapstyle));
            mainMap = googleMap;
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
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(mycurrentposition, 19)); // set the zoom
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
                    mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 19)); //set the zoom 
                }
            }
        }

        // Location update if the user move (live location update)
        void locationUpdate()
        {
            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted)
            {
                locationclient.RequestLocationUpdates(mylocationRequest,locationCallbackupdate, null);
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
    }
}