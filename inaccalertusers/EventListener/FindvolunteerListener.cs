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
    public class FindvolunteerListener : Java.Lang.Object, IValueEventListener
    {
        //Custom event
        public class VolunteerFoundEventArgs : EventArgs
        {
            public List<AvailableVolunteer> Volunteers { get; set; }
        }

        public event EventHandler<VolunteerFoundEventArgs> VolunteersFound;
        public event EventHandler VolunteernotFound;

        //define variables
        LatLng myCurrentlocation;
        string myUserID;
        List<AvailableVolunteer> availableVolunteers = new List<AvailableVolunteer>();
        //set constructor current location and user id
        public FindvolunteerListener(LatLng Currentlocation, string userID)
        {
            myCurrentlocation = Currentlocation;
            myUserID = userID;
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        // retrive data from firebase and get child data from availableVolunteer
        public void OnDataChange(DataSnapshot snapshot)
        {
            //check if it is not null
            if (snapshot.Value != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                //get all children of available volunteer in firebase
                availableVolunteers.Clear();
                foreach (DataSnapshot data in child)
                {
                    if (data.Child("accident_id").Value != null)
                    {
                        if (data.Child("accident_id").Value.ToString() == "waiting") // waiting, cancelled, and timeout
                        {
                            double latitude = double.Parse(data.Child("location").Child("latitude").Value.ToString());
                            double longitude = double.Parse(data.Child("location").Child("longitude").Value.ToString());
                            LatLng volunteerlocation = new LatLng(latitude, longitude);
                            AvailableVolunteer volunteer = new AvailableVolunteer();
                            //get set volunteer lat and lng
                            volunteer.volunteerlat = latitude;
                            volunteer.volunteerlng = longitude;
                            volunteer.DistanceFromUsers = SphericalUtil.ComputeDistanceBetween(myCurrentlocation, volunteerlocation);
                            // this show the distance from user and volunteer
                            volunteer.ID = data.Key; // get parent key
                            availableVolunteers.Add(volunteer);
                        }
                    }
                }

                if (availableVolunteers.Count > 0)
                {
                    availableVolunteers = availableVolunteers.OrderBy(o => o.DistanceFromUsers).ToList();
                    VolunteersFound?.Invoke(this, new VolunteerFoundEventArgs { Volunteers = availableVolunteers });
                }
                else
                {
                    VolunteernotFound.Invoke(this, new EventArgs());
                }
            }
            else
            {
                VolunteernotFound.Invoke(this, new EventArgs());
            }
        }
        public void Create()
        {
            FirebaseDatabase database = AppDataHelper.Getdatabase();
            DatabaseReference findvolunteerRef = database.GetReference("volunteerAvailable");
            findvolunteerRef.AddListenerForSingleValueEvent(this);
        }
    }
}