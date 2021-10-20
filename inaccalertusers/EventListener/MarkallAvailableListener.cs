using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Maps.Android;
using Firebase.Database;
using inaccalertusers.Datamodels;
using inaccalertusers.LocateUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.EventListener
{
    public class MarkallAvailableListener : Java.Lang.Object, IValueEventListener
    {

        LatLng myCurrentlocation;
        DatabaseReference reference;
        FirebaseDatabase database;
        public event EventHandler<VolunteerOnlinenear> OnlineEventList;

        public class VolunteerOnlinenear : EventArgs
        {
            public OnlineVolunteer Online { get; set; }
        }

        public MarkallAvailableListener(LatLng Currentlocation)
        {
            myCurrentlocation = Currentlocation;
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach(DataSnapshot data in child)
                {
                    if (data.Child("accident_id").Value != null)
                    {
                        if (data.Child("accident_id").Value.ToString() == "waiting")
                        {
                            double latitude = double.Parse(data.Child("location").Child("latitude").Value.ToString());
                            double longitude = double.Parse(data.Child("location").Child("longitude").Value.ToString());
                            LatLng volunteerlocation = new LatLng(latitude, longitude);
                            OnlineVolunteer onlineVolunteer = new OnlineVolunteer();
                            onlineVolunteer.onlinevolunteerlat = latitude;
                            onlineVolunteer.onlinevolunteerlng = longitude;
                            onlineVolunteer.onlineDistanceFromUsers = SphericalUtil.ComputeDistanceBetween(myCurrentlocation,volunteerlocation);
                            double distancebetween = SphericalUtil.ComputeDistanceBetween(myCurrentlocation, volunteerlocation);
                            onlineVolunteer.onlineID = data.Key;
                            if (distancebetween >= 0 && distancebetween <= 200)
                            {
                                OnlineEventList?.Invoke(this, new VolunteerOnlinenear { Online = onlineVolunteer });
                            }
                        }
                    }
                }

            }
        }

        public void DisplayOnline()
        {
            database = AppDataHelper.Getdatabase();
            reference = database.GetReference("volunteerAvailable");
            reference.AddValueEventListener(this);
        }

        public void CloseOnline()
        {
            reference.RemoveEventListener(this);
        }
    }
}