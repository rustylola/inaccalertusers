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

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        FirebaseDatabase database;
        ViewPager viewpager; 
        BottomNavigationView bottomnavigationvar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            connectView();
            
            bottomnavigationvar.NavigationItemSelected += (s, e) => {

                switch (e.Item.ItemId)
                {
                    case Resource.Id.profile:
                        Toast.MakeText(this, "Profile", ToastLength.Long).Show();
                        break;
                    case Resource.Id.accident:
                        Toast.MakeText(this, "Accident", ToastLength.Long).Show();
                        break;
                    case Resource.Id.history:
                        Toast.MakeText(this, "History", ToastLength.Long).Show();
                        break;
                }
            };

        }

        void connectView()
        {
            bottomnavigationvar = (BottomNavigationView)FindViewById(Resource.Id.bottom_nav);
            viewpager = (ViewPager)FindViewById(Resource.Id.viewpager);
            viewpager.OffscreenPageLimit = 2;
            viewpager.BeginFakeDrag();

            SetupViewpager();
        }

        private void SetupViewpager()
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
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