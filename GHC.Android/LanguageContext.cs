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
using Android.Views;
using Android.Widget;
using Java.Util;

namespace GHC
{
    internal class LanguageContext : ContextWrapper
    {
        Locale locale;
        public LanguageContext(Context context) : base(context)
        {
            ////Language settings
            ////ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
            ////Configuration config = BaseContext.Resources.Configuration;

            ////String lang = preferences.GetString("locale", "en");
            ////String systemLocale = GetSystemLocale(config).Language;
            ////if (!"".Equals(lang) && !systemLocale.Equals(lang))
            ////{
            ////    locale = new Locale(lang);
            ////    Locale.Default = locale;
            ////    SetSystemLocale(config, locale);
            ////    UpdateConfiguration(config, context);
            ////}
        }

        public static LanguageContext NewLanguageAwareContext(String targetLanguage, Context context)
        {
            Resources resources = context.Resources;
            Configuration configuration = resources.Configuration;

            Locale newLocale = new Locale(targetLanguage);

            if (Build.VERSION.SdkInt > BuildVersionCodes.N)
            {
                configuration.SetLocale(newLocale);
                LocaleList.Default = (new LocaleList(newLocale));

                context = context.CreateConfigurationContext(configuration);
            }
            else if (Build.VERSION.SdkInt > BuildVersionCodes.JellyBeanMr1)
            {
                configuration.SetLocale(newLocale);
                context = context.CreateConfigurationContext(configuration);
            }
            else
            {
                resources.UpdateConfiguration(configuration, resources.DisplayMetrics);
            }

            return new LanguageContext(context);
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
                LocaleList.Default = (new LocaleList(locale));
            }
            else
            {
                config.Locale = locale;
            }
        }

        private void UpdateConfiguration(Configuration config, Context context)
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.JellyBeanMr1)
            {
                //BaseContext.CreateConfigurationContext(config);
                context = context.CreateConfigurationContext(config);
            }
            else
            {
                context.Resources.UpdateConfiguration(config, BaseContext.Resources.DisplayMetrics);
            }
        }
    }
}