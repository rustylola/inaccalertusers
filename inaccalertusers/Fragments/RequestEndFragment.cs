using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using inaccalertusers.EventListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class RequestEndFragment : Android.Support.V4.App.DialogFragment
    {
        TextView myname;
        TextView volunteername;
        TextView accidentaddress;
        Button donebtn;

        string username;
        string volunteer;
        string address;
        //event handler
        public event EventHandler AccidentCompleted;
        //listener
        SendReportUserListener sendReport;

        public RequestEndFragment(string name, string volunteername, string accidentaddress)
        {
            username = name;
            volunteer = volunteername;
            address = accidentaddress;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.RequestEnd, container, false);

            //initialize layouts
            myname = (TextView)view.FindViewById(Resource.Id.mynametext);
            volunteername = (TextView)view.FindViewById(Resource.Id.firstaidertext);
            accidentaddress = (TextView)view.FindViewById(Resource.Id.addresstext);
            donebtn = (Button)view.FindViewById(Resource.Id.donebtntext);
            Detail();
            //event
            donebtn.Click += Donebtn_Click;

            return view;
        }

        private void Donebtn_Click(object sender, EventArgs e)
        {

            sendReport = new SendReportUserListener(username, volunteer, address);
            sendReport.SendEndReport();
            AccidentCompleted?.Invoke(this, new EventArgs());
        }

        void Detail()
        {
            myname.Text = username;
            volunteername.Text = volunteer;
            accidentaddress.Text = address;
        }
    }
}