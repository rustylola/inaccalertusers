using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using inaccalertusers.EventListener;
using inaccalertusers.LocateUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class profileFragment : Android.Support.V4.App.Fragment
    {
        //define layouts
        TextView mynamehere;
        TextView myemailhere;
        TextView myphonehere;
        Button logoutbtnhere;

        //Alert Dialog inialize
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        //firebase auth
        FirebaseAuth auth;

        //profile eventlistener
        UserProfileEventListener profile = new UserProfileEventListener();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //initialize layouts
            View view = inflater.Inflate(Resource.Layout.profile, container, false);

            mynamehere = (TextView)view.FindViewById(Resource.Id.usernamehere);
            myemailhere = (TextView)view.FindViewById(Resource.Id.usermailhere);
            myphonehere = (TextView)view.FindViewById(Resource.Id.userphonehere);
            logoutbtnhere = (Button)view.FindViewById(Resource.Id.logoutbtn);
            //event logout
            logoutbtnhere.Click += Logoutbtnhere_Click;
            return view;
        }

        private void Logoutbtnhere_Click(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            alertDialog.SetTitle("Log out");
            alertDialog.SetMessage("Are you sure?");
            alertDialog.SetPositiveButton("Yes", (logout, args) => {
                showprogressDialog();
                auth = AppDataHelper.GetfirebaseAuth();
                profile.Logout();
                auth.SignOut();
                Intent intent = new Intent(Activity, typeof(loginuser));
                StartActivity(intent);
                closeprogressDialog();
            }).SetNegativeButton("No", (logout, args) => {
                return;
            });
            alertDialog.Show();
        }

        public void MyDetails(string name, string phone, string email)
        {
            mynamehere.Text = name;
            myemailhere.Text = email;
            myphonehere.Text = phone;
        }

        void showprogressDialog()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            alert.SetView(Resource.Layout.progressdialogue);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void closeprogressDialog()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }
    }
}