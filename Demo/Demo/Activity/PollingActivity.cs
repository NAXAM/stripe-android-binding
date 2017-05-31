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
using Com.Stripe.Android.View;
using Demo.Adapter;
using Demo.Controller;
using Com.Stripe.Android.Model;
using Com.Stripe.Android;
using System.Reactive.Disposables;
using Android.Support.V7.Widget;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using Com.Stripe.Android.Net;
using Com.Stripe.Android.Exception; 

namespace Demo.Activity
{
    [Activity(Label = "Demo", MainLauncher = false, Theme = "@style/SampleTheme", Icon = "@drawable/icon")]
    public class PollingActivity : AppCompatActivity
    {
        private static string FUNCTIONAL_SOURCE_PUBLISHABLE_KEY =
            "pk_test_y3aqzMndgbjMxcHPuwYk2mXA";
        private static string RETURN_SCHEMA = "stripe://";
        private static string RETURN_HOST_ASYNC = "async";
        private static string RETURN_HOST_SYNC = "sync";

        private static string QUERY_CLIENT_SECRET = "client_secret";
        private static string QUERY_SOURCE_ID = "source";

        private CardInputWidget mCardInputWidget;
        private CompositeDisposable mCompositeSubscription;
        private static PollingAdapter mPollingAdapter;
        private static ErrorDialogHandler mErrorDialogHandler;
        private static PollingDialogController mPollingDialogController;
        private Source mPollingSource;
        private static ProgressDialogController mProgressDialogController;
        private Stripe mStripe;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_polling);

            mCompositeSubscription = new CompositeDisposable();
            mCardInputWidget = FindViewById<CardInputWidget>(Resource.Id.card_widget_three_d);
            mErrorDialogHandler = new ErrorDialogHandler(SupportFragmentManager);
            mProgressDialogController = new ProgressDialogController(SupportFragmentManager);
            mPollingDialogController = new PollingDialogController(this);
            mStripe = new Stripe(this);

            Button threeDSecureButton = FindViewById<Button>(Resource.Id.btn_three_d_secure);
            threeDSecureButton.Click += (s, e) =>
            {
                BeginSequence(false);
            };
            Button threeDSyncButton = FindViewById<Button>(Resource.Id.btn_three_d_secure_sync);
            threeDSyncButton.Click += (s, e) =>
            {
                BeginSequence(true);
            };

            RecyclerView recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);

            RecyclerView.LayoutManager linearLayoutManager = new LinearLayoutManager(this);
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(linearLayoutManager);
            mPollingAdapter = new PollingAdapter();
            recyclerView.SetAdapter(mPollingAdapter);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent.Data != null && intent.Data.Query != null)
            {
                // The client secret and source ID found here is identical to
                // that of the source used to get the redirect URL.

                String host = intent.Data.Host;
                String clientSecret = intent.Data.GetQueryParameter(QUERY_CLIENT_SECRET);
                String sourceId = intent.Data.GetQueryParameter(QUERY_SOURCE_ID);
                if (clientSecret != null
                        && sourceId != null
                        && clientSecret.Equals(mPollingSource.ClientSecret)
                        && sourceId.Equals(mPollingSource.Id))
                {
                    if (RETURN_HOST_SYNC.Equals(host))
                    {
                        PollSynchronouslyForSourceChanges(sourceId, clientSecret);
                    }
                    else
                    {
                        PollForSourceChanges(sourceId, clientSecret);
                    }
                }
                mPollingDialogController.DismissDialog();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mCompositeSubscription.Clear();
        }

        void BeginSequence(bool shouldPollWithBlockingMethod)
        {
            Card displayCard = mCardInputWidget.Card;
            if (displayCard == null)
            {
                mErrorDialogHandler.ShowError("Invalid Card Data");
                return;
            }
            CreateCardSource(displayCard, shouldPollWithBlockingMethod);
        }

        private void CreateCardSource(Card card, bool shouldPollWithBlockingMethod)
        {
            SourceParams cardSourceParams = SourceParams.CreateCardParams(card);
            IObservable<Source> cardSourceObservable = System.Reactive.Linq.Observable.FromAsync(() =>
            {
                return Task.Run(
                    () => mStripe.CreateSourceSynchronous(cardSourceParams, FUNCTIONAL_SOURCE_PUBLISHABLE_KEY));
            });

            mCompositeSubscription.Add(cardSourceObservable
              //.SubscribeOn(Scheduler.Immediate)
              .ObserveOn(Scheduler.CurrentThread)
              .Do((s) =>
              {
                  mProgressDialogController.SetMessageResource(Resource.String.createSource);
                  mProgressDialogController.StartProgress();
              })
              .Subscribe(
                (s) =>
                {
                    try
                    {
                        SourceCardData sourceCardData = (SourceCardData)s.SourceTypeModel;
                        mPollingAdapter.AddItem(
                            s.Status,
                            sourceCardData.ThreeDSecureStatus,
                            s.Id,
                            s.Type);
                        // If we need to get 3DS verification for this card, we
                        // first create a 3DS Source.
                        if (SourceCardData.Required.Equals(
                                sourceCardData.ThreeDSecureStatus))
                        {

                            // The card Source can be used to create a 3DS Source
                            CreateThreeDSecureSource(s.Id,
                                    shouldPollWithBlockingMethod);
                        }
                        else
                        {
                            mProgressDialogController.FinishProgress();
                        }
                    }
                    catch(Exception e)
                    {
                        mProgressDialogController.FinishProgress();
                        mErrorDialogHandler.ShowError(e.Message);
                    }
                    // Making a note of the Card Source in our list.
                    
                },
                (t) =>
                {
                    mErrorDialogHandler.ShowError(t.Message);
                }));
        }

        private void CreateThreeDSecureSource(string sourceId, bool shouldPollWithBlockingMethod)
        {
            SourceParams threeDParams = SourceParams.CreateThreeDSecureParams(
                1000L,
                "EUR",
                GetUrl(shouldPollWithBlockingMethod),
                sourceId);

            IObservable<Source> threeDSecureObservable = System.Reactive.Linq.Observable.FromAsync(() =>
            {
                return Task.Run(() => mStripe.CreateSourceSynchronous(
                                threeDParams,
                                FUNCTIONAL_SOURCE_PUBLISHABLE_KEY));
            });

            mCompositeSubscription.Add(threeDSecureObservable
                .SubscribeOn(Scheduler.Immediate)
                .ObserveOn(Scheduler.CurrentThread)
                .Do((s) => { }, (t) =>
                {
                    mProgressDialogController.FinishProgress();
                })
                .Subscribe(
                (source) =>
                {
                    ShowDialog(source);
                },
                (t) =>
                {
                    mErrorDialogHandler.ShowError(t.Message);
                }));
        }

        void ShowDialog(Source source)
        {
            // Caching the source object here because this app makes a lot of them.
            mPollingSource = source;
            mPollingDialogController.ShowDialog(source.SourceRedirect.Url);
        }

        void PollForSourceChanges(string sourceId, string clientSecret)
        {
            mProgressDialogController.SetMessageResource(Resource.String.pollingSource);
            mProgressDialogController.StartProgress();
            mStripe.PollSource(
                    sourceId,
                    clientSecret,
                    FUNCTIONAL_SOURCE_PUBLISHABLE_KEY,
                    new PollingResponseHandler(),
                null);
        }
        class PollingResponseHandler : Java.Lang.Object,IPollingResponseHandler
        {
              
            public void OnPollingResponse(PollingResponse pollingResponse)
            {
                mProgressDialogController.FinishProgress();
                UpdatePollingSourceList(pollingResponse);
            }
             
        }

        private void PollSynchronouslyForSourceChanges(string sourceId, string clientSecret)
        {
            IObservable<PollingResponse> sourceUpdateObservable = System.Reactive.Linq.Observable.FromAsync(
                () =>
                {
                    return Task.Run(() => mStripe.PollSourceSynchronous(
                                sourceId,
                                clientSecret,
                                FUNCTIONAL_SOURCE_PUBLISHABLE_KEY,
                                null));
                });
            mCompositeSubscription.Add(sourceUpdateObservable
                .SubscribeOn(Scheduler.Immediate)
                .ObserveOn(Scheduler.CurrentThread)
                .Do((s) =>
                {
                    mProgressDialogController.SetMessageResource(Resource.String.pollingSource);
                    mProgressDialogController.StartProgress();
                }, (t) =>
                {
                    mProgressDialogController.FinishProgress();
                })
                .Subscribe(
                (pollingResponse) =>
                {
                    UpdatePollingSourceList(pollingResponse);
                },
                (throwable) =>
                {
                    mErrorDialogHandler.ShowError(throwable.Message);
                }));
        }

        public static void UpdatePollingSourceList(PollingResponse pollingResponse)
        {
            Source source = pollingResponse.Source;
            if (source == null)
            {
                mPollingAdapter.AddItem(
                        "No source found",
                        "Stopped",
                        "Error",
                        "None");
                return;
            }

            if (pollingResponse.IsSuccess)
            {
                mPollingAdapter.AddItem(
                        source.Status,
                        "complete",
                        source.Id,
                        source.Type);
            }
            else if (pollingResponse.IsExpired)
            {
                mPollingAdapter.AddItem(
                        "Expired",
                        "Stopped",
                        source.Id,
                        source.Type);
            }
            else
            {
                StripeException stripeEx = pollingResponse.StripeException;
                if (stripeEx != null)
                {
                    mPollingAdapter.AddItem(
                            "error",
                            "ERR",
                            stripeEx.Message,
                            source.Type);
                }
                else
                {
                    mPollingAdapter.AddItem(
                            source.Status,
                            "failed",
                            source.Id,
                            source.Type);
                }
            }
        }
        private static string GetCountString(int count)
        {
            return string.Format("API Queries: %d", count);
        }
        private static String GetUrl(bool isSync)
        {
            if (isSync)
            {
                return RETURN_SCHEMA + RETURN_HOST_SYNC;
            }
            else
            {
                return RETURN_SCHEMA + RETURN_HOST_ASYNC;
            }
        }
    }
}