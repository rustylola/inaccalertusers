using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertusers.Datamodels;
using inaccalertusers.LocateUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.EventListener
{
    public class HistoryDataListener : Java.Lang.Object, IValueEventListener
    {
        List<HistoryDataModel> historyList = new List<HistoryDataModel>();

        public event EventHandler<HistoryDataEventArgs> HistoryDataRetrieve;

        public class HistoryDataEventArgs : EventArgs
        {
            public List<HistoryDataModel> HistoryDataGet { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                historyList.Clear();
                foreach(DataSnapshot historyData in child)
                {
                    HistoryDataModel datamodel = new HistoryDataModel();
                    datamodel.uID = historyData.Key;
                    datamodel.myname = historyData.Child("sender").Value.ToString();
                    datamodel.volunteername = historyData.Child("receiver").Value.ToString();
                    datamodel.addresslocation = historyData.Child("accident_location").Value.ToString();
                    datamodel.date = historyData.Child("date-happen").Value.ToString();
                    datamodel.time = historyData.Child("time-happen").Value.ToString();
                    historyList.Add(datamodel);
                }
                HistoryDataRetrieve.Invoke(this, new HistoryDataEventArgs { HistoryDataGet = historyList });
            }
        }

        public void Create()
        {
            string userUID = AppDataHelper.Getcurrentuser().Uid;
            DatabaseReference database = AppDataHelper.Getdatabase().GetReference("users/" + userUID + "/accident_history");
            database.AddValueEventListener(this);
        }

    }
}