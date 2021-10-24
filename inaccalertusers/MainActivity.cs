using Android.App;
using Android.OS;
using Android.Widget;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Content;
using Android.Views;
using System;
using Firebase;
using Firebase.Database;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using inaccalertusers.Adapter;
using inaccalertusers.Fragments;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using inaccalertusers.EventListener;
using inaccalertusers.LocateUpdate;

namespace inaccalertusers
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = false, NoHistory = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        
        //firebase
        UserProfileEventListener userProfileEventListener = new UserProfileEventListener();

        //viewer
        ViewPager viewpager;
        
        //navigation tab
        BottomNavigationView bottomnavigationvar;
        //toolbar
        Android.Support.V7.Widget.Toolbar toolbar;

        //fragments
        historyFragment Hfragment = new historyFragment();
        notificationFragment Nfragment = new notificationFragment();
        profileFragment Pfragment = new profileFragment();

        //request permission list

        const int RequestID = 0;
        readonly string[] permissionGroup =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
        };

        //Calling methods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            CheckSpecialPermission();
            connectView();
            ShowFirstFragment();
            userProfileEventListener.Create();
            Nfragment.localnotification += Nfragment_localnotification;
        }


        void ShowFirstFragment()
        {
            viewpager.SetCurrentItem(1, true);
            SupportActionBar.Title = "Map Notification";
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbarmenu, menu);

            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.sidemenufirst)
            {
                StartActivity(new Intent(Application.Context, typeof(AboutActivity)));
            }
            else if (item.ItemId == Resource.Id.sidemenusecond)
            {
                StartActivity(new Intent(Application.Context, typeof(DevActivity)));
            }
            return base.OnOptionsItemSelected(item);
        }

        //Bottom navigation count and view pager
        void connectView()
        {
            //toolbar
            toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.onlinetoolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "";
            //initialize layouts
            bottomnavigationvar = (BottomNavigationView)FindViewById(Resource.Id.bottom_nav);
            viewpager = (ViewPager)FindViewById(Resource.Id.viewpager);
            //view pager limit note : 0 is included
            viewpager.OffscreenPageLimit = 2;
            viewpager.BeginFakeDrag();
            bottomnavigationvar.NavigationItemSelected += Bottomnavigationvar_NavigationItemSelected;
            SetupViewpager();
        }

        //Bottom navigation events per fragment
        private void Bottomnavigationvar_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.accident)
            {
                viewpager.SetCurrentItem(1, true);
                SupportActionBar.Title = "Map Notification";
            }
            else if (e.Item.ItemId == Resource.Id.profile)
            {
                viewpager.SetCurrentItem(0, true);
                SupportActionBar.Title = "My Profile";
                Pfragment.MyDetails(AppDataHelper.Getname(), AppDataHelper.Getphone(), AppDataHelper.Getemail());
            }
            else if (e.Item.ItemId == Resource.Id.history)
            {
                viewpager.SetCurrentItem(2, true);
                SupportActionBar.Title = "History Notification";
            }
        }

        //Setting ViewpagerAdapter for the declared fragments 
        private void SetupViewpager()
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(Pfragment, "Profile");
            adapter.AddFragment(Nfragment, "Notify");
            adapter.AddFragment(Hfragment, "History");
            viewpager.Adapter = adapter;
        }

        

        //Permission for GPS used for this app
        bool CheckSpecialPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(permissionGroup, RequestID);
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }

        //Show Text Permission result
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (grantResults.Length < 1)
            {
                return;
            }
            if (grantResults[0] == (int) Android.Content.PM.Permission.Granted)
            {
                Toast.MakeText(this, "Permission Success", ToastLength.Short).Show();
                Nfragment.locationUpdate();
            }
            else
            {
                Toast.MakeText(this, "Permission Failed", ToastLength.Short).Show();
            }
        }

        public override void OnBackPressed()
        {
            Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Close the Application");
            alertDialog.SetMessage("This application will close.");
            alertDialog.SetPositiveButton("Close", (close, args) =>
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());

            }).SetNegativeButton("Stay", (stay, args) => {
                return;
            });
            alertDialog.Show();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        private void Nfragment_localnotification(object sender, EventArgs e)
        {
            NotificationHelper notificationHelper = new NotificationHelper();
            if ((int)Build.VERSION.SdkInt >= 26)
            {
                notificationHelper.NotifyVersion26(this, Resources, (NotificationManager)GetSystemService(NotificationService));
            }
            else
            {
                notificationHelper.NotifyOtherVersion(this, Resources, (NotificationManager)GetSystemService(NotificationService));
            }
        }

    }
}