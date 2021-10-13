using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using inaccalertusers.Adapter;
using inaccalertusers.Datamodels;
using inaccalertusers.EventListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Fragments
{
    public class historyFragment : Android.Support.V4.App.Fragment
    {
        RecyclerView myrecyclerView;
        List<HistoryDataModel> datamodel;
        HistoryDataListener historyDataListener;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.history, container, false);

            myrecyclerView = (RecyclerView)view.FindViewById(Resource.Id.historylistview);

            RetrieveData();

            return view;
        }

        private void SetupRecylerView()
        {
            myrecyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(myrecyclerView.Context));
            HistoryViewAdapter adapter = new HistoryViewAdapter(datamodel);
            myrecyclerView.SetAdapter(adapter);
        }

        public void RetrieveData()
        {
            historyDataListener = new HistoryDataListener();
            historyDataListener.Create();
            historyDataListener.HistoryDataRetrieve += HistoryDataListener_HistoryDataRetrieve;
        }

        private void HistoryDataListener_HistoryDataRetrieve(object sender, HistoryDataListener.HistoryDataEventArgs e)
        {
            datamodel = e.HistoryDataGet;
            SetupRecylerView();
        }
    }
}