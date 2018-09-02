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
using Android.Transitions;
using Android.Views;
using Android.Widget;
using AndroidViewAnimations;
using Calligraphy;
using GHC.Data;

namespace GHC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class LoginActivity : AppCompatActivity, Transition.ITransitionListener
    {
        EditText txtPhone, txtPassword;
        Button btnLogin;
        TextView tvRegisterHint;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login_activity);

            txtPhone = FindViewById<EditText>(Resource.Id.txtPhone);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            tvRegisterHint = FindViewById<TextView>(Resource.Id.tvRegisterHint);

            txtPhone.Visibility = ViewStates.Invisible;
            txtPassword.Visibility = ViewStates.Invisible;
            btnLogin.Visibility = ViewStates.Invisible;

            Transition sharedElementEnterTransition = Window.SharedElementEnterTransition;
            sharedElementEnterTransition.AddListener(this);
            if (sharedElementEnterTransition.Targets.Count < 1)
            {
                DisplayViews();
            }
            

            btnLogin.Click += BtnLogin_Click;
            tvRegisterHint.Click += TvRegisterHint_Click;
        }

        private void TvRegisterHint_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.email_password_required), ToastLength.Short).Show();
                return;
            }

            string phone = txtPhone.Text;
            if (phone.StartsWith("0"))
            {
                phone = phone.Remove(0, 1);
                phone = $"+255{phone}";
            }

            ProgressDialog progress = new ProgressDialog(this, Resource.Style.MyAlertDialogStyle);
            progress.SetMessage(Resources.GetString(Resource.String.authenticating));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.Indeterminate = true;
            progress.SetCancelable(false);
            progress.Show();

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

            progress.Dismiss();
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

        protected override void AttachBaseContext(Context newBase)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(newBase);

            String lang = preferences.GetString("locale", "en");
            newBase = LanguageContext.NewLanguageAwareContext(lang, newBase);
            Context context = new LanguageContext(newBase);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }

        public void OnTransitionCancel(Transition transition)
        {
            
        }

        public void OnTransitionEnd(Transition transition)
        {
            DisplayViews();
        }

        private void DisplayViews()
        {
            YoYo.With(Techniques.FadeInUp)
                            .Duration(400)
                            .WithStartListener(animator => { txtPhone.Visibility = ViewStates.Visible; })
                            .PlayOn(FindViewById(Resource.Id.txtPhone));

            YoYo.With(Techniques.FadeInUp)
                .Duration(400)
                .WithStartListener(animator => { txtPassword.Visibility = ViewStates.Visible; })
                .PlayOn(FindViewById(Resource.Id.txtPassword));

            YoYo.With(Techniques.FadeInUp)
                .Duration(400)
                .WithStartListener(animator => { btnLogin.Visibility = ViewStates.Visible; })
                .PlayOn(FindViewById(Resource.Id.btnLogin));
        }

        public void OnTransitionPause(Transition transition)
        {
            
        }

        public void OnTransitionResume(Transition transition)
        {
            
        }

        public void OnTransitionStart(Transition transition)
        {
            
        }
    }
}