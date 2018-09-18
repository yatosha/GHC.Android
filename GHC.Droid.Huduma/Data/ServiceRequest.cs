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
    public class ServiceRequest
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime InitiatedTime { get; set; }
        public DateTime? AssignedTime { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime? CancelledTime { get; set; }
        public RequestStatus Status { get; set; }
        public long CustomerId { get; set; }
        public long HealthServiceId { get; set; }
        public long HealthcareProviderId { get; set; }
    }

    public enum RequestStatus
    {
        Initiated, Accepted, InProgress, Completed, Cancelled
    }
}