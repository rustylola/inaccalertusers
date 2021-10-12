using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertusers.LocateUpdate;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.EventListener
{
    public class SendReportUserListener : Java.Lang.Object, IValueEventListener 
    {
        string myname;
        string nameofvolunteer;
        string myaddress;

        //initialize firebase
        FirebaseDatabase database;
        DatabaseReference dbReference;
        public SendReportUserListener(string username,string volunteername,string addressname)
        {
            myname = username;
            nameofvolunteer = volunteername;
            myaddress = addressname;
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            
        }

        public void SendEndReport()
        {
            database = AppDataHelper.Getdatabase();
            string uid = AppDataHelper.Getcurrentuser().Uid;
            DateTime dateTime = DateTime.Now;
            string dd = dateTime.ToString("dd");
            string mm = dateTime.ToString("MM");
            string yy = dateTime.ToString("yyyy");
            string hh = dateTime.Hour.ToString();
            string min = dateTime.Minute.ToString();

            dbReference = database.GetReference("users/" + uid + "/accident_history").Push();

            HashMap detailMap = new HashMap();
            detailMap.Put("sender", myname);
            detailMap.Put("accident_location", myaddress);
            detailMap.Put("receiver", nameofvolunteer);
            detailMap.Put("date-happen", yy + "-" + mm + "-" + dd);
            detailMap.Put("time-happen", hh + ":" + min);
            dbReference.SetValue(detailMap);

        }
    }
}