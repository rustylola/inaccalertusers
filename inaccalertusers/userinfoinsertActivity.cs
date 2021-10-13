using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Android.Support.V7.App;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using Android.Support.Design.Widget;
using Android.Gms.Tasks;
using inaccalertusers.EventListener;
using Java.Util;
using inaccalertusers.LocateUpdate;

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/logintheme", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class userinfoinsertActivity : AppCompatActivity
    {

        TextInputLayout fbnametext;
        TextInputLayout fbemailtext;
        TextInputLayout fbphonetext;
        Button continuebtn;

        FirebaseDatabase database;

        TaskCompletionListener taskCompletionListener = new TaskCompletionListener();

        string name, email, uid, phone;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.userinfoinsert);
            initializefirebase();

            fbnametext = (TextInputLayout)FindViewById(Resource.Id.fbname);
            fbemailtext = (TextInputLayout)FindViewById(Resource.Id.fbemail);
            fbphonetext = (TextInputLayout)FindViewById(Resource.Id.fbphone);
            continuebtn = (Button)FindViewById(Resource.Id.continuebtnlayout);

            name = Intent.GetStringExtra("fbname" ?? "Empty");
            email = Intent.GetStringExtra("fbemail" ?? "Empty");
            uid = Intent.GetStringExtra("fbuid" ?? "Empty");

            checktext(name,email,uid);

            continuebtn.Click += Continuebtn_Click;
        }

        void initializefirebase()
        {
            database = AppDataHelper.Getdatabase();
        }

        void checktext(string takename, string takeemail, string takeuid)
        {

            fbnametext.EditText.Text = takename;
            fbemailtext.EditText.Text = takeemail;
            fbphonetext.EditText.Text = "";

            if (takeemail == "")
            {
                fbemailtext.Enabled = true;
            }
            else
            {
                fbemailtext.Enabled = false;
            }
            if (takename == "")
            {
                fbnametext.Enabled = true;
            }
            else
            {
                fbnametext.Enabled = false;
            }
        }

        private void Continuebtn_Click(object sender, EventArgs e)
        {
            name = fbnametext.EditText.Text;
            email = fbemailtext.EditText.Text;
            phone = fbphonetext.EditText.Text;
            var authuid = uid;

            if (!email.Contains("@") || email.Length < 6 || email.Contains(" "))
            {
                Toast.MakeText(this, "Please Enter a Valid Email", ToastLength.Short).Show();
                return;
            }
            else if (phone.Length < 10 || phone.Length > 15)
            {
                Toast.MakeText(this, "Please Enter a Valid Phone number", ToastLength.Short).Show();
                return;
            }


            AddingFbinfo(name, email, phone, authuid);
            
        }

        //Add OTP via twilio

        void AddingFbinfo(string getname, string getemail, string getphone, string getuid)
        {

            CreateUpdateAccListener createUpdateAccListener = new CreateUpdateAccListener(getname,getemail,getphone);
            createUpdateAccListener.CreateRef();
            

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
            Toast.MakeText(this, "All Done!", ToastLength.Short).Show();

        }
    }
}