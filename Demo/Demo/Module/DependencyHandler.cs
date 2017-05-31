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
using Demo.Controller;

namespace Demo.Module
{
    public class DependencyHandler
    {

        /*
         * Change this to your publishable key.
         *
         * You can get your key here: https://manage.stripe.com/account/apikeys
         */
        private static string PUBLISHABLE_KEY = "pk_test_y3aqzMndgbjMxcHPuwYk2mXA";

        private AsyncTaskTokenController mAsyncTaskController;
        private CardInputWidget mCardInputWidget;
        private Context mContext;
        private ErrorDialogHandler mErrorDialogHandler;
        private IntentServiceTokenController mIntentServiceTokenController;
        private ListViewController mListViewController;
        private RxTokenController mRxTokenController;
        private ProgressDialogController mProgresDialogController;

        public DependencyHandler(
                AppCompatActivity activity,
                CardInputWidget cardInputWidget,
                ListView outputListView)
        {

            mCardInputWidget = cardInputWidget;
            mContext = activity.BaseContext;

            mProgresDialogController =
                    new ProgressDialogController(activity.SupportFragmentManager);

            mListViewController = new ListViewController(outputListView);

            mErrorDialogHandler = new ErrorDialogHandler(activity.SupportFragmentManager);
        }

        /**
         * Attach a listener that creates a token using the {@link android.os.AsyncTask}-based method.
         * Only gets attached once, unless you call {@link #clearReferences()}.
         *
         * @param button a button that, when clicked, gets a token.
         * @return a reference to the {@link AsyncTaskTokenController}
         */

        public AsyncTaskTokenController AttachAsyncTaskTokenController(Button button)
        {
            if (mAsyncTaskController == null)
            {
                mAsyncTaskController = new AsyncTaskTokenController(
                        button,
                        mCardInputWidget,
                        mContext,
                        mErrorDialogHandler,
                        mListViewController,
                        mProgresDialogController,
                        PUBLISHABLE_KEY);
            }
            return mAsyncTaskController;
        }

        /**
         * Attach a listener that creates a token using an {@link android.app.IntentService} and the
         * synchronous {@link com.stripe.android.Stripe#createTokenSynchronous(Card, string)} method.
         *
         * Only gets attached once, unless you call {@link #clearReferences()}.
         *
         * @param button a button that, when clicked, gets a token.
         * @return a reference to the {@link IntentServiceTokenController}
         */

        public IntentServiceTokenController AttachIntentServiceTokenController(
                AppCompatActivity appCompatActivity,
                Button button)
        {
            if (mIntentServiceTokenController == null)
            {
                mIntentServiceTokenController = new IntentServiceTokenController(
                        appCompatActivity,
                        button,
                        mCardInputWidget,
                        mErrorDialogHandler,
                        mListViewController,
                        mProgresDialogController,
                        PUBLISHABLE_KEY);
            }
            return mIntentServiceTokenController;
        }

        /**
         * Attach a listener that creates a token using a {@link rx.Subscription} and the
         * synchronous {@link com.stripe.android.Stripe#createTokenSynchronous(Card, string)} method.
         *
         * Only gets attached once, unless you call {@link #clearReferences()}.
         *
         * @param button a button that, when clicked, gets a token.
         * @return a reference to the {@link RxTokenController}
         */

        public RxTokenController AttachRxTokenController(Button button)
        {
            if (mRxTokenController == null)
            {
                mRxTokenController = new RxTokenController(
                        button,
                        mCardInputWidget,
                        mContext,
                        mErrorDialogHandler,
                        mListViewController,
                        mProgresDialogController,
                        PUBLISHABLE_KEY);
            }
            return mRxTokenController;
        }

        /**
         * Clear all the references so that we can start over again.
         */
        public void ClearReferences()
        {

            if (mAsyncTaskController != null)
            {
                mAsyncTaskController.Detach();
            }

            if (mRxTokenController != null)
            {
                // mRxTokenController.Detach();
            }

            if (mIntentServiceTokenController != null)
            {
                mIntentServiceTokenController.Detach();
            }

            mAsyncTaskController = null;
            mRxTokenController = null;
            mIntentServiceTokenController = null;
        }
    }

}