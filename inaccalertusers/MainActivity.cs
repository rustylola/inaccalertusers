using Android.App;
using Android.OS;
using Android.Widget;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Content;
using Android.Views;
using System;
using Firebase;
using Firebase.Database;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using inaccalertusers.Adapter;
using inaccalertusers.Fragments;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        FirebaseDatabase database;
        //viewer
        ViewPager viewpager;
        
        //navigation tab
        BottomNavigationView bottomnavigationvar;

        //fragments
        historyFragment Hfragment = new historyFragment();
        notificationFragment Nfragment = new notificationFragment();
        profileFragment Pfragment = new profileFragment();

        //request permission list

        const int RequestID = 0;
        readonly string[] permissionGroup =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
        };

        //Calling methods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            CheckSpecialPermission();
            connectView();
            viewpager.SetCurrentItem(1, true);
            
        }

        //Bottom navigation count and view pager
        void connectView()
        {
            bottomnavigationvar = (BottomNavigationView)FindViewById(Resource.Id.bottom_nav);
            viewpager = (ViewPager)FindViewById(Resource.Id.viewpager);
            //view pager limit note : 0 is included
            viewpager.OffscreenPageLimit = 2;
            viewpager.BeginFakeDrag();
            bottomnavigationvar.NavigationItemSelected += Bottomnavigationvar_NavigationItemSelected;
            SetupViewpager();
        }

        //Bottom navigation events per fragment
        private void Bottomnavigationvar_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.accident)
            {
                viewpager.SetCurrentItem(1, true);
            }
            else if (e.Item.ItemId == Resource.Id.profile)
            {
                viewpager.SetCurrentItem(0, true);
            }
            else if (e.Item.ItemId == Resource.Id.history)
            {
                viewpager.SetCurrentItem(2, true);
            }
        }

        //Setting ViewpagerAdapter for the declared fragments 
        private void SetupViewpager()
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(Pfragment, "Profile");
            adapter.AddFragment(Nfragment, "Notify");
            adapter.AddFragment(Hfragment, "History");
            viewpager.Adapter = adapter;
        }

        void initializedatabase()
        {
            var app = FirebaseApp.InitializeApp(this);

            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetApplicationId("inaccalert-database")
                    .SetApiKey("AIzaSyCDcTY55MlwDzx2r_zAij1uGu0QOMzdzVQ")
                    .SetDatabaseUrl("https://inaccalert-database-default-rtdb.firebaseio.com")
                    .SetStorageBucket("inaccalert-database.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(this, option);
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

        }

        //Permission for GPS used for this app
        bool CheckSpecialPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(permissionGroup, RequestID);
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }

        //Show Text Permission result
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (grantResults[0] == (int) Android.Content.PM.Permission.Granted)
            {
                Toast.MakeText(this, "Permission Success", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Permission Failed", ToastLength.Short).Show();
            }
        }

    }
}