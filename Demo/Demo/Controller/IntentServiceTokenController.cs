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
using Com.Stripe.Android.View;
using Android.Support.V7.App;
using Android.Support.V4.Content;
using Demo.Service;
using Com.Stripe.Android.Model;

namespace Demo.Controller
{
    public class IntentServiceTokenController
    {

        private Android.App.Activity mActivity;
        private CardInputWidget mCardInputWidget;
        private static ErrorDialogHandler mErrorDialogHandler;
        private static ListViewController mOutputListViewController;
        private static ProgressDialogController mProgressDialogController;
        private string mPublishableKey;

        private TokenBroadcastReceiver mTokenBroadcastReceiver;

        public IntentServiceTokenController(
                AppCompatActivity appCompatActivity,
                Button button,
                CardInputWidget cardInputWidget,
                ErrorDialogHandler errorDialogHandler,
                ListViewController outputListController,
                ProgressDialogController progressDialogController,
                string publishableKey)
        {

            mActivity = appCompatActivity;
            mCardInputWidget = cardInputWidget;
            mErrorDialogHandler = errorDialogHandler;
            mOutputListViewController = outputListController;
            mProgressDialogController = progressDialogController;
            mPublishableKey = publishableKey;

            button.Click += (s, e) =>
            {
                SaveCard();
            };
            RegisterBroadcastReceiver();
        }

        /**
         * Unregister the {@link BroadcastReceiver}.
         */
        public void Detach()
        {
            if (mTokenBroadcastReceiver != null)
            {
                LocalBroadcastManager.GetInstance(mActivity)
                        .UnregisterReceiver(mTokenBroadcastReceiver);
                mTokenBroadcastReceiver = null;
                mActivity = null;
            }
            mCardInputWidget = null;
        }

        private void RegisterBroadcastReceiver()
        {
            mTokenBroadcastReceiver = new TokenBroadcastReceiver();
            LocalBroadcastManager.GetInstance(mActivity).RegisterReceiver(
                    mTokenBroadcastReceiver,
                    new IntentFilter(TokenIntentService.TOKEN_ACTION));
        }

        private void SaveCard()
        {
            Card cardToSave = mCardInputWidget.Card;
            if (cardToSave == null)
            {
                mErrorDialogHandler.ShowError("Invalid Card Data");
                return;
            }
            Intent tokenServiceIntent = TokenIntentService.CreateTokenIntent(
                    mActivity,
                    cardToSave.Number,
                    cardToSave.ExpMonth,
                    cardToSave.ExpYear,
                    cardToSave.CVC,
                    mPublishableKey);
            mProgressDialogController.StartProgress();
            mActivity.StartService(tokenServiceIntent);
        }

        private class TokenBroadcastReceiver : BroadcastReceiver
        {

            // Prevent instantiation of a local broadcast receiver outside this class.
            public TokenBroadcastReceiver() { }


            public override void OnReceive(Context context, Intent intent)
            {
                mProgressDialogController.FinishProgress();

                if (intent == null)
                {
                    return;
                }

                if (intent.HasExtra(TokenIntentService.STRIPE_ERROR_MESSAGE))
                {
                    mErrorDialogHandler.ShowError(
                            intent.GetStringExtra(TokenIntentService.STRIPE_ERROR_MESSAGE));
                    return;
                }

                if (intent.HasExtra(TokenIntentService.STRIPE_CARD_TOKEN_ID) &&
                        intent.HasExtra(TokenIntentService.STRIPE_CARD_LAST_FOUR))
                {
                    mOutputListViewController.AddToList(
                            intent.GetStringExtra(TokenIntentService.STRIPE_CARD_LAST_FOUR),
                            intent.GetStringExtra(TokenIntentService.STRIPE_CARD_TOKEN_ID));
                }
            }


        }
    }
}