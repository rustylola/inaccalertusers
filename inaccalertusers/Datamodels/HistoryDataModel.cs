using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertusers.Datamodels
{
    public class HistoryDataModel
    {
        public string uID { get; set; }
        public string myname { get; set; }
        public string volunteername { get; set; }
        public string addresslocation { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}