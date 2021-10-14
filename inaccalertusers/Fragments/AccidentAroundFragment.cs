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
    public class AccidentAroundFragment : Android.Support.V4.App.DialogFragment
    {
        //initialize button
        Button yesbutton;
        Button nobutton;

        //public event
        public event EventHandler YesEvent;
        public event EventHandler NoEvent;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.AccidentAround, container, false);
            //layout initialize
            yesbutton = (Button)view.FindViewById(Resource.Id.yesbtn);
            nobutton = (Button)view.FindViewById(Resource.Id.nobtn);
            //event
            yesbutton.Click += Yesbutton_Click;
            nobutton.Click += Nobutton_Click;

            return view;
        }

        private void Nobutton_Click(object sender, EventArgs e)
        {
            NoEvent.Invoke(this, new EventArgs());
        }

        private void Yesbutton_Click(object sender, EventArgs e)
        {
            YesEvent.Invoke(this, new EventArgs());
        }
    }
}