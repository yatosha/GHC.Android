using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GHC.Data
{
    public static class SettingsHelper
    {
        static string prefsFile = "Preferences";
        const string KEY_TOKEN = "KEY_TOKEN";
        const string KEY_DEVICE_TOKEN = "KEY_DEVICE_TOKEN";

        public static void SaveToken(string token, Context context)
        {
            //We need an Editor object to make preference changes.
            ISharedPreferences preferences = context.GetSharedPreferences(prefsFile, FileCreationMode.Private);
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutString(KEY_TOKEN, token);

            //Commit the edits
            editor.Commit();
        }

        public static string GetToken(Context context)
        {
            ISharedPreferences preferences = context.GetSharedPreferences(prefsFile, FileCreationMode.Private);
            string token = preferences.GetString(KEY_TOKEN, null);
            return token;
        }

        public static void SaveDeviceToken(string token, Context context)
        {
            //We need an Editor object to make preference changes.
            ISharedPreferences preferences = context.GetSharedPreferences(prefsFile, FileCreationMode.Private);
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutString(KEY_DEVICE_TOKEN, token);

            //Commit the edits
            editor.Commit();
        }

        public static string GetDeviceToken(Context context)
        {
            ISharedPreferences preferences = context.GetSharedPreferences(prefsFile, FileCreationMode.Private);
            string token = preferences.GetString(KEY_DEVICE_TOKEN, null);
            return token;
        }
    }
}