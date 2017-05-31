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

namespace Demo.Controller
{

    public class ListViewController
    {


        private SimpleAdapter mAdatper;
        private List<IDictionary<string, object>> mCardTokens;
        private Context mContext;
        ListView MyListView;
        public ListViewController(ListView listView)
        {
            MyListView = listView;
            mContext = MyListView.Context;
            mCardTokens = new List<IDictionary<string, object>>(); 
            mAdatper = new SimpleAdapter(mContext, mCardTokens,
                    Resource.Layout.list_item_layout,
                    new string[] { "last4", "tokenId" },
                    new int[] { Resource.Id.last4, Resource.Id.tokenId });
            MyListView.Adapter = mAdatper;
        }

        public void AddToList(Token token)
        {
            AddToList(token.Card.Last4, token.Id);
        }

        public void AddToList(string last4, string tokenId)
        {
            string endingIn = mContext.GetString(Resource.String.endingIn);
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("last4", endingIn + " " + last4);
            map.Add("tokenId", tokenId);
            mCardTokens.Add(map);
            mAdatper.NotifyDataSetChanged();
        }
    }

}