using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class NotifyNowFragment : Android.Support.V4.App.DialogFragment
    {
        Button notifynowbtnhere;

        public event EventHandler Clicknotifynow;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.NotifyNowLayout, container, false);

            notifynowbtnhere = (Button)view.FindViewById(Resource.Id.clickok);
            notifynowbtnhere.Click += Notifynowbtnhere_Click;

            return view;
        }

        private void Notifynowbtnhere_Click(object sender, EventArgs e)
        {
            Clicknotifynow.Invoke(this, new EventArgs());
        }
    }
}