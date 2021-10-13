using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/logintheme", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class getstartedActivity : AppCompatActivity
    {
        
        Button getstartbtn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your application here
            SetContentView(Resource.Layout.getstarted);

            getstartbtn = (Button)FindViewById(Resource.Id.getstarted);
            getstartbtn.Click += Getstartbtn_Click;
        }

        private void Getstartbtn_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(loginuser)));
            OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.FadeOut);
        }
    }
}