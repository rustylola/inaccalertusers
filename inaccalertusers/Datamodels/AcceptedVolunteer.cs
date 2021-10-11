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
    public class AcceptedVolunteer
    {
        public string volunteerID { get; set; }
        public string volunteerName { get; set; }
        public string volunteerPhone { get; set; }
        public string volunteerLat { get; set; }
        public string volunteerLng { get; set; }
    }
}