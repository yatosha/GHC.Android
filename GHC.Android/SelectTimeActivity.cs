using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GHC.Data;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class SelectTimeActivity : AppCompatActivity
    {
        TimePicker timePicker;
        Button btnSubmit;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            string theme = SettingsHelper.GetTheme(this);
            if (theme == "dark")
            {
                SetTheme(Resource.Style.Dark_Theme);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.select_time_activity);
            
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById(Resource.Id.toolbar).JavaCast<Android.Support.V7.Widget.Toolbar>();
            SetSupportActionBar(toolbar);

            ImageView backButton = FindViewById<ImageView>(Resource.Id.MenuButton);
            backButton.Click += ((sender, e) =>
            {
                OnBackPressed();
            });

            timePicker = FindViewById<TimePicker>(Resource.Id.timePicker);
            btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);

            btnSubmit.Click += (sender, e) =>
            {
                Intent.PutExtra("hour", timePicker.Hour);
                Intent.PutExtra("minute", timePicker.Minute);
                SetResult(Result.Ok, Intent);
                Finish();
            };
        }
    }
}