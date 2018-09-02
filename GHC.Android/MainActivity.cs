using System;
using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Calligraphy;
using Android.Content;
using PL.Bclogic.Pulsator4droid.Library;
using AndroidViewAnimations;
using Android.Support.V7.Widget;
using GHC.Adapters;
using Android.Views.Animations;
using Android.Preferences;
using Android.Runtime;
using System.Threading;
using Square.Picasso;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        PulsatorLayout pulsator;
        ImageView btnGo;
        RecyclerView recyclerView;
        TextView helpView;

        string[] menuItems = { "History", "Settings" };
        AppState appState = AppState.Idle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            pulsator = FindViewById<PulsatorLayout>(Resource.Id.pulsator);
            btnGo = FindViewById<ImageView>(Resource.Id.btnGo);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            helpView = FindViewById<TextView>(Resource.Id.help);

            GridLayoutManager lm = new GridLayoutManager(this, 2);
            recyclerView.SetLayoutManager(lm);

            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(recyclerView.Context, DividerItemDecoration.Vertical);
            DividerItemDecoration dividerItemDecoration2 = new DividerItemDecoration(recyclerView.Context, DividerItemDecoration.Horizontal);
            recyclerView.AddItemDecoration(dividerItemDecoration);
            recyclerView.AddItemDecoration(dividerItemDecoration2);

            MainMenuAdapter adapter = new MainMenuAdapter(menuItems);
            recyclerView.SetAdapter(adapter);

            adapter.ItemClick += Adapter_ItemClick;

            btnGo.Click += BtnGo_Click;

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //         SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //         fab.Click += FabOnClick;
        }

        private void Adapter_ItemClick(object sender, MainMenuAdapterClickEventArgs e)
        {
            if (e.Position == 0)
            {

            }
            else
            {

            }
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            if (pulsator.IsStarted)
            {
                if (appState == AppState.Searching)
                {
                    appState = AppState.Cancelled;
                    Picasso.With(this).Load(Resource.Drawable.button_cancelled).Into(btnGo);
                    pulsator.Visibility = ViewStates.Gone;
                    pulsator.Stop();
                    pulsator.RequestLayout();

                    helpView.SetText(Resource.String.service_cancelled);
                    Animation animation = new TranslateAnimation(0, 0, 250, 0);
                    animation.Duration = 300;
                    animation.FillAfter = true;
                    helpView.StartAnimation(animation);
                }
                if (appState == AppState.Cancelled)
                {
                    appState = AppState.Idle;
                    Picasso.With(this).Load(Resource.Drawable.button).Into(btnGo);
                    pulsator.Visibility = ViewStates.Gone;
                    pulsator.Stop();
                    pulsator.RequestLayout();

                    helpView.SetText(Resource.String.tap_here);
                }
                
                //pulsator.StartAnimation(animation);
            }
            else
            {
                //YoYo.With(Techniques.SlideOutUp)
                //.Duration(400)
                //.WithRepeatListener(listener => { })
                //.PlayOn(FindViewById(Resource.Id.btnGo));

                //YoYo.With(Techniques.SlideOutUp)
                //.Duration(400)
                //.WithRepeatListener(listener => { })
                //.PlayOn(FindViewById(Resource.Id.pulsator));

                if (appState == AppState.Idle)
                {
                    Intent intent = new Intent(this, typeof(ServicesActivity));
                    StartActivityForResult(intent, 200);
                }

                //pulsator.Visibility = ViewStates.Visible;
                //helpView.Text = "SEARCHING FOR NEARBY MEDICAL PERSONNEL";

                //Animation animation = new TranslateAnimation(0, 0, 0, 250);
                //animation.Duration = 300;
                //animation.FillAfter = true;
                //helpView.StartAnimation(animation);

                //pulsator.Stop();
                //pulsator.Start();
            }
        }



        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //int id = item.ItemId;
            //if (id == Resource.Id.action_settings)
            //{
            //    return true;
            //}

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == 200 && resultCode == Result.Ok)
            {
                appState = AppState.Searching;
                Picasso.With(this).Load(Resource.Drawable.button_searching).Into(btnGo);
                pulsator.Visibility = ViewStates.Visible;
                helpView.SetText(Resource.String.searching);

                Animation animation = new TranslateAnimation(0, 0, 0, 250);
                animation.Duration = 300;
                animation.FillAfter = true;
                helpView.StartAnimation(animation);

                pulsator.Stop();
                pulsator.Start();
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }



        // Required by Calligraphy
        protected override void AttachBaseContext(Context newBase)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(newBase);

            String lang = preferences.GetString("locale", "en");
            newBase = LanguageContext.NewLanguageAwareContext(lang, newBase);
            Context context = new LanguageContext(newBase);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }
    }

    public enum AppState
    {
        Idle, Searching, Waiting, ReceivingTreatment, Cancelled, Done
    }
}

