using System;
using System.Collections.Generic;
using Square.OkHttp3;
using Square.Retrofit2.Http;

namespace Demo.Service
{
    public interface StripeService
    {
        [FormUrlEncoded]
        [POST(Value = "ephemeral_keys")]
        IObservable<ResponseBody> CreateEphemeralKey([FieldMap]IDictionary<string, string> apiVersionMap);
    }
}