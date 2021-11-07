using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using inaccalertusers.LocateUpdate;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class CreateMyProfileFragment : Android.Support.V4.App.DialogFragment
    {
        TextInputLayout fullnamehere;
        TextInputLayout emailhere;
        TextInputLayout phonehere;
        Button makeitprofile;
        string existingemail;
        public event EventHandler Createnow;
        FirebaseAuth mAuth = AppDataHelper.GetfirebaseAuth();
        public CreateMyProfileFragment(string email)
        {
            existingemail = email;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.CreateMyProfile, container, false);

            fullnamehere = (TextInputLayout)view.FindViewById(Resource.Id.createname);
            emailhere = (TextInputLayout)view.FindViewById(Resource.Id.createemail);
            phonehere = (TextInputLayout)view.FindViewById(Resource.Id.createphone);
            makeitprofile = (Button)view.FindViewById(Resource.Id.makeitprofilehere);

            emailhere.EditText.Text = existingemail;

            makeitprofile.Click += Makeitprofile_Click;

            return view;
        }

        private void Makeitprofile_Click(object sender, EventArgs e)
        {
            string fullname = fullnamehere.EditText.Text;
            string fullemail = emailhere.EditText.Text;
            string fullphone = phonehere.EditText.Text;

            if (fullname.Length < 4)
            {
                Toast.MakeText(Activity, "Name is too short. Try again", ToastLength.Short).Show();
                return;
            }
            else if (fullname.Length > 35)
            {
                Toast.MakeText(Activity, "Name must be 35 Characters only", ToastLength.Short).Show();
                return;
            }
            else if (!fullemail.Contains("@") || fullemail.Length < 8 || fullemail.Contains(" "))
            {
                Toast.MakeText(Activity, "Please enter a valid Email", ToastLength.Short).Show();
                return;
            }
            else if (fullphone.Length < 10 || fullphone.Length > 15 || fullphone.Contains(" "))
            {
                Toast.MakeText(Activity, "Please enter a valid Phone number", ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(Activity, "Do something", ToastLength.Short).Show();
            HashMap userMap = new HashMap();
            userMap.Put("email", fullemail);
            userMap.Put("phone", fullphone);
            userMap.Put("name", fullname);

            FirebaseDatabase database = AppDataHelper.Getdatabase();
            DatabaseReference userReference = database.GetReference("users/" + mAuth.CurrentUser.Uid);
            userReference.SetValue(userMap);

            Createnow.Invoke(this, new EventArgs());
        }
    }
}