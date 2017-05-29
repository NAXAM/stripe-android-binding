using Android.App;
using Android.Widget;
using Android.OS;
using Com.Stripe.Android.View;

namespace StripeQs
{
    [Activity(Label = "StripeQs", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var mCardInputWidget = FindViewById<CardInputWidget>(Resource.Id.card_input_widget);

        }
    }
}

