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
using Android.Support.V7.Widget;
using Com.Stripe.Android.Model;

namespace Demo.Adapter
{



    public class PollingAdapter : RecyclerView.Adapter
    {
        public class ViewModel
        {
            public string mStatus;
            public string mRedirectStatus;
            public string mSourceId;
            public string mSourceType;

            public ViewModel(string Status, string redirectStatus, string sourceId, string sourceType)
            {
                mStatus = Status;
                mRedirectStatus = redirectStatus;
                mSourceId = sourceId;
                mSourceType = sourceType;
            }
        }

        public class PollingViewHolder : RecyclerView.ViewHolder
        {
            private TextView mStatusView;
            private TextView mRedirectStatusView;
            private TextView mSourceIdView;
            private TextView mSourceTypeView;
            public PollingViewHolder(LinearLayout pollingLayout) : base(pollingLayout)
            {

                mStatusView = pollingLayout.FindViewById<TextView>(Resource.Id.tv_ending_status);
                mRedirectStatusView = pollingLayout.FindViewById<TextView>(Resource.Id.tv_redirect_status);
                mSourceIdView = pollingLayout.FindViewById<TextView>(Resource.Id.tv_source_id);
                mSourceTypeView = pollingLayout.FindViewById<TextView>(Resource.Id.tv_source_type);
            }

            public void SetStatus(string status)
            {
                mStatusView.Text = status;
            }

            public void SetSourceId(string sourceId)
            {
                string last6 = sourceId == null || sourceId.Length < 6
                        ? sourceId
                        : sourceId.Substring(sourceId.Length - 6);
                mSourceIdView.Text = last6;
            }

            public void SetSourceType(string sourceType)
            {
                string viewableType = sourceType;
                if (Source.ThreeDSecure.Equals(sourceType))
                {
                    viewableType = "3DS";
                }
                mSourceTypeView.Text = viewableType;
            }

            public void SetRedirectStatus(string redirectStatus)
            {
                mRedirectStatusView.Text = redirectStatus;
            }


        }

        private List<ViewModel> mDataset = new List<ViewModel>();
        public PollingAdapter() { }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LinearLayout pollingView = (LinearLayout)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.polling_list_item, parent, false);
            PollingViewHolder vh = new PollingViewHolder(pollingView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var hold = (PollingViewHolder)holder;
            ViewModel model = mDataset.ElementAt(position);
            hold.SetStatus(model.mStatus);
            hold.SetRedirectStatus(model.mRedirectStatus);
            hold.SetSourceId(model.mSourceId);
            hold.SetSourceType(model.mSourceType);
        }

        public override int ItemCount
        {
            get
            {
                return mDataset.Count;
            }
        }

        public void AddItem(string Status, string redirectStatus, string sourceId, string sourceType)
        {
            mDataset.Add(new ViewModel(Status, redirectStatus, sourceId, sourceType));
            NotifyDataSetChanged();
        }

    }

}