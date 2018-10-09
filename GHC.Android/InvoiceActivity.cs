using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class InvoiceActivity : AppCompatActivity
    {
        TextView tvName, tvService, tvDurationArrival, tvDurationService, tvDate, tvAmount;
        Button btnClose;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.invoice_activity);

            btnClose = FindViewById<Button>(Resource.Id.btnClose);
            tvName = FindViewById<TextView>(Resource.Id.tvName);
            tvService = FindViewById<TextView>(Resource.Id.tvService);
            tvDurationArrival = FindViewById<TextView>(Resource.Id.tvDurationArrival);
            tvDurationService = FindViewById<TextView>(Resource.Id.tvDurationService);
            tvDate = FindViewById<TextView>(Resource.Id.tvDate);
            tvAmount = FindViewById<TextView>(Resource.Id.tvAmount);

            RequestVM request = JsonConvert.DeserializeObject<RequestVM>(Intent.GetStringExtra("request"));
            tvName.Text = request.CustomerName;
            tvDate.Text = DateTime.Today.ToString("dd MMM, yyyy");
            tvService.Text = request.HealthServiceName;
            tvAmount.Text = request.Price.ToString("#,###");

            TimeSpan? arrivalDuration = request.StartedTime - request.AssignedTime;
            TimeSpan? treatmentDuration = request.CompletedTime - request.StartedTime;

            tvDurationArrival.Text = $"{arrivalDuration.Value.Hours.ToString("00")}:{arrivalDuration.Value.Minutes.ToString("00")}:{arrivalDuration.Value.Seconds.ToString("00")}";
            tvDurationService.Text = $"{treatmentDuration.Value.Hours.ToString("00")}:{treatmentDuration.Value.Minutes.ToString("00")}:{treatmentDuration.Value.Seconds.ToString("00")}";

            btnClose.Click += ((sender, e) =>
           {
               Intent intent = new Intent(this, typeof(MainActivity));
               intent.AddFlags(ActivityFlags.ClearTop);
               StartActivity(intent);
           });
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
    }
}