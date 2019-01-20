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

namespace GHC
{
    [Application]
    public class GHCApp : Application
    {
        public GHCApp(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //SetupTracker();

            //AppCenter.Start("28a1a4e1-b729-456f-b514-257b4e7e8333",
            //       typeof(Analytics), typeof(Crashes));

            //PrintKeyHash();

            //RegisterActivityLifecycleCallbacks(this);

            //EnableStrictMode();

            //InitImageLoader();

            //UpdateLanguage(this);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                    .SetDefaultFontPath("fonts/SanFranciscoText-Regular.ttf")
                    //.SetFontAttrId(Resource.Attribute.fontPath)
                // Adding a custom view that support adding a typeFace
                // .AddCustomViewWithSetTypeface(Java.Lang.Class.FromType(typeof(CustomViewWithTypefaceSupport)))
                // Adding a custom style
                // .AddCustomStyle(Java.Lang.Class.FromType(typeof(TextField)), Resource.Attribute.textFieldStyle)
                .Build()
            );

            //AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            //{
            //    args.Handled = true;
            //};
        }
    }
}