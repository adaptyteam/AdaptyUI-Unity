//
//  Result+JSON.cs
//  Adapty
//
//  Created by Aleksei Goncharov on 18.05.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK.SimpleJSON {
    internal static partial class JSONNodeExtensions {
        internal static Adapty.Result<AdaptyUI.View> ExtractViewOrError(this string json)
        {
            Adapty.Error error = null;
            AdaptyUI.View view = null;

            try {
                var response = JSONNode.Parse(json);
                error = response.GetErrorIfPresent("error");
                if (error is null) {
                    view = response.GetView("success");
                }

            } catch (Exception ex) {
                error = new Adapty.Error(Adapty.ErrorCode.DecodingFailed, "Failed decoding AdaptyUI.View Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<AdaptyUI.View>(view, error);
        }
    }
}