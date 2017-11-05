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
using Com.Stripe.Android.Model;

namespace Demo.Activity
{
    [Activity(Label = "Demo", MainLauncher = true, Theme = "@style/SampleTheme", Icon = "@drawable/icon")]
    public class LauncherActivity : AppCompatActivity
    {
        private static string FUNCTIONAL_SOURCE_PUBLISHABLE_KEY =
            "pk_test_y3aqzMndgbjMxcHPuwYk2mXA";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_launcher);

            Button tokenButton = FindViewById<Button>(Resource.Id.btn_make_card_tokens);
            tokenButton.Click += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(PaymentActivity));
                StartActivity(intent);
            };

            Button sourceButton = FindViewById<Button>(Resource.Id.btn_make_sources);
            sourceButton.Click += (s, e) =>
            {
                // TODO Intent intent = new Intent(this, typeof(PollingActivity));
                // StartActivity(intent);
            };
        }
    }
}