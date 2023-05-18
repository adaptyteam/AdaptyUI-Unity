using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyUIAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK {
    using System.Reflection;
    using AdaptySDK.SimpleJSON;
    using static AdaptySDK.AdaptyUI;
    using Unity.VisualScripting;

    public interface AdaptyUIEventListener {
        void OnPerformAction(AdaptyUI.View view, AdaptyUI.Action action) { }
        void OnSelectProduct(AdaptyUI.View view, Adapty.PaywallProduct product) { }
        void OnStartPurchase(AdaptyUI.View view, Adapty.PaywallProduct product) { }
        void OnCancelPurchase(AdaptyUI.View view, Adapty.PaywallProduct product) { }
        void OnFinishPurchase(AdaptyUI.View view, Adapty.PaywallProduct product, Adapty.Profile profile) { }
        void OnFailPurchase(AdaptyUI.View view, Adapty.PaywallProduct product, Adapty.Error error) { }
        void OnFinishRestore(AdaptyUI.View view, Adapty.Profile profile) { }
        void OnFailRestore(AdaptyUI.View view, Adapty.Error error) { }
        void OnFailRendering(AdaptyUI.View view, Adapty.Error error) { }
        void OnFailLoadingProducts(AdaptyUI.View view, Adapty.Error error) { }
    }

    class AdaptyUnknownEventListenerImpl : AdaptyUnknownEventListener {
        private const string _paywallViewDidPerformActionName = "paywall_view_did_perform_action";
        private const string _paywallViewDidSelectProductName = "paywall_view_did_select_product";
        private const string _paywallViewDidStartPurchaseName = "paywall_view_did_start_purchase";
        private const string _paywallViewDidCancelPurchaseName = "paywall_view_did_cancel_purchase";
        private const string _paywallViewDidFinishPurchaseName = "paywall_view_did_finish_purchase";
        private const string _paywallViewDidFailPurchaseName = "paywall_view_did_fail_purchase";
        private const string _paywallViewDidFinishRestoreName = "paywall_view_did_finish_restore";
        private const string _paywallViewDidFailRestoreName = "paywall_view_did_fail_restore";
        private const string _paywallViewDidFailRenderingName = "paywall_view_did_fail_rendering";
        private const string _paywallViewDidFailLoadingProductsName = "paywall_view_did_fail_loading_products";

        private AdaptyUIEventListener m_listener;

        public AdaptyUnknownEventListenerImpl(AdaptyUIEventListener listener)
        {
            m_listener = listener;
        }

        public void OnUnknownMessage(string type, JSONNode response) {
            UnityEngine.Debug.Log(string.Format("#AdaptyUI# OnMessage <-- {0}, {1}", type, response));

            if (!response.IsObject)
            {
                UnityEngine.Debug.Log(string.Format("#AdaptyUI# OnMessage <-- Not an Object!"));
                return;
            }

            var responseObject = response.AsObject;

            switch (type) {
                case _paywallViewDidPerformActionName:
                    m_listener.OnPerformAction(
                        responseObject.GetView("view"),
                        responseObject.GetAction("action")
                    );
                    break;
                 case _paywallViewDidSelectProductName:
                    m_listener.OnSelectProduct(
                        responseObject.GetView("view"),
                        responseObject.GetProduct("product")
                    );
                    break;
                case _paywallViewDidStartPurchaseName:
                    m_listener.OnStartPurchase(
                        responseObject.GetView("view"),
                        responseObject.GetProduct("product")
                    );
                    break;
                case _paywallViewDidCancelPurchaseName:
                    m_listener.OnCancelPurchase(
                        responseObject.GetView("view"),
                        responseObject.GetProduct("product")
                    );
                    break;
                case _paywallViewDidFinishPurchaseName:
                    m_listener.OnFinishPurchase(
                        responseObject.GetView("view"),
                        responseObject.GetProduct("product"),
                        responseObject.GetProfile("profile")
                    );
                    break;
                case _paywallViewDidFailPurchaseName:
                    m_listener.OnFailPurchase(
                        responseObject.GetView("view"),
                        responseObject.GetProduct("product"),
                        responseObject.GetError("error")
                    );
                    break;
                case _paywallViewDidFinishRestoreName:
                    m_listener.OnFinishRestore(
                        responseObject.GetView("view"),
                        responseObject.GetProfile("profile")
                    );
                    break;
                case _paywallViewDidFailRestoreName:
                    m_listener.OnFailRestore(
                        responseObject.GetView("view"),
                        responseObject.GetError("error")
                    );
                    break;
                case _paywallViewDidFailRenderingName:
                    m_listener.OnFailRendering(
                        responseObject.GetView("view"),
                        responseObject.GetError("error")
                    );
                    break;
                case _paywallViewDidFailLoadingProductsName:
                    m_listener.OnFailLoadingProducts(
                        responseObject.GetView("view"),
                        responseObject.GetError("error")
                    );
                    break;
            }
        }
    }

    public static partial class AdaptyUI {

        public static void SetEventListener(AdaptyUIEventListener listener) {
            Adapty.SetUnknownEventListener(new AdaptyUnknownEventListenerImpl(listener));
        }
    }
}