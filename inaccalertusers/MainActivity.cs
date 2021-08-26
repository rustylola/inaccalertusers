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

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        FirebaseDatabase database;
        //viewer
        ViewPager viewpager;
        TextView title;
        //navigation tab
        BottomNavigationView bottomnavigationvar;

        //fragments
        historyFragment Hfragment = new historyFragment();
        notificationFragment Nfragment = new notificationFragment();
        profileFragment Pfragment = new profileFragment();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            connectView();
            viewpager.SetCurrentItem(1, true);
            title.Text = "Notify Volunteer";

        }

        

        void connectView()
        {
            title = (TextView)FindViewById(Resource.Id.textTitle);
            bottomnavigationvar = (BottomNavigationView)FindViewById(Resource.Id.bottom_nav);
            viewpager = (ViewPager)FindViewById(Resource.Id.viewpager);
            viewpager.OffscreenPageLimit = 2;
            viewpager.BeginFakeDrag();
            bottomnavigationvar.NavigationItemSelected += Bottomnavigationvar_NavigationItemSelected;
            SetupViewpager();
        }

        private void Bottomnavigationvar_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.accident)
            {
                viewpager.SetCurrentItem(1, true);
                title.Text = "Notify Volunteer";
            }
            else if (e.Item.ItemId == Resource.Id.profile)
            {
                viewpager.SetCurrentItem(0, true);
                title.Text = "My Profile";
            }
            else if (e.Item.ItemId == Resource.Id.history)
            {
                viewpager.SetCurrentItem(2, true);
                title.Text = "Notification History";
            }
        }

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
    }
}