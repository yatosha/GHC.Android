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
using SQLite;

namespace GHC.Data
{
    [Table("Requests")]
    public class RequestVM
    {
        [PrimaryKey]
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string ProviderName { get; set; }
        public string HealthServiceName { get; set; }
        public string HealthServiceSwahiliName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime InitiatedTime { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public DateTime? AssignedTime { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime? CancelledTime { get; set; }
        public double Price { get; set; }
    }
}