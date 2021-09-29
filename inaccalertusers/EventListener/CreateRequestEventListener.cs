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
        NewRequestDetails newRequest;
        FirebaseDatabase database;
        DatabaseReference newRequestRef;
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

            newRequest.UserID = newRequestRef.Key;
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
            newRequestRef.RemoveEventListener(this);
            newRequestRef.RemoveValue();
        }
    }
}