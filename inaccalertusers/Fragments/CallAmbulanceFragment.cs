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
                case "Brgy Baritan":
                    brgynumtext.Text = "2825593";
                    break;
                case "Brgy Bayan-bayanan":
                    brgynumtext.Text = "2817474";
                    break;
                case "Brgy Catmon":
                    brgynumtext.Text = "2880217";
                    break;
                case "Brgy Flores":
                    brgynumtext.Text = "2822227";
                    break;
                case "Brgy Dampalit":
                    brgynumtext.Text = "2826701";
                    break;
                case "Brgy Hulong Duhat":
                    brgynumtext.Text = "2811373";
                    break;
                case "Brgy Ibaba":
                    brgynumtext.Text = "2816597";
                    break;
                case "Brgy Muzon":
                    brgynumtext.Text = "2820255";
                    break;
                case "Brgy Panghulo":
                    brgynumtext.Text = "4466316";
                    break;
                case "Brgy Maysilo":
                    brgynumtext.Text = "2946180";
                    break;
                case "Brgy Niugan":
                    brgynumtext.Text = "4073236";
                    break;
                case "Brgy San Agustin":
                    brgynumtext.Text = "2810758";
                    break;
                case "Brgy Santulan":
                    brgynumtext.Text = "4464284";
                    break;
                case "Brgy Tanong":
                    brgynumtext.Text = "2820083";
                    break;
                case "Brgy Acacia":
                    brgynumtext.Text = "4476573";
                    break;
                case "Brgy Longos":
                    brgynumtext.Text = "4472744";
                    break;
                case "Brgy Potrero":
                    brgynumtext.Text = "3641096";
                    break;
                case "Brgy Tinajeros":
                    brgynumtext.Text = "2874162";
                    break;
                case "Brgy Tunsuya":
                    brgynumtext.Text = "3511332";
                    break;
                case "Brgy Tugatog":
                    brgynumtext.Text = "2875432";
                    break;
            }
                
        }


    }
}