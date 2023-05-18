//
//  View+JSON.cs
//  Adapty
//
//  Created by Aleksei Goncharov on 18.05.2023.
//
using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class AdaptyUI
    {
        public partial class View
        {
            internal View(JSONObject jsonNode)
            {
                Id = jsonNode.GetString("id");
                TemplateId = jsonNode.GetString("template_id");
                PaywallId = jsonNode.GetString("paywall_id");
                PaywallVariationId = jsonNode.GetString("paywall_variation_id");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON {
    internal static partial class JSONNodeExtensions {
        internal static AdaptyUI.View GetView(this JSONObject obj)
           => new AdaptyUI.View(obj);

        internal static AdaptyUI.View GetView(this JSONNode node, string aKey)
        => new AdaptyUI.View(GetObject(node, aKey));
    }
}