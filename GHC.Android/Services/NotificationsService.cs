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
            if (message.Data.ContainsKey("requestId"))
            {
                long requestId = long.Parse(message.Data["requestId"]);
                string content = message.Data["message"];

                SendNotification(requestId, content);
            }
        }

        void SendNotification(long requestId, string content)
        {
            string title = "Request Accepted";
            string info = "Healthcare provider is arriving";
            if (content == "administering")
            {
                title = "Receiving Treatment";
                info = "You are now receiving treatment";
            }
            if (content == "completed")
            {
                title = "Treatment Complete";
                info = "Treatment has been completed";
            }

            Intent intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("requestId", requestId);

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Mipmap.ic_launcher_foreground)
                .SetContentTitle("Global Home Care")
                .SetContentText(title)
                .SetContentInfo(info)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetDefaults(NotificationDefaults.All);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

        //void SendNotification(string videoId, string title)
        //{
        //    var intent = new Intent(this, typeof(VideoPlayerActivity));
        //    intent.AddFlags(ActivityFlags.ClearTop);
        //    intent.PutExtra("VideoId", videoId);
        //    var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

        //    var notificationBuilder = new Notification.Builder(this)
        //        .SetSmallIcon(Resource.Drawable.Icon)
        //        .SetContentTitle("GLOBAL TV LIVE")
        //        .SetContentText(title)
        //        .SetAutoCancel(true)
        //        .SetContentIntent(pendingIntent)
        //        .SetDefaults(NotificationDefaults.All);

        //    var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
        //    notificationManager.Notify(0, notificationBuilder.Build());
        //}
    }
}