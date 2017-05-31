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
using Com.Stripe.Android.Model;
using Java.Lang;
using Com.Stripe.Android;
using Com.Stripe.Android.Exception;
using Android.Support.V4.Content;

namespace Demo.Service
{
    public class TokenIntentService : IntentService
    {

        public static string TOKEN_ACTION = "com.stripe.example.service.tokenAction";
        public static string STRIPE_CARD_LAST_FOUR = "com.stripe.example.service.cardLastFour";
        public static string STRIPE_CARD_TOKEN_ID = "com.stripe.example.service.cardTokenId";
        public static string STRIPE_ERROR_MESSAGE = "com.stripe.example.service.errorMessage";

        private static string EXTRA_CARD_NUMBER = "com.stripe.example.service.extra.cardNumber";
        private static string EXTRA_MONTH = "com.stripe.example.service.extra.month";
        private static string EXTRA_YEAR = "com.stripe.example.service.extra.year";
        private static string EXTRA_CVC = "com.stripe.example.service.extra.cvc";
        private static string EXTRA_PUBLISHABLE_KEY =
                "com.stripe.example.service.extra.publishablekey";

        public static Intent CreateTokenIntent(Android.App.Activity launchingActivity, string cardNumber = null, Integer month = null, Integer year = null, string cvc = null, string publishableKey = null)
        {
            return new Intent(launchingActivity, typeof(TokenIntentService))
                .PutExtra(EXTRA_CARD_NUMBER, cardNumber)
                .PutExtra(EXTRA_MONTH, month)
                .PutExtra(EXTRA_YEAR, year)
                .PutExtra(EXTRA_CVC, cvc)
                .PutExtra(EXTRA_PUBLISHABLE_KEY, publishableKey);
        }

        public TokenIntentService() : base("TokenIntentService")
        { 
        }

        protected override void OnHandleIntent(Intent intent)
        {
            string errorMessage = null;
            Token token = null;
            if (intent != null)
            {
                string cardNumber = intent.GetStringExtra(EXTRA_CARD_NUMBER);
                Integer month = (Integer)intent.Extras.Get(EXTRA_MONTH);
                Integer year = (Integer)intent.Extras.Get(EXTRA_YEAR);
                string cvc = intent.GetStringExtra(EXTRA_CVC);

                string publishableKey = intent.GetStringExtra(EXTRA_PUBLISHABLE_KEY);
                Card card = new Card(cardNumber, month, year, cvc);

                Stripe stripe = new Stripe(this);
                try
                {
                    token = stripe.CreateTokenSynchronous(card, publishableKey);
                }
                catch (StripeException stripeEx)
                {
                    errorMessage = stripeEx.LocalizedMessage;
                }
            }

            Intent localIntent = new Intent(TOKEN_ACTION);
            if (token != null)
            {
                localIntent.PutExtra(STRIPE_CARD_LAST_FOUR, token.Card.Last4);
                localIntent.PutExtra(STRIPE_CARD_TOKEN_ID, token.Id);
            }

            if (errorMessage != null)
            {
                localIntent.PutExtra(STRIPE_ERROR_MESSAGE, errorMessage);
            }

            // Broadcasts the Intent to receivers in this app.
            LocalBroadcastManager.GetInstance(this).SendBroadcast(localIntent);
        }
    }

}