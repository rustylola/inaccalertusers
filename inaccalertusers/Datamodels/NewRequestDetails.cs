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
    public class NewRequestDetails
    {
        public double userLat { get; set; }
        public double userLng { get; set; }
        public double volunteerLat { get; set; }
        public double volunteerLng { get; set; }
        public string userAdrress { get; set; }
        public string distanceString { get; set; }
        public string distanceValue { get; set; }
        public string durationgString { get; set; }
        public double durationValue { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserID { get; set; }
        public string VolunteerID { get; set; }
        public string VolunteerPhone { get; set; }

    }
}