﻿using System;
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

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class SettingsActivity : AppCompatActivity
    {
        SettingsFragment fragment;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.settings_activity);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.SetContentInsetsAbsolute(0, 0);
            SetSupportActionBar(toolbar);

            ImageView backButton = FindViewById<ImageView>(Resource.Id.MenuButton);
            backButton.Click += ((sender, e) =>
            {
                OnBackPressed();
            });

            fragment = new SettingsFragment();
            var ft = SupportFragmentManager.BeginTransaction();
            ft.Add(Resource.Id.fragmentContainer, fragment);
            ft.Commit();
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

        public override void OnBackPressed()
        {
            SetResult(Result.Ok);
            base.OnBackPressed();
        }

        
    }
}