using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using inaccalertusers.EventListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/logintheme", NoHistory = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class loginuser : AppCompatActivity
    {

        EditText emailtext;
        EditText passwordtext;
        Button loginbtn;
        Button loginfb;
        CoordinatorLayout rootview;
        FirebaseAuth mAuth;

        TextView createtext;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.loginuser);

            emailtext = (EditText)FindViewById(Resource.Id.emailinput);
            passwordtext = (EditText)FindViewById(Resource.Id.passwordinput);
            loginbtn = (Button)FindViewById(Resource.Id.loginem);
            rootview = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            loginfb = (Button)FindViewById(Resource.Id.loginfb);

            loginbtn.Click += Loginbtn_Click;

            createtext = (TextView)FindViewById(Resource.Id.createnew);
            createtext.Click += Createtext_Click;

            loginfb.Click += Loginfb_Click;

            initializefirebase();
        }

        private void Loginfb_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //initializing firebase connection
        void initializefirebase()
        {
            var app = FirebaseApp.InitializeApp(this);

            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetApplicationId("inaccalert-database")
                    .SetApiKey("AIzaSyCDcTY55MlwDzx2r_zAij1uGu0QOMzdzVQ")
                    .SetDatabaseUrl("https://inaccalert-database-default-rtdb.firebaseio.com")
                    .SetStorageBucket("inaccalert-database.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(this, option);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }

        }

        private void Loginbtn_Click(object sender, EventArgs e)
        {
            string email = emailtext.Text;
            string password = passwordtext.Text;

            if (!email.Contains("@"))
            {
                Snackbar.Make(rootview, "Please enter a valid Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootview, "Please enter a valid Password", Snackbar.LengthShort).Show();
                return;
            }

            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();

            mAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(taskCompletionListener)
                .AddOnFailureListener(taskCompletionListener);

            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootview, "Login Failed", Snackbar.LengthShort).Show();
            return;
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
        }

        private void Createtext_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(registerationActivity)));
            OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
        }
    }
}