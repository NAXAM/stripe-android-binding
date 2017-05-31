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
using Android.Support.V7.App;
using Android.Support.V4.App;
namespace Demo.Controller
{
    public class PollingDialogController
    {

        AppCompatActivity mActivity;
        Android.Support.V7.App.AlertDialog mAlertDialog;

        public PollingDialogController(AppCompatActivity appCompatActivity)
        {
            mActivity = appCompatActivity;
        }

        public void ShowDialog(String url)
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(mActivity);
            View dialogView = LayoutInflater.From(mActivity).Inflate(Resource.Layout.polling_dialog, null);

            TextView linkView = dialogView.FindViewById<TextView>(Resource.Id.tv_link_redirect);
            linkView.SetText(Resource.String.verify);
            linkView.Click += (s, e) =>
            {
                string action = Intent.ActionView;
                Intent browserIntent = new Intent(action, Android.Net.Uri.Parse(url));
                mActivity.StartActivity(browserIntent);
            };
            builder.SetView(dialogView);

            mAlertDialog = builder.Create();
            mAlertDialog.Show();
        }

        public void DismissDialog()
        {
            if (mAlertDialog != null)
            {
                mAlertDialog.Dismiss();
                mAlertDialog = null;
            }
        }
    }
}