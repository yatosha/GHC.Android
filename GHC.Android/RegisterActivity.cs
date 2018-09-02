using System;
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
using GHC.Data;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class RegisterActivity : AppCompatActivity
    {
        EditText txtName, txtPhone, txtPassword, txtPasswordRepeat, txtPricingScheme;
        Button btnRegister;

        Android.Support.V7.App.AlertDialog alertDialog;

        long selectedScheme = 0;

        List<PricingCategory> schemes;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register_activity);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById(Resource.Id.toolbar).JavaCast<Android.Support.V7.Widget.Toolbar>();
            toolbar.SetContentInsetsAbsolute(0, 0);
            SetSupportActionBar(toolbar);

            ImageView backButton = FindViewById<ImageView>(Resource.Id.MenuButton);
            backButton.Click += ((sender, e) =>
            {
                OnBackPressed();
            });

            txtName = FindViewById<EditText>(Resource.Id.txtName);
            txtPhone = FindViewById<EditText>(Resource.Id.txtPhone);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            txtPasswordRepeat = FindViewById<EditText>(Resource.Id.txtPasswordRepeat);
            txtPricingScheme = FindViewById<EditText>(Resource.Id.txtPricingScheme);
            btnRegister = FindViewById<Button>(Resource.Id.btnRegister);

            txtPricingScheme.Click += TxtPricingScheme_Click;
            btnRegister.Click += BtnRegister_Click;

            schemes = await ServiceHelper.GetPricingCategories();
            if (schemes != null)
            {
                var dialogView = LayoutInflater.Inflate(Resource.Layout.pricing_schemes_list, null);
                using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                {
                    dialog.SetTitle(Resource.String.select_pricing_scheme);
                    dialog.SetView(dialogView);
                    dialog.SetNegativeButton("Cancel", (s, a) => { });
                    alertDialog = dialog.Create();
                }
                var items = schemes.Select(x => x.Name).ToArray();
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
                ListView listView = dialogView.FindViewById<ListView>(Resource.Id.listView);
                listView.Adapter = adapter;
                listView.ItemClick += ListView_ItemClick;
            }
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            selectedScheme = schemes[e.Position].Id;
            txtPricingScheme.Text = schemes[e.Position].Name;
            alertDialog.Dismiss();
        }

        //TO DO: Validation
        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string phone = txtPhone.Text;
            if (phone.StartsWith("0"))
            {
                phone = phone.Remove(0, 1);
                phone = $"+255{phone}";
            }

            CustomerViewModel model = new CustomerViewModel()
            {
                Name = txtName.Text,
                Password = txtPassword.Text,
                Phone = phone,
                PricingCategoryId = selectedScheme
            };

            ProgressDialog progress = new ProgressDialog(this, Resource.Style.MyAlertDialogStyle);
            progress.SetMessage(Resources.GetString(Resource.String.authenticating));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.Indeterminate = true;
            progress.SetCancelable(false);
            progress.Show();

            bool registered = await ServiceHelper.Register(model);
            if (registered)
            {
                progress.SetMessage(Resources.GetString(Resource.String.authenticating));
                string token = await ServiceHelper.Authenticate(phone, txtPassword.Text);
                if (token != null)
                {
                    SettingsHelper.SaveToken(token, this);
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                else
                {
                    string loginFailed = Resources.GetString(Resource.String.login_failed);
                    string loginFailedDetails = Resources.GetString(Resource.String.login_failed_details);
                    AlertUser(loginFailed, loginFailedDetails);
                }
            }
            else
            {
                string registrationFailed = Resources.GetString(Resource.String.registration_failed);
                string registrationFailedDetails = Resources.GetString(Resource.String.registration_failed_details);
                AlertUser(registrationFailed, registrationFailedDetails);
            }

            progress.Dismiss();
        }

        private void TxtPricingScheme_Click(object sender, EventArgs e)
        {
            if (alertDialog != null)
                alertDialog.Show();
        }

        private void AlertUser(string title, string message)
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetPositiveButton("Ok", (senderAlert, args) => {

            });
            Dialog dialog = alert.Create();
            dialog.Show();
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
}