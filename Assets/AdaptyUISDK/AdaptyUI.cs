using System;
using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyUI = AdaptySDK.iOS.AdaptyUIIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyUI = AdaptySDK.Android.AdaptyUIAndroid;
#else
using _AdaptyUI = AdaptySDK.Noop.AdaptyUINoop;
#endif

namespace AdaptySDK {
    using AdaptySDK.SimpleJSON;

    public static partial class AdaptyUI {
        public static readonly string sdkVersion = "2.0.1";

        public static void CreatePaywallView(Adapty.Paywall paywall, string locale, bool preloadProducts, Action<View, Adapty.Error> completionHandler)
            => CreatePaywallView(paywall, locale, preloadProducts, null, completionHandler);

        public static void CreatePaywallView(Adapty.Paywall paywall, string locale, bool preloadProducts, Dictionary<String, bool> personalizedOffers, Action<View, Adapty.Error> completionHandler) {
            string paywallJson;
            try {
                paywallJson = paywall.ToJSONNode().ToString();
            } catch (Exception ex) {
                var error = new Adapty.Error(Adapty.ErrorCode.EncodingFailed, "Failed encoding Adapty.Paywall", $"AdaptyUnityError.EncodingFailed({ex})");
                try {
                    completionHandler(null, error);
                } catch (Exception e) {
                    throw new Exception("Failed to invoke Action<AdaptyUI.View, Adapty.Error> completionHandler in Adapty.CreatePaywallView(..)", e);
                }
                return;
            }

            string personalizedOffersString;

            try {
                personalizedOffersString = JSONNodeExtensions.ConvertDictionaryToJSONNode(personalizedOffers).ToString();
            } catch (Exception ex) {
                personalizedOffersString = null;
            }

            _AdaptyUI.CreatePaywallView(paywallJson, locale, preloadProducts, personalizedOffersString, (json) => {
                if (completionHandler == null) return;
                var response = json.ExtractViewOrError();
                try {
                    completionHandler(response.Value, response.Error);
                } catch (Exception e) {
                    throw new Exception("Failed to invoke Action<AdaptyUI.View, Adapty.Error> completionHandler in Adapty.CreatePaywallView(..)", e);
                }
            });
        }

        public static void PresentPaywallView(View view, Action<Adapty.Error> completionHandler)
            => _AdaptyUI.PresentPaywallView(view.Id, (json) => {
                if (completionHandler == null) return;
                var error = json.ExtractErrorIfPresent();
                try {
                    completionHandler(error);
                } catch (Exception e) {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyUI.PresentPaywallView(..)", e);
                }
            });

        public static void DismissPaywallView(View view, Action<Adapty.Error> completionHandler)
            => _AdaptyUI.DismissPaywallView(view.Id, (json) => {
                if (completionHandler == null) return;
                var error = json.ExtractErrorIfPresent();
                try {
                    completionHandler(error);
                } catch (Exception e) {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyUI.DismissPaywallView(..)", e);
                }
            });
    }
}