using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Calligraphy;
using GHC.Adapters;
using GHC.Data;
using Java.Interop;
using Newtonsoft.Json;
using PL.Bclogic.Pulsator4droid.Library;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity, ILocationListener
    {
        BroadcastReceiver receiver;

        Location _currentLocation;
        LocationManager _locationManager;

        string _locationProvider;

        readonly string[] PermissionsLocation =
        {
          Manifest.Permission.AccessCoarseLocation,
          Manifest.Permission.AccessFineLocation
        };

        const int RequestLocationId = 0;

        long requestId = 0;
        Timer timer;

        PulsatorLayout pulsator;
        ImageView btnGo;
        TextView helpView;
        ImageView imgSettings, imgProfile;

        AppState appState = AppState.Idle;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            string theme = SettingsHelper.GetTheme(this);
            if (theme == "dark")
            {
                SetTheme(Resource.Style.Dark_Theme);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            pulsator = FindViewById<PulsatorLayout>(Resource.Id.pulsator);
            btnGo = FindViewById<ImageView>(Resource.Id.btnGo);
            imgProfile = FindViewById<ImageView>(Resource.Id.imgProfile);
            imgSettings = FindViewById<ImageView>(Resource.Id.imgSettings);
            helpView = FindViewById<TextView>(Resource.Id.help);

            imgProfile.Click += ((sender, e) =>
           {
               StartActivity(typeof(ProfileActivity));
           });

            imgSettings.Click += ((sender, e) =>
            {
                Intent intent = new Intent(this, typeof(SettingsActivity));
                StartActivity(intent);
            });

            btnGo.Click += BtnGo_Click;

            if ((int)Build.VERSION.SdkInt < 23)
            {
                InitializeLocationManager();
                return;
            }
            else
            {
                GetLocationPermissionAsync();
            }

            long requestId = Intent.GetLongExtra("requestId", 0);
            if (requestId != 0)
            {
                string message = Intent.GetStringExtra("message");

                await HandleMessage(requestId, message);
            }
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            if (pulsator.IsStarted)
            {
                if (appState == AppState.Searching)
                {
                    appState = AppState.Cancelled;
                    Picasso.With(this).Load(Resource.Drawable.button_cancelled).Into(btnGo);
                    pulsator.Visibility = ViewStates.Gone;
                    pulsator.Stop();
                    pulsator.RequestLayout();

                    helpView.SetText(Resource.String.service_cancelled);
                    Animation animation = new TranslateAnimation(0, 0, 250, 0);
                    animation.Duration = 300;
                    animation.FillAfter = true;
                    helpView.StartAnimation(animation);
                }
                if (appState == AppState.Cancelled)
                {
                    appState = AppState.Idle;
                    Picasso.With(this).Load(Resource.Drawable.button).Into(btnGo);
                    pulsator.Visibility = ViewStates.Gone;
                    pulsator.Stop();
                    pulsator.RequestLayout();

                    helpView.SetText(Resource.String.tap_here);
                }
                
                //pulsator.StartAnimation(animation);
            }
            else
            {
                //YoYo.With(Techniques.SlideOutUp)
                //.Duration(400)
                //.WithRepeatListener(listener => { })
                //.PlayOn(FindViewById(Resource.Id.btnGo));

                //YoYo.With(Techniques.SlideOutUp)
                //.Duration(400)
                //.WithRepeatListener(listener => { })
                //.PlayOn(FindViewById(Resource.Id.pulsator));

                if (appState == AppState.Idle)
                {
                    Intent intent = new Intent(this, typeof(ServicesActivity));
                    StartActivityForResult(intent, 200);
                }

                //pulsator.Visibility = ViewStates.Visible;
                //helpView.Text = "SEARCHING FOR NEARBY MEDICAL PERSONNEL";

                //Animation animation = new TranslateAnimation(0, 0, 0, 250);
                //animation.Duration = 300;
                //animation.FillAfter = true;
                //helpView.StartAnimation(animation);

                //pulsator.Stop();
                //pulsator.Start();
            }
        }



        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //int id = item.ItemId;
            //if (id == Resource.Id.action_settings)
            //{
            //    return true;
            //}

            return base.OnOptionsItemSelected(item);
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == 200 && resultCode == Result.Ok)
            {
                int hour = data.GetIntExtra("hour", 0);
                int minute = data.GetIntExtra("minute", 0);
                DateTime? scheduledTime = DateTime.Today;
                if (hour == 0 && minute == 0)
                    scheduledTime = null;
                else
                    scheduledTime = scheduledTime?.AddHours(hour).AddMinutes(minute);


                long healthServiceId = data.GetLongExtra("serviceId", 0);
                string token = SettingsHelper.GetToken(this);

                appState = AppState.Searching;
                Picasso.With(this).Load(Resource.Drawable.button_searching).Into(btnGo);
                pulsator.Visibility = ViewStates.Visible;
                helpView.SetText(Resource.String.searching);

                Animation animation = new TranslateAnimation(0, 0, 0, 250);
                animation.Duration = 300;
                animation.FillAfter = true;
                helpView.StartAnimation(animation);

                pulsator.Stop();
                pulsator.Start();

                double latitude = -6.791992, longitude = 39.208219;
                if (_currentLocation != null)
                {
                    latitude = _currentLocation.Latitude;
                    longitude = _currentLocation.Longitude;
                }
                ServiceRequest request = new ServiceRequest()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Status = RequestStatus.Initiated,
                    HealthServiceId = healthServiceId,
                    ScheduledTime = scheduledTime
                };

                requestId = await ServiceHelper.LogRequest(token, request);
                if (requestId > 0)
                {
                    //timer = new Timer(2000);
                    //timer.Elapsed += Timer_Elapsed;
                    //timer.Start();
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        internal async Task HandleMessage(long requestId, string message)
        {
            if (message == "accepted")
            {
                appState = AppState.Waiting;
                Picasso.With(this).Load(Resource.Drawable.button_arriving).Into(btnGo);
                pulsator.Visibility = ViewStates.Visible;
                helpView.SetText(Resource.String.worker_arriving);

                Animation animation = new TranslateAnimation(0, 0, 0, 250);
                animation.Duration = 300;
                animation.FillAfter = true;
                helpView.StartAnimation(animation);

                pulsator.Stop();
                pulsator.Start();
            }
            else if (message == "administering")
            {
                appState = AppState.ReceivingTreatment;
                Picasso.With(this).Load(Resource.Drawable.button_receiving).Into(btnGo);
                pulsator.Visibility = ViewStates.Visible;
                helpView.SetText(Resource.String.receiving_treatment);

                pulsator.Stop();
                pulsator.Start();
            }
            else if (message == "completed")
            {
                appState = AppState.Done;
                Picasso.With(this).Load(Resource.Drawable.button_completed).Into(btnGo);
                pulsator.Visibility = ViewStates.Visible;
                helpView.SetText(Resource.String.worker_arriving);

                helpView.Text = Resources.GetString(Resource.String.treatment_complete);
                Animation animation = new TranslateAnimation(0, 0, 0, 0);
                animation.Duration = 300;
                animation.FillAfter = true;
                helpView.StartAnimation(animation);

                pulsator.Stop();

                string token = SettingsHelper.GetToken(this);
                RequestVM request = await ServiceHelper.GetRequest(token, requestId);
                if (request != null)
                {
                    Intent intent = new Intent(this, typeof(InvoiceActivity));
                    intent.AddFlags(ActivityFlags.ClearTop);
                    intent.PutExtra("request", JsonConvert.SerializeObject(request));
                    StartActivity(intent);
                }
            }

        }



        // Required by Calligraphy
        protected override void AttachBaseContext(Context newBase)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(newBase);

            String lang = preferences.GetString("locale", "en");
            newBase = LanguageContext.NewLanguageAwareContext(lang, newBase);
            Context context = new LanguageContext(newBase);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }


        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Coarse
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
                _locationManager.RequestLocationUpdates(_locationProvider, 1000, 10, this);
                //_currentLocation = _locationManager.GetLastKnownLocation(_locationProvider);
            }
            else
            {
                //Explain to the user why we need location and prompt access to location
                Snackbar.Make(FindViewById(Resource.Id.root), "Location should be enabled in settings", Snackbar.LengthIndefinite)
                        .SetAction("OK", v =>
                        {
                            Intent locationIntent = new Intent(Settings.ActionLocationSourceSettings);
                            StartActivity(locationIntent);
                        })
                        .Show();
                _locationProvider = string.Empty;
            }


            Log.Debug("Global Home Care", "Using " + _locationProvider + ".");
        }

        void GetLocationPermissionAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;
            if (ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted)
            {
                InitializeLocationManager();
                return;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
            {
                //Explain to the user why we need to read the contacts
                Snackbar.Make(this.FindViewById(Resource.Id.root), "Location access is required to register Customer Location.", Snackbar.LengthIndefinite)
                        .SetAction("OK", v => ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId))
                        .Show();

                return;
            }

            ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
        }

        protected override void OnResume()
        {
            receiver = new RequestsReceiver { Activity = this };
            IntentFilter filter = new IntentFilter();
            filter.AddAction("com.com.globalhomecare.app.REQUESTMESSAGE");
            RegisterReceiver(receiver, filter);

            if (_locationManager != null && !string.IsNullOrEmpty(_locationProvider))
                _locationManager.RequestLocationUpdates(_locationProvider, 10000, 10, this);

            base.OnResume();
        }

        protected override void OnPause()
        {
            if (receiver != null)
                UnregisterReceiver(receiver);

            if (_locationManager != null)
                _locationManager.RemoveUpdates(this);

            base.OnPause();
        }

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }

        [BroadcastReceiver(Enabled = true)]
        [IntentFilter(new[] { "com.com.globalhomecare.app.REQUESTMESSAGE" })]
        public class RequestsReceiver : BroadcastReceiver
        {
            public MainActivity Activity { get; set; }

            public override async void OnReceive(Context context, Intent intent)
            {
                long requestId = intent.GetLongExtra("requestId", 0);
                string message = intent.GetStringExtra("message");

                if (Activity != null)
                    await Activity.HandleMessage(requestId, message);
            }
        }


    }

    

    public enum AppState
    {
        Idle, Searching, Waiting, ReceivingTreatment, Cancelled, Done
    }
}

