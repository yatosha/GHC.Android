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
using Calligraphy;
using GHC.Data;

namespace GHC
{
    [Application]
    public class GHCApp : Application
    {
        public GHCApp(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public async override void OnCreate()
        {
            base.OnCreate();

            //SetupTracker();

            //AppCenter.Start("28a1a4e1-b729-456f-b514-257b4e7e8333",
            //       typeof(Analytics), typeof(Crashes));

            await Repository.InitializeDatabase();

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                    .SetDefaultFontPath("fonts/Poppins-SemiBold.ttf")
                .Build()
            );
        }
    }
}