//
//  Adapty+JSON.cs
//  Adapty
//
//  Created by Aleksei Goncharov on 18.05.2023.
//
using System;
using System.Collections.Generic;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {

        internal static Adapty.PaywallProduct GetProduct(this JSONObject obj, string aKey)
        {
            return new Adapty.PaywallProduct(GetObject(obj, aKey));
        }

        internal static Adapty.Profile GetProfile(this JSONObject obj, string aKey)
        {
            return new Adapty.Profile(GetObject(obj, aKey));
        }

        internal static JSONNode ConvertDictionaryToJSONNode(Dictionary<String, bool> personalizedOffers) {
            var json = new JSONObject();

            foreach (var item in personalizedOffers) {

                json.Add(item.Key, item.Value);
            }

            return json;
        }
    }
}