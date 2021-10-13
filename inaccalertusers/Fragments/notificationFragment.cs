using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Places;
using inaccalertusers.Datamodels;
using inaccalertusers.EventListener;
using inaccalertusers.LocateUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class notificationFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback // call back for google map
    {
        //firebase
        CreateRequestEventListener requestListener;
        FindvolunteerListener findvolunteerListener;

        //Declare variable including google maps and Layouts
        public GoogleMap mainMap;
        TextView searchtext;
        LinearLayout searchbar;
        Button notifybtn;

        //layout
        ImageView centermarker;
        BottomSheetBehavior requestdeailbottomsheet;
        BottomSheetBehavior volunteerinfobehavior;
        RelativeLayout locatemebtn;

        //layout sheets
        TextView detaillocation;
        Button gonotifybtn;
        //layout sheets
        TextView volunteername;
        TextView distanceEstimate;
        RelativeLayout callintent;

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

        //Location
        LatLng currentlocationLatlng;
        string userAddressLocation;
        string volunteerNameaccept;
        string volunteerPhoneaccept;

        //flags
        bool circleMarkerFlags = true;

        //Dialog Fragments 
        requestfirsaider requestfirsaiderfragment;
        Android.Support.V4.App.FragmentTransaction manager; // Transaction

        //Datamodel
        NewRequestDetails newdataRequestmodel;

        //marker
        Marker userpin;
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
            FrameLayout volunteerinformationsheets = (FrameLayout)view.FindViewById(Resource.Id.volunteer_details);

            //Layout for sheets
            detaillocation = (TextView)view.FindViewById(Resource.Id.detaillocation);
            gonotifybtn = (Button)view.FindViewById(Resource.Id.gonotify);

            //layout for other sheets //////////////
            volunteername = (TextView)view.FindViewById(Resource.Id.volunteername);
            distanceEstimate = (TextView)view.FindViewById(Resource.Id.volunteerdistance);
            callintent = (RelativeLayout)view.FindViewById(Resource.Id.calluserbtn);

            //BottomSheet Initialization
            requestdeailbottomsheet = BottomSheetBehavior.From(requestdetailsheets);
            volunteerinfobehavior = BottomSheetBehavior.From(volunteerinformationsheets);

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

        private void RequestListener_AcceptedRequestVolunteer(object sender, CreateRequestEventListener.VolunteerAcceptEventArgs e)
        {
            if (requestfirsaiderfragment != null)
            {
                requestfirsaiderfragment.Dismiss();
                requestfirsaiderfragment = null;
            }
            volunteerPhoneaccept = e.acceptedVolunteer.volunteerPhone;
            volunteerNameaccept = e.acceptedVolunteer.volunteerName;
            volunteername.Text = e.acceptedVolunteer.volunteerName;

            requestdeailbottomsheet.State = BottomSheetBehavior.StateHidden; //requesting buttom sheet
            volunteerinfobehavior.State = BottomSheetBehavior.StateExpanded; //volunteer buttom sheet details
            notifybtn.Visibility = ViewStates.Invisible;

            // PUT LOADING BAR ---------------------------->

            jsonMapdirection(double.Parse(e.acceptedVolunteer.volunteerLat),double.Parse(e.acceptedVolunteer.volunteerLng));
        }

        //Click event method
        void Connectcontrol()
        {
            searchbar.Click += Searchbar_Click;
            locatemebtn.Click += Locatemebtn_Click;
            notifybtn.Click += Notifybtn_Click;
            gonotifybtn.Click += Gonotifybtn_Click;
            callintent.Click += Callintent_Click;
        }
        //call volunteer
        private void Callintent_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:" + volunteerPhoneaccept);
            Intent intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        //gonotify = send reqeuest
        private void Gonotifybtn_Click(object sender, EventArgs e)
        {
            // it returns text current location and name of the user
            requestfirsaiderfragment = new requestfirsaider(searchtext.Text, AppDataHelper.Getname());
            requestfirsaiderfragment.Cancelable = false;
            manager = FragmentManager.BeginTransaction();
            requestfirsaiderfragment.Show(manager, "Request");
            //cancelrequest click event
            requestfirsaiderfragment.CancelRequest += Requestfirsaiderfragment_CancelRequest;
            //Data fetch in request
            //from user
            newdataRequestmodel = new NewRequestDetails();
            newdataRequestmodel.userLat = currentlocationLatlng.Latitude;
            newdataRequestmodel.userLng = currentlocationLatlng.Longitude;
            newdataRequestmodel.userAdrress = userAddressLocation;
            newdataRequestmodel.Timestamp = DateTime.Now;
            //from volunteer
            //newdataRequestmodel.volunteerLat = 0;
            //newdataRequestmodel.volunteerLng = 0;
            //distance and duration
            //newdataRequestmodel.distanceString = "waiting";
            //newdataRequestmodel.distanceValue = 0;
            //newdataRequestmodel.durationgString = "waiting";
            requestListener = new CreateRequestEventListener(newdataRequestmodel);
            requestListener.NoVolunteerAcceptRequest += RequestListener_NoVolunteerAcceptRequest;
            requestListener.AcceptedRequestVolunteer += RequestListener_AcceptedRequestVolunteer; // event handler
            requestListener.CreateRequest();

            findvolunteerListener = new FindvolunteerListener(currentlocationLatlng, newdataRequestmodel.UserID);
            findvolunteerListener.VolunteersFound += FindvolunteerListener_VolunteersFound;
            findvolunteerListener.VolunteernotFound += FindvolunteerListener_VolunteernotFound;
            findvolunteerListener.Create();
        }

        private void RequestListener_VolunteerUpdate(object sender, CreateRequestEventListener.VolunteerLocationUpdateEventArgs e)
        {
            if (e.Status == "accept")
            {
                try
                {
                    mapUpdate.UpdateVolunteerLocation(currentlocationLatlng, e.VolunteerLocation);
                    distanceEstimate.Text = mapUpdate.distanceString + " / " + mapUpdate.durationString;
                }
                catch
                {
                    Console.WriteLine("Can't perform");
                }
                
            }
            else if(e.Status == "arrive")
            {
                mapUpdate.UpdateArrive();
                distanceEstimate.Text = "Volunteer Arriving";
                MediaPlayer player = MediaPlayer.Create(Activity, Resource.Raw.AccidentAlert);
                player.Start();
            }
            else if(e.Status == "ended")
            {
                 // hide volunteer detail bottom sheet

                string accidentlocationaddress = searchtext.Text;
                RequestEndFragment requestEnd = new RequestEndFragment(AppDataHelper.Getname(), volunteerNameaccept,accidentlocationaddress);
                requestEnd.Cancelable = false;
                manager = FragmentManager.BeginTransaction();
                requestEnd.Show(manager,"end request");
                requestEnd.AccidentCompleted += (i, o) =>
                {
                    requestEnd.Dismiss();
                    Toast.MakeText(Activity, "Report Uploaded.", ToastLength.Long).Show();
                };
                // Reset the app ------------------------------->
                
                requestListener.EndRequest();
                requestListener = null;
                ResetApp();
            }
        }

        //if no one accept request
        private void RequestListener_NoVolunteerAcceptRequest(object sender, EventArgs e)
        {
            Activity.RunOnUiThread(() => {
                if (requestfirsaiderfragment != null && requestListener != null)
                {
                    requestListener.CancelRequestonTimeoutdetails();
                    requestListener = null;
                    requestfirsaiderfragment.Dismiss();
                    requestfirsaiderfragment = null;
                    //alert dialog to say no available volunteer nearby
                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                    alert.SetTitle("Volunteer Availability Message");
                    alert.SetMessage("Available Volunteers Couldn't accept your request, Try again later");
                    alert.Show();
                }
            });
        }

        //if no found near volunteer
        private void FindvolunteerListener_VolunteernotFound(object sender, EventArgs e)
        {
            if (requestfirsaiderfragment !=null && requestListener != null)
            {
                requestListener.CancelRequestdetails();
                requestListener = null;
                requestfirsaiderfragment.Dismiss();
                requestfirsaiderfragment = null;
                //alert dialog to say no available volunteer nearby
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                alert.SetTitle("Volunteer Availability Message");
                alert.SetMessage("No Available volunteer found, Try again later");
                alert.Show();
            }
        }

        // if there is an near volunteer
        private void FindvolunteerListener_VolunteersFound(object sender, FindvolunteerListener.VolunteerFoundEventArgs e)
        {
            if (requestListener != null)
            {
                requestListener.NotifyVolunteer(e.Volunteers);
            }
        }

        //cancel event
        private void Requestfirsaiderfragment_CancelRequest(object sender, EventArgs e)
        {
            //user cancel request before the volunteer accept
            if (requestfirsaiderfragment != null && requestListener != null)
            {
                requestListener.CancelRequestdetails();
                requestListener = null;
                requestfirsaiderfragment.Dismiss();
                requestfirsaiderfragment = null;
            }
        }

        void Notifybtn_Click(object sender, EventArgs e)
        {

            //notifybtn.Text = "Please Wait...";
            //notifybtn.Enabled = false;
            notifybtn.Visibility = ViewStates.Invisible;
            requestdeailbottomsheet.State = BottomSheetBehavior.StateExpanded;
            RequestShow();
            //Remeber : this method must call if the volunteer already accept the request
            // then run this code
            //jsonMapdirection();
        }

        async void jsonMapdirection(double lat, double lng)
        {
            LatLng volunteerSampleLocation = new LatLng(lat, lng);
            string json;
            json = await mapUpdate.GetDirectionJsonAsync(currentlocationLatlng, volunteerSampleLocation);

            if (!string.IsNullOrEmpty(json))
            {
                userpin.Remove();
                mapUpdate.DrawOnMap(json);
                distanceEstimate.Text = mapUpdate.distanceString + " / " + mapUpdate.durationString;
                requestListener.VolunteerUpdate += RequestListener_VolunteerUpdate;
                //test // Display views
            }
        }

        //Trip Draw Request
        void RequestShow()
        {
            circleMarkerFlags = false; // to remove circle marker every move the camera
            DrawMark(mainMap); // inputting marker
            centermarker.Visibility = ViewStates.Invisible;
            searchbar.Enabled = false;
            //show current location
            detaillocation.Text = searchtext.Text;
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
                    userAddressLocation = searchtext.Text;
                    DrawCircle(mainMap);
                }
                else
                {
                    circle.Remove();
                    currentlocationLatlng = mainMap.CameraPosition.Target; // NOTE : able to // for request
                    searchtext.Text = await mapUpdate.FindcoordinateAddress(currentlocationLatlng);
                    userAddressLocation = searchtext.Text;
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
                    currentlocationLatlng = myposition; // for request
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
                    userAddressLocation = place.Name.ToString();
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
                .InvokeFillColor(Color.Argb(020, 209, 72, 54))); //Gmap Add Circle
        }

        public void DrawMark(GoogleMap gMap)
        {
            MarkerOptions mymarker = new MarkerOptions();
            mymarker.SetPosition(currentlocationLatlng);
            mymarker.SetTitle("Accident Location"); // title of the marker
            mymarker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            userpin = gMap.AddMarker(mymarker);
            userpin.ShowInfoWindow();
            //userpin.Remove();
        }

        public void ResetApp()
        {
            mainMap.Clear();
            circleMarkerFlags = true; // to remove circle marker every move the camera
            DrawCircle(mainMap);
            centermarker.Visibility = ViewStates.Visible;
            searchbar.Enabled = true;
            //show current location
            getmyCurrentLocation();
            volunteerinfobehavior.State = BottomSheetBehavior.StateHidden;
        }


    }
}