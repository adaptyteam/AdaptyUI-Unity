using System;
using System.Collections.Generic;
using UnityEngine;
namespace AdaptySDK.Android {
#if UNITY_ANDROID
    using AdaptyAndroidCallback = AdaptyAndroidCallbackAction;

    internal static class AdaptyUIAndroid
    {
        private static AndroidJavaClass AdaptyUIAndroidClass = new AndroidJavaClass("com.adapty.unity_ui.AdaptyUIAndroidWrapper");

        internal static void CreatePaywallView(string paywall, string locale, bool preloadProducts, string personalizedOffers, Action<string> completionHandler)
            => AdaptyUIAndroidClass.CallStatic("createView", paywall, locale, preloadProducts, personalizedOffers, AdaptyAndroidCallback.Action(completionHandler));

        internal static void PresentPaywallView(string viewId, Action<string> completionHandler)
            => AdaptyUIAndroidClass.CallStatic("presentView", viewId, AdaptyAndroidCallback.Action(completionHandler));

        internal static void DismissPaywallView(string viewId, Action<string> completionHandler)
            => AdaptyUIAndroidClass.CallStatic("dismissView", viewId, AdaptyAndroidCallback.Action(completionHandler));
    }
#endif
}