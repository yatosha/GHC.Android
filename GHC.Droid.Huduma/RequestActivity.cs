using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Calligraphy;
using GHC.Data;
using Newtonsoft.Json;

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

        AppState appState = AppState.Idle;

        Timer timer;
        DateTime initialTime;

        string name, phone, serviceName;

        TextView tvName, tvPhone, tvService, tvDistance, tvDuration, tvDurationTitle, tvHint;
        Button btnNavigate, btnCall, btnAccept;
        ProgressBar progressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.request_activity);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById(Resource.Id.toolbar).JavaCast<Android.Support.V7.Widget.Toolbar>();
            //toolbar.SetContentInsetsAbsolute(0, 0);
            SetSupportActionBar(toolbar);

            ImageView backButton = FindViewById<ImageView>(Resource.Id.MenuButton);
            backButton.Click += ((sender, e) =>
            {
                OnBackPressed();
            });

            tvName = FindViewById<TextView>(Resource.Id.tvName);
            tvPhone = FindViewById<TextView>(Resource.Id.tvPhone);
            tvService = FindViewById<TextView>(Resource.Id.tvService);
            tvDuration = FindViewById<TextView>(Resource.Id.tvDuration);
            tvDurationTitle = FindViewById<TextView>(Resource.Id.tvDurationTitle);
            tvHint = FindViewById<TextView>(Resource.Id.tvHint);

            btnAccept = FindViewById<Button>(Resource.Id.btnAccept);
            btnCall = FindViewById<Button>(Resource.Id.btnCall);
            btnNavigate = FindViewById<Button>(Resource.Id.btnNavigate);

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            name = Intent.GetStringExtra("customerName");
            phone = Intent.GetStringExtra("customerPhone");
            serviceName = Intent.GetStringExtra("service");

            tvName.Text = name.ToUpper();
            tvPhone.Text = phone;
            tvService.Text = serviceName;

            tvDuration.Visibility = ViewStates.Invisible;
            tvDurationTitle.Visibility = ViewStates.Invisible;

            btnAccept.Click += BtnAccept_Click;
            btnCall.Click += BtnCall_Click;
            btnNavigate.Click += BtnNavigate_Click;
        }

        // Required by Calligraphy
        protected override void AttachBaseContext(Context newBase)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(newBase);

            string lang = preferences.GetString("locale", "en");
            newBase = LanguageContext.NewLanguageAwareContext(lang, newBase);
            Context context = new LanguageContext(newBase);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }

        private void BtnNavigate_Click(object sender, EventArgs e)
        {
            double latitude = Intent.GetDoubleExtra("latitude", 0);
            double longitude =Intent.GetDoubleExtra("longitude", 0);

            // Create a Uri from an intent string. Use the result to create an Intent.
            Android.Net.Uri gmmIntentUri = Android.Net.Uri.Parse($"google.navigation:q={latitude},{longitude}");

            // Create an Intent from gmmIntentUri. Set the action to ACTION_VIEW
            Intent mapIntent = new Intent(Intent.ActionView, gmmIntentUri);
            // Make the Intent explicit by setting the Google Maps package
            mapIntent.SetPackage("com.google.android.apps.maps");

            try
            {
                // Attempt to start an activity that can handle the Intent
                StartActivity(mapIntent);
            }
            catch
            {
                Toast.MakeText(this, "Google Maps not installed!", ToastLength.Short).Show();
            }
            
        }

        private void BtnCall_Click(object sender, EventArgs e)
        {
            Call(phone);
        }

        private async void BtnAccept_Click(object sender, EventArgs e)
        {
            string token = SettingsHelper.GetToken(this);
            long requestId = Intent.GetLongExtra("requestId", 0);

            btnAccept.Visibility = ViewStates.Invisible;
            progressBar.Visibility = ViewStates.Visible;

            if (appState == AppState.Idle)
            {
                RequestVM request = await ServiceHelper.AcceptRequest(token, requestId);
                if (request != null)
                {
                    appState = AppState.Arriving;
                    btnAccept.SetText(Resource.String.start_treatment);

                    //btnAccept.Visibility = ViewStates.Gone;
                    btnCall.Visibility = ViewStates.Visible;
                    btnNavigate.Visibility = ViewStates.Visible;
                    await Repository.SaveRequest(request);

                    tvHint.Text = "YOU ARE NOW TRAVELLING TO THE PATIENT'S LOCATION";

                    tvDurationTitle.Visibility = ViewStates.Visible;
                    tvDuration.Visibility = ViewStates.Visible;

                    initialTime = DateTime.Now;
                    timer = new Timer(1000);
                    timer.Elapsed += Timer_Elapsed;
                    timer.Start();
                }
            }
            else if (appState == AppState.Arriving)
            {
                RequestVM request = await ServiceHelper.AdministerRequest(token, requestId);
                if (request != null)
                {
                    appState = AppState.ProvidingTreatment;
                    btnAccept.SetText(Resource.String.finish);

                    //btnNavigate.Visibility = ViewStates.Gone;
                    //btnCall.Visibility = ViewStates.Gone;
                    await Repository.SaveRequest(request);

                    tvHint.Text = "YOU ARE NOW ADMINISTERING TREATMENT TO THE PATIENT";

                    initialTime = DateTime.Now;
                    tvDurationTitle.Text = "TREATMENT DURATION";
                }
            }
            else if (appState == AppState.ProvidingTreatment)
            {
                RequestVM request = await ServiceHelper.CompleteRequest(token, requestId);
                if (request != null)
                {
                    appState = AppState.Completed;

                    await Repository.SaveRequest(request);
                    timer.Stop();

                    Intent intent = new Intent(this, typeof(InvoiceActivity));
                    intent.AddFlags(ActivityFlags.ClearTop);
                    intent.PutExtra("request", JsonConvert.SerializeObject(request));
                    StartActivity(intent);

                    Finish();
                }
            }
            btnAccept.Visibility = ViewStates.Visible;
            progressBar.Visibility = ViewStates.Invisible;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var elapsed = e.SignalTime - initialTime;

            double hours = elapsed.TotalHours;
            double minutes = elapsed.TotalMinutes % 60;
            double seconds = elapsed.TotalSeconds;

            RunOnUiThread( () => tvDuration.Text = $"{hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}");
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

    public enum AppState
    {
        Idle, Arriving, ProvidingTreatment, Cancelled, Completed
    }
}