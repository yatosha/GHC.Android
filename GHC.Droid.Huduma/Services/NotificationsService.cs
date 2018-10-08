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
using Firebase.Messaging;
using Android.Util;
using GHC.Data;
using Newtonsoft.Json;

namespace GHC.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class NotificationsService : FirebaseMessagingService
    {
        const string TAG = "NotificationsService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            if (message.Data.ContainsKey("Id"))
            {
                long requestId = long.Parse(message.Data["Id"]);
                string customerName = message.Data["CustomerName"];
                string customerPhone = message.Data["CustomerPhone"];
                string healthServiceName = message.Data["HealthServiceName"];
                string healthServiceSwahiliName = message.Data["HealthServiceSwahiliName"];
                double latitude = double.Parse(message.Data["Latitude"]);
                double longitude = double.Parse(message.Data["Longitude"]);
                string initiatedTime = message.Data["InitiatedTime"];

                var intent = new Intent("com.com.globalhomecare.huduma.REQUESTMESSAGE");
                intent.PutExtra("requestId", requestId);
                intent.PutExtra("customerName", customerName);
                intent.PutExtra("customerPhone", customerPhone);
                intent.PutExtra("healthServiceName", healthServiceName);
                intent.PutExtra("healthServiceSwahiliName", healthServiceSwahiliName);
                intent.PutExtra("latitude", latitude);
                intent.PutExtra("longitude", longitude);
                intent.PutExtra("initiatedTime", initiatedTime);
                SendBroadcast(intent);
            }
        }
    }
}