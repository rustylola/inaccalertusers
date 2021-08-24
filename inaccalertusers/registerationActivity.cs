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

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/logintheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class registerationActivity : AppCompatActivity
    {

        //Declaring layouts
        TextInputLayout fullnametext;
        TextInputLayout useremailtext;
        TextInputLayout userphonetext;
        TextInputLayout userpasswordtext;
        TextInputLayout confirmpasstext;
        TextView backtolog;
        Button createacc;

        CoordinatorLayout rootView;

        FirebaseAuth mAuth;
        FirebaseDatabase database;

        //Adding Eventlistener hadler
        TaskCompletionListener taskCompletionListener = new TaskCompletionListener();

        //setting variables to public to access in other methods
        string fullname, useremail, userphone, userpassword, confirmpass;

        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.registerActivity);

            initializefirebase();
            mAuth = FirebaseAuth.Instance;
            connectcontrol();
        }

        private void Backtolog_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(loginuser));
            Finish();
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
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

        }

        //declaring and adding click event
        void connectcontrol()
        {
            rootView = (CoordinatorLayout) FindViewById(Resource.Id.rootView);

            fullnametext = (TextInputLayout) FindViewById(Resource.Id.registername);
            useremailtext = (TextInputLayout) FindViewById(Resource.Id.registeremail);
            userphonetext = (TextInputLayout) FindViewById(Resource.Id.registerphone);
            userpasswordtext = (TextInputLayout) FindViewById(Resource.Id.registerpassword);
            confirmpasstext = (TextInputLayout) FindViewById(Resource.Id.registerconfirmpassword);
            createacc = (Button) FindViewById(Resource.Id.registerbtn);

            createacc.Click += Createacc_Click;

            backtolog = (TextView)FindViewById(Resource.Id.backtologin);
            backtolog.Click += Backtolog_Click;
        }

        //Input Validity
        private void Createacc_Click(object sender, EventArgs e)
        {
            
            fullname = fullnametext.EditText.Text;
            useremail = useremailtext.EditText.Text;
            userphone = userphonetext.EditText.Text;
            userpassword = userpasswordtext.EditText.Text;
            confirmpass = confirmpasstext.EditText.Text;

            if(fullname.Length < 4)
            {
                Snackbar.Make(rootView, "Please enter a valid Full name", Snackbar.LengthShort).Show();
                return;
            }
            else if (fullname.Length > 35)
            {
                Snackbar.Make(rootView, "Name must be 35 Characters only", Snackbar.LengthShort).Show();
                return;
            }
            else if (!useremail.Contains("@") || useremail.Length < 11)
            {
                Snackbar.Make(rootView, "Please enter a valid Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (userphone.Length < 10)
            {
                Snackbar.Make(rootView, "Please enter a valid Phone number", Snackbar.LengthShort).Show();
                return;
            }
            else if (userpassword.Length < 8)
            {
                Snackbar.Make(rootView, "Please enter a passwrod up to 8 characters", Snackbar.LengthShort).Show();
                return;
            }
            else if (userpassword.Length > 30)
            {
                Snackbar.Make(rootView, "Password must be 30 Characters only", Snackbar.LengthShort).Show();
                return;
            }
            else if (userpassword != confirmpass)
            {
                Snackbar.Make(rootView, "Password and Confirm password not the same", Snackbar.LengthShort).Show();
                return;
            }

            // adding method to checking success and failure
            registeruser(fullname, useremail, userphone,userpassword);
        }

        void registeruser(string name, string email, string phonenum, string password)
        {
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;
            
            mAuth.CreateUserWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);
        }

        //Failed method
        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "User Registration Failed", Snackbar.LengthShort).Show();
        }

        //Sucess method : Adding UID,email,name,and phone section to the firebasedatabase
        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "User Registration Success", Snackbar.LengthShort).Show();

            HashMap userMap = new HashMap();
            userMap.Put("email", useremail);
            userMap.Put("phone", userphone);
            userMap.Put("name", fullname);

            DatabaseReference userReference = database.GetReference("users/" + mAuth.CurrentUser.Uid);
            userReference.SetValue(userMap);

            TimeSpan.FromSeconds(5);
            StartActivity(new Intent(Application.Context, typeof(loginuser)));
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
        }

        //Creating file preferences for the local storage created by the user when registering to the app
        void SaveSharedPreference()
        {
            
            editor = preferences.Edit();

            editor.PutString("email", useremail);
            editor.PutString("fullname", fullname);
            editor.PutString("phone", userphone);

            editor.Apply();

        }

        //Retrieving the data from local storage NOTE: It can use especially if the user already had account
        void RetrievedData()
        {
            string email = preferences.GetString("email","");
        }
    }
}