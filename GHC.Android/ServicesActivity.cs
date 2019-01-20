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
        const int SELECT_TIME_REQUEST_CODE = 200;

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
            

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle(Resource.String.when_do_you_need_service);

            builder.SetNegativeButton(Resource.String.cancel, (sender2, e2) => { });

            ArrayAdapter<string> dialogAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            dialogAdapter.Add(Resources.GetString(Resource.String.now));
            dialogAdapter.Add(Resources.GetString(Resource.String.later));
            builder.SetAdapter(dialogAdapter, async (sender2, e2) =>
            {
                string option = dialogAdapter.GetItem(e2.Which);
                if (option == Resources.GetString(Resource.String.now))
                {
                    Intent returnIntent = new Intent();
                    returnIntent.PutExtra("serviceId", service.Id);
                    returnIntent.PutExtra("serviceName", service.Name);
                    SetResult(Result.Ok, returnIntent);
                    Finish();
                }
                else
                {
                    Intent intent = new Intent(this, typeof(SelectTimeActivity));
                    intent.PutExtra("serviceId", service.Id);
                    intent.PutExtra("serviceName", service.Name);
                    StartActivityForResult(intent, SELECT_TIME_REQUEST_CODE);
                }
            });

            builder.Show();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SELECT_TIME_REQUEST_CODE && resultCode == Result.Ok)
            {
                int hour = data.GetIntExtra("hour", 0);
                int minute = data.GetIntExtra("minute", 0);
                long serviceId = data.GetLongExtra("serviceId", 0);
                string serviceName = data.GetStringExtra("serviceName");

                Intent returnIntent = new Intent();
                returnIntent.PutExtra("serviceId", serviceId);
                returnIntent.PutExtra("serviceName", serviceName);
                returnIntent.PutExtra("hour", hour);
                returnIntent.PutExtra("minute", minute);
                SetResult(Result.Ok, returnIntent);
                Finish();
            }
            base.OnActivityResult(requestCode, resultCode, data);
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