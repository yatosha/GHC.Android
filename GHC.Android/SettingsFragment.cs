using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using GHC.Data;

namespace GHC
{
    public class SettingsFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        const int REQUEST_CODE_PIN = 100;
        const int REQUEST_CODE_PASSWORD = 200;

        ISharedPreferences sp;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);

            Preference versionPreference = FindPreference("keyAbout");
            versionPreference.Summary = Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0).VersionName;

            PreferenceManager.GetDefaultSharedPreferences(Activity).RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnResume()
        {
            base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(Activity).RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnPause()
        {
            PreferenceManager.GetDefaultSharedPreferences(Activity).UnregisterOnSharedPreferenceChangeListener(this);
            base.OnPause();
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            Intent intent = new Intent(Activity, typeof(SettingsActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            StartActivity(intent);
            Activity.Finish();
        }


    }
}