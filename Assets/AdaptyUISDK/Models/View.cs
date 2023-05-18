using System;

namespace AdaptySDK
{
    public static partial class AdaptyUI
    {
        public partial class View
        {
            /// Unique identifier of the access level configured by you in Adapty Dashboard.
            public readonly string Id;

            /// Unique identifier of the access level configured by you in Adapty Dashboard.
            public readonly string TemplateId;

            /// Unique identifier of the access level configured by you in Adapty Dashboard.
            public readonly string PaywallId;

            /// Unique identifier of the access level configured by you in Adapty Dashboard.
            public readonly string PaywallVariationId;

            public void Present(Action<Adapty.Error> completionHandler)
                => AdaptyUI.PresentPaywallView(this, completionHandler);

            public void Dismiss(Action<Adapty.Error> completionHandler)
                => AdaptyUI.DismissPaywallView(this, completionHandler);
        }
    }
}