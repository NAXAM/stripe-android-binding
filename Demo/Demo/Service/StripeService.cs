using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Retrofit2.Http;
using Okhttp3;
using IO.Reactivex.Internal.Operators;

namespace Demo.Service
{
    public interface StripeService
    {
        [FormUrlEncoded]
        [POST(Value = "ephemeral_keys")]
        IObservable<ResponseBody> CreateEphemeralKey([FieldMap]IDictionary<string, string> apiVersionMap);
    }
}