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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Calligraphy;
using GHC.Adapters;
using GHC.Data;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class ProfileActivity : AppCompatActivity
    {
        RecyclerView recyclerview;
        HistoryAdapter adapter;

        TextView tvName, tvNumServices;

        Android.Support.V7.Widget.Toolbar toolbar;
        FrameLayout frameLayout;
        ProgressBar progressBar;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.profile_activity);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            tvName = FindViewById<TextView>(Resource.Id.tvName);
            tvNumServices = FindViewById<TextView>(Resource.Id.tvNumServices);
            recyclerview = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            toolbar.Visibility = ViewStates.Invisible;
            frameLayout.Visibility = ViewStates.Invisible;
            recyclerview.Visibility = ViewStates.Invisible;

            LinearLayoutManager lm = new LinearLayoutManager(this);
            recyclerview.SetLayoutManager(lm);

            string name = SettingsHelper.GetName(this);
            tvName.Text = name;

            string token = SettingsHelper.GetToken(this);
            List<RequestVM> requests = await ServiceHelper.GetHistory(token);
            if (requests != null)
            {
                adapter = new HistoryAdapter(requests.ToArray());
                recyclerview.SetAdapter(adapter);

                tvNumServices.Text = requests.Count.ToString("#,###");

                toolbar.Visibility = ViewStates.Visible;
                frameLayout.Visibility = ViewStates.Visible;
                recyclerview.Visibility = ViewStates.Visible;
                progressBar.Visibility = ViewStates.Gone;
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
    }
}