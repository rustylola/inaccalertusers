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
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/logintheme", NoHistory = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class loginuser : AppCompatActivity, IFacebookCallback
    {

        EditText emailtext;
        EditText passwordtext;
        Button loginbtn;
        LoginButton loginfb;
        CoordinatorLayout rootview;
        FirebaseAuth mAuth;

        FirebaseAuth firebaseAuth;

        TextView createtext;
        private bool usingFirebase;

        ICallbackManager callbackManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.loginuser);

            emailtext = (EditText)FindViewById(Resource.Id.emailinput);
            passwordtext = (EditText)FindViewById(Resource.Id.passwordinput);
            loginbtn = (Button)FindViewById(Resource.Id.loginem);
            rootview = (CoordinatorLayout)FindViewById(Resource.Id.rootView);

            createtext = (TextView)FindViewById(Resource.Id.createnew);
            createtext.Click += Createtext_Click;

            loginbtn.Click += Loginbtn_Click;

            initializefirebase();

            loginfb = (LoginButton)FindViewById(Resource.Id.loginfb);
            loginfb.SetReadPermissions(new List<string> {"email", "phone", "name"});
            callbackManager = CallbackManagerFactory.Create();
            loginfb.RegisterCallback(callbackManager, this);

            firebaseAuth = GetFirebaseAuth();
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

        FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(this);

            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetProjectId("inaccalert-database")
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

            return mAuth;
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


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        public void OnCancel()
        {
        }

        public void OnError(FacebookException error)
        {
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            if (!usingFirebase)
            {
                usingFirebase = true;
                LoginResult loginResult = result as LoginResult;
                var credential = FacebookAuthProvider.GetCredential(loginResult.AccessToken.Token);

                TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
                firebaseAuth.SignInWithCredential(credential)
                    .AddOnSuccessListener(taskCompletionListener)
                    .AddOnFailureListener(taskCompletionListener);
            }
            else
            {
                Toast.MakeText(this, "Login successful", ToastLength.Short).Show();
                usingFirebase = false;

                var name = firebaseAuth.CurrentUser.DisplayName;
                var email = firebaseAuth.CurrentUser.Email;
                var phone = firebaseAuth.CurrentUser.PhoneNumber;

                Intent nextactivity = new Intent(this, typeof(MainActivity));
                nextactivity.PutExtra("fbname", name);
                nextactivity.PutExtra("fbemail", email);
                nextactivity.PutExtra("fbphne", phone);

                StartActivity(nextactivity);
            }
            
        }
    }
}