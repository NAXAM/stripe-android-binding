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
using System.Reactive;
using Com.Stripe.Android.Model;
using Com.Stripe.Android;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace Demo.Controller
{
    public class RxTokenController
    {

        private CardInputWidget mCardInputWidget;
        private CompositeDisposable mCompositeSubscription;
        private Context mContext;
        private static ErrorDialogHandler mErrorDialogHandler;
        private static ListViewController mOutputListController;
        private static ProgressDialogController mProgressDialogController;
        private string mPublishableKey;

        public RxTokenController(
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
            mOutputListController = outputListController;
            mProgressDialogController = progressDialogController;
            mPublishableKey = publishableKey;
            mCompositeSubscription = new CompositeDisposable();
            // 
            mCompositeSubscription.Add(Disposable.Create(() =>
            {
                SaveCard();
            }));

            
        }
        public void Detach()
        {
            if (mCompositeSubscription != null)
            {
                mCompositeSubscription.Clear();
            }
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

            Stripe stripe = new Stripe(mContext);

            IObservable<Token> tokenObservable = Observable.FromAsync((s) =>
           {
               return Task.Run(() => stripe.CreateTokenSynchronous(cardToSave, mPublishableKey));
           });
            mCompositeSubscription.Add(
                tokenObservable
                .SubscribeOn<Token>(Scheduler.Immediate)
                .ObserveOn<Token>(Scheduler.CurrentThread)
                .Do<Token>(
                    (token) =>
                    {
                        mProgressDialogController.StartProgress();
                    },
                    (b) =>
                    {
                        mProgressDialogController.FinishProgress();
                    })
                .Subscribe(
                    (token) =>
                    {
                        mOutputListController.AddToList(token);
                    },
                    (throwable) =>
                    {
                        mErrorDialogHandler.ShowError(throwable.Message);
                    }));

        }
    }
}