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


namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        FirebaseDatabase database;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

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