using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;
using GHC.Data;

namespace GHC.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "FirebaseIIDService";
        public override async void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            SettingsHelper.SaveDeviceToken(refreshedToken, this);


            FirebaseMessaging.Instance.SubscribeToTopic("updates");

            string token = SettingsHelper.GetToken(this);
            if (token != null)
                await SendRegistrationToServer(token, refreshedToken);
        }

        async Task SendRegistrationToServer(string token, string deviceToken)
        {
            // Add custom implementation here as needed.
            string deviceModel = Build.Model;
            string deviceName = Build.Product;
            string osVersion = Build.VERSION.Release;

            bool registered = await ServiceHelper.RegisterToken(token, deviceToken, deviceName, deviceModel, osVersion);
        }

    }
}