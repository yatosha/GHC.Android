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
using Android.Views;
using Android.Widget;
using GHC.Data;
using Java.Util;

namespace GHC
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, Label = "@string/app_name", Icon = "@mipmap/ic_launcher")]
    public class SplashActivity : AppCompatActivity
    {
        Locale locale;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.Fullscreen);
            }

            try
            {
                //IntentFilter intentFilter = new IntentFilter("android.net.conn.CONNECTIVITY_CHANGE");
                //ApplicationContext.RegisterReceiver(new ConnectivityReceiver(), intentFilter);

                //Language settings
                ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
                Configuration config = BaseContext.Resources.Configuration;

                String lang = preferences.GetString("locale", null);
                if (lang != null)
                {
                    String systemLocale = GetSystemLocale(config).Language;
                    if (!"".Equals(lang) && !systemLocale.Equals(lang))
                    {
                        locale = new Locale(lang);
                        Locale.Default = locale;
                        SetSystemLocale(config, locale);
                        UpdateConfiguration(config);
                    }

                    string token = SettingsHelper.GetToken(this);
                    if (token == null)
                        StartActivity(typeof(LoginActivity));
                    else
                        StartActivity(typeof(MainActivity));
                }
                else
                {
                    StartActivity(typeof(LanguageActivity));
                }
            }
            catch
            {

            }
        }

        private static Locale GetSystemLocale(Configuration config)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return config.Locales.Get(0);
            }
            else
            {
                return config.Locale;
            }
        }


        private static void SetSystemLocale(Configuration config, Locale locale)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                config.SetLocale(locale);
            }
            else
            {
                config.Locale = locale;
            }
        }

        private void UpdateConfiguration(Configuration config)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            {
                BaseContext.CreateConfigurationContext(config);
            }
            else
            {
                BaseContext.Resources.UpdateConfiguration(config, BaseContext.Resources.DisplayMetrics);
            }
        }
    }
}