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
using Com.Stripe.Android.Model;
using Com.Stripe.Android;
using Java.Lang;

namespace Demo.Controller
{
    public class AsyncTaskTokenController
    {

       public CardInputWidget mCardInputWidget;
        public Context mContext;
        public static ErrorDialogHandler mErrorDialogHandler;
        public static ListViewController mOutputListController;
        public static ProgressDialogController mProgressDialogController;
        public string mPublishableKey;

        public AsyncTaskTokenController(
                Button button,
                CardInputWidget cardInputWidget,
                Context context,
                ErrorDialogHandler errorDialogHandler,
                ListViewController outputListController,
                ProgressDialogController progressDialogController,
                string publishableKey)
        {
            mCardInputWidget = cardInputWidget;
            mContext = context;
            mErrorDialogHandler = errorDialogHandler;
            mPublishableKey = publishableKey;
            mProgressDialogController = progressDialogController;
            mOutputListController = outputListController;

            button.Click += (s, e) =>
            {
                SaveCard();
            };
        }

        public void Detach()
        {
            mCardInputWidget = null;
        }

        private void SaveCard()
        {
            Card cardToSave = mCardInputWidget.Card;
            if (cardToSave == null)
            {
                mErrorDialogHandler.ShowError("Invalid Card Data");
                return;
            }
            mProgressDialogController.StartProgress();
            new Stripe(mContext).CreateToken(
                    cardToSave,
                    mPublishableKey,
                    new TokenCallBack());
        }

        public class TokenCallBack :Java.Lang.Object, ITokenCallback
        {
           
            public void OnError(Java.Lang.Exception error)
            {
                mErrorDialogHandler.ShowError(error.LocalizedMessage);
                mProgressDialogController.FinishProgress();
            }

            public void OnSuccess(Token token)
            {
                mOutputListController.AddToList(token);
                mProgressDialogController.FinishProgress();
            }
 
        }
    }

}