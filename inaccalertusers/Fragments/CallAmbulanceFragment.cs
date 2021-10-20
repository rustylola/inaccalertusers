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
    public class CallAmbulanceFragment : Android.Support.V4.App.DialogFragment
    {
        Spinner brgycategory;
        TextView brgynumtext;
        Button btnwait;
        Button btncall;

        public string pickcateg { get; set; }
        public string number { get; set; }

        public event EventHandler Waiting;
        public event EventHandler Callambu;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.SelectCallNum, container, false);

            brgynumtext = (TextView)view.FindViewById(Resource.Id.emergencynumbertext);
            btnwait = (Button)view.FindViewById(Resource.Id.waitfirstaidbtn);
            btncall = (Button)view.FindViewById(Resource.Id.callambubtn);

            //spinner
            brgycategory = (Spinner)view.FindViewById(Resource.Id.btgycateg);
            brgycategory.ItemSelected += Brgycategory_ItemSelected;
            var adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.ambulancelist, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            brgycategory.Adapter = adapter;

            //event
            btnwait.Click += Btnwait_Click;
            btncall.Click += Btncall_Click;

            return view;
        }

        private void Btncall_Click(object sender, EventArgs e)
        {
            number = brgynumtext.Text;
            if (pickcateg == "Pick Barangay")
            {
                Toast.MakeText(Activity, "Pick Barangay Category.", ToastLength.Long).Show();
                return;
            }
            if (number == "")
            {
                Toast.MakeText(Activity, "Pick Barangay Category.", ToastLength.Long).Show();
                return;
            }

            Callambu.Invoke(this, new EventArgs());
        }

        private void Btnwait_Click(object sender, EventArgs e)
        {
            Waiting.Invoke(this, new EventArgs());
        }

        private void Brgycategory_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner selectedCateg = (Spinner) sender;
            string toast = string.Format("Selected Ambulance Category is {0}", selectedCateg.GetItemAtPosition(e.Position));
            //Toast.MakeText(Activity, toast, ToastLength.Long).Show();
            pickcateg = selectedCateg.GetItemAtPosition(e.Position).ToString();

            switch (pickcateg)
            {
                case "Pick Barangay":
                    brgynumtext.Text = "";
                    break;
                case "BARANGAY BAGUMBAYAN NORTH":
                    brgynumtext.Text = "+632 351-5482";
                    break;
                case "BARANGAY BAGUMBAYAN SOUTH":
                    brgynumtext.Text = "+632 351-5739";
                    break;
                case "BARANGAY BANGCULASI":
                    brgynumtext.Text = "+632 281-8732";
                    break;
                case "BARANGAY DAANGHARI":
                    brgynumtext.Text = "+632 351-1071";
                    break;
                case "BARANGAY NAVOTAS EAST":
                    brgynumtext.Text = "+632 283-3750";
                    break;
                case "BARANGAY NAVOTAS WEST":
                    brgynumtext.Text = "+632 281-7857";
                    break;
                case "BARANGAY NORTH BOULEVARD NORTH":
                    brgynumtext.Text = "+632 282-2869";
                    break;
                case "BARANGAY NORTH BOULEVARD SOUTH":
                    brgynumtext.Text = "+632 283-7495";
                    break;
                case "BARANGAY SAN JOSE":
                    brgynumtext.Text = "+632 282-3051";
                    break;
                case "BARANGAY SAN RAFAEL VILLAGE":
                    brgynumtext.Text = "+632 251-8350";
                    break;
                case "BARANGAY SAN ROQUE":
                    brgynumtext.Text = "+632 351-6063";
                    break;
                case "BARANGAY SIPAC-ALMACEN":
                    brgynumtext.Text = "+632 283-8800";
                    break;
                case "BARANGAY TANGOS":
                    brgynumtext.Text = "8429-51-59";
                    break;
                case "BARANGAY TANZA":
                    brgynumtext.Text = "(02) 8351 4089";
                    break;
            }
                
        }


    }
}