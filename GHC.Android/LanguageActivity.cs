using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Util;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Transitions;
using Android.Views;
using Android.Widget;
using Calligraphy;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class LanguageActivity : AppCompatActivity
    {
        RecyclerView recyclerView;
        ImageView overlay, logo;

        LanguagesAdapter adapter;
        string locale = "en";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.language_activity);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            LinearLayoutManager lm = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(lm);

            string[] languages = { "Kiswahili", "English" };
            adapter = new LanguagesAdapter(languages);
            recyclerView.SetAdapter(adapter);

            adapter.ItemClick += Adapter_ItemClick;

            overlay = FindViewById<ImageView>(Resource.Id.overlay);
            logo = FindViewById<ImageView>(Resource.Id.logo);

            Button btnContinue = FindViewById<Button>(Resource.Id.btnContinue);
            btnContinue.Click += BtnContinue_Click;
        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutString("locale", locale);
            editor.Commit();


            Intent intent = new Intent(this, typeof(LoginActivity));
            //ActivityOptionsCompat options = ActivityOptionsCompat.
            //    MakeSceneTransitionAnimation(this, (View)overlay, "background");

            Pair p1 = Pair.Create(overlay, "background");
            Pair p2 = Pair.Create(logo, "logo");

            ActivityOptionsCompat options = ActivityOptionsCompat.
                MakeSceneTransitionAnimation(this, p1, p2);


            StartActivity(intent, options.ToBundle());
        }

        private void Adapter_ItemClick(object sender, LanguagesAdapterClickEventArgs e)
        {
            if (e.Position == 0)
            {
                locale = "sw";
                adapter.SelectedPosition = 0;
                adapter.NotifyDataSetChanged();
            }
            else
            {
                locale = "en";
                adapter.SelectedPosition = 1;
                adapter.NotifyDataSetChanged();
            }
        }

        // Required by Calligraphy
        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }
    }
}