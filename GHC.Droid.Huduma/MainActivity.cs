using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V7.Widget;
using GHC.Adapters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using GHC.Data;
using Android.Locations;
using Android;
using System.Linq;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Util;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Provider;
using Android.Preferences;
using Calligraphy;
using System;

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

        HubConnection _hub;
        List<RequestVM> serviceRequests = new List<RequestVM>();

        RecyclerView rvRequests, rvMenu;
        RequestsAdapter adapter2;

        string[] menuItems = { "Profile", "Settings" };

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            rvMenu = FindViewById<RecyclerView>(Resource.Id.rvMenu);
            rvRequests = FindViewById<RecyclerView>(Resource.Id.rvRequests);

            GridLayoutManager lm = new GridLayoutManager(this, 2);
            rvMenu.SetLayoutManager(lm);

            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(this, DividerItemDecoration.Vertical);
            DividerItemDecoration dividerItemDecoration2 = new DividerItemDecoration(this, DividerItemDecoration.Horizontal);
            rvMenu.AddItemDecoration(dividerItemDecoration);
            rvMenu.AddItemDecoration(dividerItemDecoration2);
            

            MainMenuAdapter adapter = new MainMenuAdapter(menuItems);
            adapter.ItemClick += Adapter_ItemClick;
            rvMenu.SetAdapter(adapter);

            LinearLayoutManager manager = new LinearLayoutManager(this);
            rvRequests.SetLayoutManager(manager);
            rvRequests.AddItemDecoration(dividerItemDecoration2);

            serviceRequests = await Repository.GetPendingRequests();
            if (serviceRequests.Count < 1)
            {
                serviceRequests = await ServiceHelper.GetServiceRequests();
            }
            
            adapter2 = new RequestsAdapter(serviceRequests.ToArray(), _currentLocation);
            rvRequests.SetAdapter(adapter2);
            adapter2.ItemClick += Adapter2_ItemClick;

            if ((int)Build.VERSION.SdkInt < 23)
            {
                InitializeLocationManager();
                return;
            }
            else
            {
                GetLocationPermissionAsync();
            }

        }

        private void Adapter_ItemClick(object sender, MainMenuAdapterClickEventArgs e)
        {
            if (e.Position == 0)
            {
                StartActivity(typeof(ProfileActivity));
            }
            else
            {
                Intent intent = new Intent(this, typeof(SettingsActivity));
                StartActivity(intent);
            }
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

        private void Adapter2_ItemClick(object sender, RequestsAdapterClickEventArgs e)
        {
            RequestVM vm = serviceRequests[e.Position];
            Intent intent = new Intent(this, typeof(RequestActivity));
            intent.PutExtra("requestId", vm.Id);
            intent.PutExtra("service", vm.HealthServiceName);
            intent.PutExtra("customerName", vm.CustomerName);
            intent.PutExtra("customerPhone", vm.CustomerPhone);
            intent.PutExtra("latitude", vm.Latitude);
            intent.PutExtra("longitude", vm.Longitude);
            StartActivity(intent);
        }

        /// <summary>
        /// Initializes SignalR.
        /// </summary>
        public async Task InitializeSignalR()
        {
            _hub = new HubConnectionBuilder()
                .WithUrl("http://http://globalhomecare.azurewebsites.net/hubs/requesthub")
                .Build();

            _hub.On<string, string, string, double, double>("LogRequest",
                (customerName, customerPhone, healthServiceName, latitude, longitude) => 
                {
                    RequestVM req = new RequestVM()
                    {
                        CustomerName = customerName,
                        CustomerPhone = customerPhone,
                        HealthServiceName = healthServiceName,
                        Latitude = latitude,
                        Longitude = longitude
                    };

                    serviceRequests.Add(req);
                });

            await _hub.StartAsync();
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


            Log.Debug("Global Huduma", "Using " + _locationProvider + ".");
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
            filter.AddAction("com.com.globalhomecare.huduma.REQUESTMESSAGE");
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

        public void HandleMessage(RequestVM request)
        {
            if (serviceRequests != null)
            {
                bool inProgressPresent = serviceRequests.Any(x => x.StartedTime != null);
                if (!inProgressPresent)
                {
                    serviceRequests.Insert(0, request);

                    RunOnUiThread(() =>
                   {
                       adapter2 = new RequestsAdapter(serviceRequests.ToArray(), _currentLocation);
                       rvRequests.SetAdapter(adapter2);
                       adapter2.ItemClick += Adapter2_ItemClick;
                       adapter2.NotifyDataSetChanged();
                   });
                    
                }
            }
        }

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            adapter2.Location = _currentLocation;
            adapter2.NotifyDataSetChanged();
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
        [IntentFilter(new[] { "com.com.globalhomecare.huduma.REQUESTMESSAGE" })]
        public class RequestsReceiver : BroadcastReceiver
        {
            public MainActivity Activity { get; set; }

            public override void OnReceive(Context context, Intent intent)
            {
                long requestId = intent.GetLongExtra("requestId", 0);
                string customerName = intent.GetStringExtra("customerName");
                string customerPhone = intent.GetStringExtra("customerPhone");
                string healthServiceName = intent.GetStringExtra("healthServiceName");
                string healthServiceSwahiliName = intent.GetStringExtra("healthServiceSwahiliName");
                double latitude = intent.GetDoubleExtra("latitude", 0);
                double longitude = intent.GetDoubleExtra("longitude", 0);
                DateTime initiatedTime = DateTime.Parse(intent.GetStringExtra("initiatedTime"));

                RequestVM vm = new RequestVM()
                {
                    Id = requestId,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    HealthServiceName = healthServiceName,
                    HealthServiceSwahiliName = healthServiceSwahiliName,
                    Latitude = latitude,
                    Longitude = longitude,
                    InitiatedTime = initiatedTime
                };

                if (Activity != null)
                    Activity.HandleMessage(vm); 
            }
        }
    }
}