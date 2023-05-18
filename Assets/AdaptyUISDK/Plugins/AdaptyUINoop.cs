using System;
using System.Collections.Generic;

namespace AdaptySDK.Noop {
    internal static class AdaptyUINoop {
        internal static void CreatePaywallView(string paywall, string locale, bool preloadProducts, string personalizedOffers, Action<string> completionHandler) { }

        internal static void PresentPaywallView(string viewId, Action<string> completionHandler) { }

        internal static void DismissPaywallView(string viewId, Action<string> completionHandler) { }
    }

    internal static class AdaptyUINoopCallbackAction {
        internal static void InitializeOnce() { }
    }
}