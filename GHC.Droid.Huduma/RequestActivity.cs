using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GHC.Data;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class RequestActivity : AppCompatActivity
    {
        const int PERMISSION_REQUEST_CODE = 200;

        readonly string[] PhonePermissions =
            {
            Manifest.Permission.CallPhone
            };

        string name, phone;

        TextView tvName, tvPhone;
        Button btnNavigate, btnCall, btnAccept;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.request_activity);

            tvName = FindViewById<TextView>(Resource.Id.tvName);
            tvPhone = FindViewById<TextView>(Resource.Id.tvPhone);

            btnAccept = FindViewById<Button>(Resource.Id.btnAccept);
            btnCall = FindViewById<Button>(Resource.Id.btnCall);
            btnNavigate = FindViewById<Button>(Resource.Id.btnNavigate);

            name = Intent.GetStringExtra("customerName");
            phone = Intent.GetStringExtra("customerPhone");

            tvName.Text = name.ToUpper();
            tvPhone.Text = phone;

            btnAccept.Click += BtnAccept_Click;
            btnCall.Click += BtnCall_Click;
            btnNavigate.Click += BtnNavigate_Click;
        }

        private void BtnNavigate_Click(object sender, EventArgs e)
        {
            double latitude = Intent.GetDoubleExtra("latitude", 0);
            double longitude =Intent.GetDoubleExtra("longitude", 0);

            // Create a Uri from an intent string. Use the result to create an Intent.
            Android.Net.Uri gmmIntentUri = Android.Net.Uri.Parse($"{latitude},{longitude}");

            // Create an Intent from gmmIntentUri. Set the action to ACTION_VIEW
            Intent mapIntent = new Intent(Intent.ActionView, gmmIntentUri);
            // Make the Intent explicit by setting the Google Maps package
            mapIntent.SetPackage("com.google.android.apps.maps");

            // Attempt to start an activity that can handle the Intent
            StartActivity(mapIntent);
        }

        private void BtnCall_Click(object sender, EventArgs e)
        {
            Call(phone);
        }

        private async void BtnAccept_Click(object sender, EventArgs e)
        {
            string token = SettingsHelper.GetToken(this);
            long requestId = Intent.GetLongExtra("requestId", 0);
            bool accepted = await ServiceHelper.AcceptRequest(token, requestId);
            if (accepted)
            {
                btnAccept.Visibility = ViewStates.Gone;
                btnCall.Visibility = ViewStates.Visible;
                btnNavigate.Visibility = ViewStates.Visible;
            }
        }

        void Call(string number)
        {
            //Check to see if any permission in our group is available, if one, then all are
            const string permission = Manifest.Permission.CallPhone;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                var uri = Android.Net.Uri.Parse($"tel:{number}");
                var intent = new Intent(Intent.ActionCall, uri);
                StartActivity(intent);
                return;
            }

            //need to request permission
            if (ShouldShowRequestPermissionRationale(permission))
            {
                //Explain to the user why we need to read the contacts
                var dialog = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                dialog.SetTitle("Ruhusa");
                dialog.SetMessage("Unahitaji kuipa ruhusa app kupiga simu");
                dialog.SetPositiveButton("OK", (sender, e) => { RequestPermissions(PhonePermissions, PERMISSION_REQUEST_CODE); });
                dialog.Create().Show();
                return;
            }

            //Finally request permissions with the list of permissions and Id
            RequestPermissions(PhonePermissions, PERMISSION_REQUEST_CODE);
        }
    }
}