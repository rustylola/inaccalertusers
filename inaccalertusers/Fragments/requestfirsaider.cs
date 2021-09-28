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
    public class requestfirsaider : Android.Support.V4.App.DialogFragment
    {
        //define layout
        TextView nameuser;
        TextView locationuser;
        Button cancelbtn;

        //define variable
        string location;
        string username;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.request_volunteer, container, false);

            //initialize layout
            nameuser = (TextView)view.FindViewById(Resource.Id.nameofuser);
            locationuser = (TextView)view.FindViewById(Resource.Id.nameoflocation);
            cancelbtn = (Button)view.FindViewById(Resource.Id.cancelbutton);
            nameuser.Text = username;
            locationuser.Text = location;
            return view;
        }

        public requestfirsaider(string mylocation, string name)
        {
            location = mylocation;
            username = name;
        }
    }
}