using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertusers.Datamodels;
using inaccalertusers.LocateUpdate;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.EventListener
{
    public class CreateRequestEventListener : Java.Lang.Object, IValueEventListener
    {
        //initialize firebase, Reference and class NewRequestDetails.cs
        NewRequestDetails newRequest;
        FirebaseDatabase database;
        DatabaseReference newRequestRef;
        DatabaseReference notifyvolunteerRef;
        //initalize publicly available volunteer
        List<AvailableVolunteer> mAvailableVolunteer;
        AvailableVolunteer selectedVolunteer;
        //Timer
        System.Timers.Timer Requesttimer = new System.Timers.Timer();
        int TimeCounter = 0;
        bool isVolunteerAccepted;
        //Event
        public event EventHandler NoVolunteerAcceptRequest;

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            
        }

        public CreateRequestEventListener(NewRequestDetails myNewRequestDetail)
        {
            newRequest = myNewRequestDetail;
            database = AppDataHelper.Getdatabase();
            Requesttimer.Interval = 1000;
            Requesttimer.Elapsed += Requesttimer_Elapsed;
        }

        public void CreateRequest()
        {
            newRequestRef = database.GetReference("accidentRequest").Push();

            HashMap userlocation = new HashMap();
            userlocation.Put("latitude", newRequest.userLat);
            userlocation.Put("longitude", newRequest.userLng);

            HashMap volunteerlocation = new HashMap();
            volunteerlocation.Put("latitude", "waiting");
            volunteerlocation.Put("longitude", "waiting");

            HashMap accidentDetail = new HashMap();

            newRequest.UserID = newRequestRef.Key; //Accident Request ID
            accidentDetail.Put("userlocation", userlocation);
            accidentDetail.Put("volunteerlocation", volunteerlocation);
            accidentDetail.Put("userlocation_address", newRequest.userAdrress);
            accidentDetail.Put("userID", AppDataHelper.Getcurrentuser().Uid);
            accidentDetail.Put("created_at", newRequest.Timestamp.ToString());
            accidentDetail.Put("volunteerID", "waiting");
            accidentDetail.Put("user_name", AppDataHelper.Getname());
            accidentDetail.Put("user_phone", AppDataHelper.Getphone());

            newRequestRef.AddValueEventListener(this);
            newRequestRef.SetValue(accidentDetail);
        }

        public void CancelRequestdetails()
        {
            if (selectedVolunteer != null)
            {
                DatabaseReference cancelRequestRef = database.GetReference("volunteerAvailable/" + selectedVolunteer.ID + "/accident_id");
                cancelRequestRef.SetValue("cancelled");
            }
            newRequestRef.RemoveEventListener(this);
            newRequestRef.RemoveValue();
        }

        public void CancelRequestonTimeoutdetails()
        {
            newRequestRef.RemoveEventListener(this);
            newRequestRef.RemoveValue();
        }

        // this method will notify the near volunteer
        public void NotifyVolunteer(List<AvailableVolunteer> availableVolunteers)
        {
            mAvailableVolunteer = availableVolunteers;
            if(mAvailableVolunteer.Count >= 1 && mAvailableVolunteer != null)
            {
                selectedVolunteer = mAvailableVolunteer[0];
                notifyvolunteerRef = database.GetReference("volunteerAvailable/" + selectedVolunteer.ID + "/accident_id");
                notifyvolunteerRef.SetValue(newRequest.UserID);

                if (mAvailableVolunteer.Count > 1)
                {
                    mAvailableVolunteer.RemoveAt(0);
                }
                else if (mAvailableVolunteer.Count == 1)
                {
                    mAvailableVolunteer = null;
                }
                Requesttimer.Enabled = true;
            }
            else
            {
                // no drive accepted
                Requesttimer.Enabled = false;
                NoVolunteerAcceptRequest?.Invoke(this, new EventArgs());
            }
        }

        //timeout
        private void Requesttimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeCounter++;
            if (TimeCounter == 10)
            {
                if (!isVolunteerAccepted)
                {
                    TimeCounter = 0;
                    DatabaseReference cancelRequestRef = database.GetReference("volunteerAvailable/" + selectedVolunteer.ID + "/accident_id");
                    cancelRequestRef.SetValue("timeout");
                }

                if (mAvailableVolunteer != null)
                {
                    NotifyVolunteer(mAvailableVolunteer);
                }
                else
                {
                    Requesttimer.Enabled = false;
                    //event
                    NoVolunteerAcceptRequest?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}