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

namespace Demo.Controller
{
    public class ListViewController
    {

        private SimpleAdapter mAdatper;
        private IList<IDictionary<string, object>> mCardTokens = new List<IDictionary<string, object>>();
        private Context mContext;

        public ListViewController(ListView listView)
        {
            mContext = listView.Context;
            mAdatper = new SimpleAdapter(mContext, mCardTokens,
                    Resource.Layout.list_item_layout,
                    new string[] { "last4", "tokenId" },
                    new int[] { Resource.Id.last4, Resource.Id.tokenId });
            listView.Adapter = mAdatper;
        }

        public void AddToList(Token token)
        {
            AddToList(token.Card.Last4, token.Id);
        }

        public void AddToList(string last4, string tokenId)
        {
            string endingIn = mContext.GetString(Resource.String.endingIn);
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("last4", endingIn + " " + last4);
            map.Add("tokenId", tokenId);
            mCardTokens.Add(map as IDictionary<string, object>);
            mAdatper.NotifyDataSetChanged();
        }
    }

}