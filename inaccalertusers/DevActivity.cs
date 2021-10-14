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
    [Activity(Label = "DevActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DevActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AboutDev);
        }
        public override void OnBackPressed()
        {
            Finish();
        }
    }
}