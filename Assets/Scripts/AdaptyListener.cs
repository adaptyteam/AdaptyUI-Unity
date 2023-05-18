using System;
using System.Collections.Generic;
using AdaptySDK;
using UnityEngine;
using static AdaptySDK.Adapty;

namespace AdaptyExample {
    public class AdaptyListener : MonoBehaviour, AdaptyEventListener, AdaptyUIEventListener {
        AdaptyRouter Router;

        void Start() {
            this.Router = this.GetComponent<AdaptyRouter>();

            Adapty.SetLogLevel(Adapty.LogLevel.Verbose);
            Adapty.SetEventListener(this);
            AdaptyUI.SetEventListener(this);

            this.GetProfile();
        }

        public void GetProfile() {
            this.LogMethodRequest("GetProfile");

            Adapty.GetProfile((profile, error) => {
                this.LogMethodResult("GetProfile", error);

                if (profile != null) {
                    this.Router.SetProfile(profile);
                }
            });
        }

        public void GetPaywall(string id, string locale, Action<Adapty.Paywall> completionHandler) {
            this.LogMethodRequest("GetPaywall");

            Adapty.GetPaywall(id, locale, (paywall, error) => {
                this.LogMethodResult("GetPaywall", error);
                completionHandler.Invoke(paywall);
            });
        }

        public void GetPaywallProducts(Adapty.Paywall paywall, Action<IList<PaywallProduct>> completionHandler) {
            this.LogMethodRequest("GetPaywallProducts");

            Adapty.GetPaywallProducts(paywall, (products, error) => {
                this.LogMethodResult("GetPaywallProducts", error);
                completionHandler.Invoke(products);
            });

        }

        public void CreatePaywallView(Adapty.Paywall paywall, string locale, bool preloadProducts, Action<AdaptyUI.View> completionHandler) {
            this.LogMethodRequest("CreatePaywallView");

            AdaptyUI.CreatePaywallView(paywall, locale: locale, preloadProducts: preloadProducts, (view, error) => {
                this.LogMethodResult("CreatePaywallView", error);
                completionHandler.Invoke(view);
            });
        }

        public void PresentPaywallView(AdaptyUI.View view, Action<Error> completionHandler) {
            this.LogMethodRequest("PresentPaywallView");

            AdaptyUI.PresentPaywallView(view, (error) => {
                this.LogMethodResult("PresentPaywallView", error);

                if (completionHandler != null) {
                    completionHandler.Invoke(error);
                }
            });
        }

        public void DismissPaywallView(AdaptyUI.View view, Action<Error> completionHandler) {
            this.LogMethodRequest("DismissPaywallView");

            AdaptyUI.DismissPaywallView(view, (error) => {
                this.LogMethodResult("DismissPaywallView", error);

                if (completionHandler != null) {
                    completionHandler.Invoke(error);
                }

                this.Router.ResetVisualPaywallsView();
            });
        }

        public void Logout(Action<Error> completionHandler) {
            this.LogMethodRequest("Logout");

            Adapty.Logout((error) => {
                this.LogMethodResult("Logout", error);
                completionHandler.Invoke(error);
            });
        }

        // - Logging

        private void LogMethodRequest(string methodName) {
            Debug.Log(string.Format("#AdaptyListener# --> {0}", methodName));
        }

        private void LogMethodResult(string methodName, Error error) {
            if (error != null) {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} error {1}", methodName, error));

                this.Router.ShowAlertPanel(error.ToString());
            } else {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} success", methodName));
            }
        }

        private void LogIncomingCall_AdaptyUI(string methodName, AdaptyUI.View view, string meta) {
            Debug.Log(string.Format("#AdaptyListener# <-- {0}, viewId = {1}, meta = {2}", methodName, view.Id, meta));
        }

        // – AdaptyEventListener

        public void OnLoadLatestProfile(Adapty.Profile profile) {
            Debug.Log("#AdaptyListener# OnReceiveUpdatedProfile called");

            this.Router.SetProfile(profile);
        }

        // - AdaptyUIEventListener

        public void OnPerformAction(AdaptyUI.View view, AdaptyUI.Action action) {
            LogIncomingCall_AdaptyUI("OnPerformAction", view, action.Type.ToString());

            switch (action.Type) {
                case AdaptyUI.ActionType.Close:
                    this.DismissPaywallView(view, null);
                    break;
                default:
                    break;
            }
        }

        public void OnSelectProduct(AdaptyUI.View view, Adapty.PaywallProduct product) {
            LogIncomingCall_AdaptyUI("OnSelectProduct", view, product.VendorProductId);
        }

        public void OnStartPurchase(AdaptyUI.View view, Adapty.PaywallProduct product) {
            LogIncomingCall_AdaptyUI("OnStartPurchase", view, product.VendorProductId);
        }

        public void OnCancelPurchase(AdaptyUI.View view, Adapty.PaywallProduct product) {
            LogIncomingCall_AdaptyUI("OnCancelPurchase", view, product.VendorProductId);
        }

        public void OnFinishPurchase(AdaptyUI.View view, Adapty.PaywallProduct product, Adapty.Profile profile) {
            LogIncomingCall_AdaptyUI("OnFinishPurchase", view, string.Format("id: {0}, profile: {1}", product.VendorProductId, profile.ProfileId));

            var accessLevel = profile.AccessLevels["premium"];
            if (accessLevel != null && accessLevel.IsActive) {
                this.DismissPaywallView(view, null);
            }
        }

        public void OnFailPurchase(AdaptyUI.View view, Adapty.PaywallProduct product, Adapty.Error error) {
            LogIncomingCall_AdaptyUI("OnFailPurchase", view, string.Format("id: {0}, error: {1}", product.VendorProductId, error.ToString()));
        }

        public void OnFinishRestore(AdaptyUI.View view, Adapty.Profile profile) {
            LogIncomingCall_AdaptyUI("OnFinishRestore", view, profile.ProfileId);
        }

        public void OnFailRestore(AdaptyUI.View view, Adapty.Error error) {
            LogIncomingCall_AdaptyUI("OnFailRestore", view, error.ToString());
        }

        public void OnFailRendering(AdaptyUI.View view, Adapty.Error error) {
            LogIncomingCall_AdaptyUI("OnFailRendering", view, error.ToString());
        }

        public void OnFailLoadingProducts(AdaptyUI.View view, Adapty.Error error) {
            LogIncomingCall_AdaptyUI("OnFailLoadingProducts", view, error.ToString());
        }
    }

}