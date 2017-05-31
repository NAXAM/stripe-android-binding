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
using Demo.Module;
using Com.Stripe.Android;
using Com.Stripe.Android.Model;
using Com.Stripe.Android.View; 

namespace Demo.Activity
{
    [Activity(MainLauncher = false, Theme = "@style/SampleTheme")]
    public class PaymentActivity : AppCompatActivity
    {
        private DependencyHandler mDependencyHandler;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.payment_activity);

            mDependencyHandler = new DependencyHandler(
                    this,
                    FindViewById<CardInputWidget>(Resource.Id.card_input_widget),
                    FindViewById<ListView>(Resource.Id.listview));

            Button saveButton = FindViewById<Button>(Resource.Id.save);
            mDependencyHandler.AttachAsyncTaskTokenController(saveButton);

            Button saveRxButton = FindViewById<Button>(Resource.Id.saverx);
            mDependencyHandler.AttachRxTokenController(saveRxButton);

            Button saveIntentServiceButton = FindViewById<Button>(Resource.Id.saveWithService);
            mDependencyHandler.AttachIntentServiceTokenController(this, saveIntentServiceButton);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mDependencyHandler.ClearReferences();
        }
    }
}