
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static AdaptySDK.Adapty;
using static AdaptySDK.AdaptyUI;

namespace AdaptySDK.iOS
{
#if UNITY_IOS
    internal static class AdaptyUIIOS {
        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUIUnity_createPaywallView")]
        private static extern void _CreatePaywallView(string paywall, string locale, bool preloadProducts, IntPtr callback);

        internal static void CreatePaywallView(string paywall, string locale, bool preloadProducts, string personalizedOffers, Action<string> completionHandler)
            => _CreatePaywallView(paywall, locale, preloadProducts, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUIUnity_presentPaywallView")]
        private static extern void _PresentPaywallView(string viewId, IntPtr callback);

        public static void PresentPaywallView(string viewId, Action<string> completionHandler)
            => _PresentPaywallView(viewId, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

      

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUIUnity_dismissPaywallView")]
        private static extern void _DismissPaywallView(string viewId, IntPtr callback);

        public static void DismissPaywallView(string viewId, Action<string> completionHandler)
            => _DismissPaywallView(viewId, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));
    }
#endif
}