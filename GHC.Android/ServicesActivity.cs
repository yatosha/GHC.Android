using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Calligraphy;
using GHC.Adapters;
using GHC.Data;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class ServicesActivity : AppCompatActivity
    {
        RecyclerView recyclerView;
        List<HealthService> services;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            string theme = SettingsHelper.GetTheme(this);
            if (theme == "dark")
            {
                SetTheme(Resource.Style.Dark_Theme);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.services_activity);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById(Resource.Id.toolbar).JavaCast<Android.Support.V7.Widget.Toolbar>();
            toolbar.SetContentInsetsAbsolute(0, 0);
            SetSupportActionBar(toolbar);

            ImageView backButton = FindViewById<ImageView>(Resource.Id.MenuButton);
            backButton.Click += ((sender, e) =>
            {
                OnBackPressed();
            });

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            LinearLayoutManager lm = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(lm);

            ProgressDialog progress = new ProgressDialog(this, Resource.Style.MyAlertDialogStyle);
            progress.SetMessage(Resources.GetString(Resource.String.loading_services));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.Indeterminate = true;
            progress.SetCancelable(false);
            progress.Show();

            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
            Configuration config = BaseContext.Resources.Configuration;
            String lang = preferences.GetString("locale", null);

            string token = SettingsHelper.GetToken(this);
            services = await ServiceHelper.GetServices(token);
            if (services != null)
            {
                ServicesAdapter adapter = new ServicesAdapter(services, lang);
                recyclerView.SetAdapter(adapter);
                adapter.ItemClick += Adapter_ItemClick;
            }

            progress.Dismiss();
        }

        private void Adapter_ItemClick(object sender, ServicesAdapterClickEventArgs e)
        {
            HealthService service = services[e.Position];

            Intent returnIntent = new Intent();
            returnIntent.PutExtra("serviceId", service.Id);
            returnIntent.PutExtra("serviceName", service.Name);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }

        protected override void AttachBaseContext(Context newBase)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(newBase);

            String lang = preferences.GetString("locale", "en");
            newBase = LanguageContext.NewLanguageAwareContext(lang, newBase);
            Context context = new LanguageContext(newBase);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }
    }
}