//
//  Action+JSON.cs
//  Adapty
//
//  Created by Aleksei Goncharov on 18.05.2023.
//
using System;

namespace AdaptySDK {
    using AdaptySDK.SimpleJSON;
    public static partial class AdaptyUI { 
        public partial class Action {
            internal Action(JSONObject jsonNode) {
                Type = jsonNode.GetActionType("type");
                Value = jsonNode.GetStringIfPresent("value");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON {
    internal static partial class JSONNodeExtensions {
        internal static AdaptyUI.Action GetAction(this JSONObject obj)
           => new AdaptyUI.Action(obj);

        internal static AdaptyUI.Action GetAction(this JSONNode node, string aKey)
           => new AdaptyUI.Action(GetObject(node, aKey));

        internal static AdaptyUI.ActionType GetActionType(this JSONNode node, string aKey)
            => GetString(node, aKey).ToActionType();

        internal static AdaptyUI.ActionType ToActionType(this string value) {
            switch (value) {
                case "close": return AdaptyUI.ActionType.Close;
                case "open_url": return AdaptyUI.ActionType.OpenUrl;
                case "custom": return AdaptyUI.ActionType.Custom;
                default: throw new Exception($"ActionType unknown value: {value}");
            }
        }
    }
}