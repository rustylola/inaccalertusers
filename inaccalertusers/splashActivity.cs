using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using inaccalertusers.LocateUpdate;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/splashtheme", MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class splashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
        }
        protected override async void OnResume()
        {
            base.OnResume();

            if (!CheckInternet())
            {
                Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertDialog.SetTitle("Internet Connection");
                alertDialog.SetMessage("You are not connected to the internet. This application will close.");
                alertDialog.SetPositiveButton("Close", (close, args) =>
                {
                    Finish();
                });
                alertDialog.Show();
            }

            FirebaseUser currentUser = AppDataHelper.Getcurrentuser();
            if (currentUser == null)
            {
               await SimulateStartup();
            }
            else
            {
                await SimulateMainActivity();
            }
            
        }

        async Task SimulateStartup()
        {
            await Task.Delay(TimeSpan.FromSeconds(8));
            StartActivity(typeof(getstartedActivity));
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
        }
        async Task SimulateMainActivity()
        {
            await Task.Delay(TimeSpan.FromSeconds(8));
            StartActivity(typeof(MainActivity));
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
        }

        bool CheckInternet()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                //Toast.MakeText(this, "No Internet Connection", ToastLength.Long).Show();
                return false;
            }
            else
            {
                //Toast.MakeText(this, "Connected to the Internet", ToastLength.Long).Show();
                return true;
            }
        }

    }
}