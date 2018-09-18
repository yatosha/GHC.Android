using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GHC.Data
{
    public class RequestVM
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string HealthServiceName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}