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
    public class OnlineVolunteer
    {
        public string onlineID { get; set; }
        public double onlineDistanceFromUsers { get; set; }
        public double onlinevolunteerlat { get; set; }
        public double onlinevolunteerlng { get; set; }
    }
}