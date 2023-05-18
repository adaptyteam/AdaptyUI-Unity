//
//  AdaptyUI+Delegate.swift
//  UnityFramework
//
//  Created by Alexey Goncharov on 11.10.23..
//

import Adapty
import AdaptyUI
import Foundation

enum ArgumentName: String, Encodable {
    case view
    case action
    case product
    case profile
    case error
}

enum MethodName: String {
    case createView = "create_view"
    case presentView = "present_view"
    case dismissView = "dismiss_view"

    case paywallViewDidPerformAction = "paywall_view_did_perform_action"
    case paywallViewDidSelectProduct = "paywall_view_did_select_product"
    case paywallViewDidStartPurchase = "paywall_view_did_start_purchase"
    case paywallViewDidCancelPurchase = "paywall_view_did_cancel_purchase"
    case paywallViewDidFinishPurchase = "paywall_view_did_finish_purchase"
    case paywallViewDidFailPurchase = "paywall_view_did_fail_purchase"
    case paywallViewDidFinishRestore = "paywall_view_did_finish_restore"
    case paywallViewDidFailRestore = "paywall_view_did_fail_restore"
    case paywallViewDidFailRendering = "paywall_view_did_fail_rendering"
    case paywallViewDidFailLoadingProducts = "paywall_view_did_fail_loading_products"

    case notImplemented = "not_implemented"
}

extension AdaptyUIUnityPlugin {
    static let delegate = AdaptyUIDelegateWrapper()
}

extension AdaptyUIUnityPlugin {
    struct AdaptyDidPerformActionEvent: Encodable {
        let view: AdaptyUI.View
        let action: AdaptyUI.Action
    }

    struct AdaptyPurchaseEvent: Encodable {
        let view: AdaptyUI.View
        let product: AdaptyPaywallProduct
    }

    struct AdaptyPurchaseFinishEvent: Encodable {
        let view: AdaptyUI.View
        let product: AdaptyPaywallProduct
        let profile: AdaptyProfile
    }

    struct AdaptyRestoreFinishEvent: Encodable {
        let view: AdaptyUI.View
        let profile: AdaptyProfile
    }

    struct AdaptyPurchaseFailEvent: Encodable {
        let view: AdaptyUI.View
        let product: AdaptyPaywallProduct
        let error: AdaptyError
    }

    struct AdaptyErrorEvent: Encodable {
        let view: AdaptyUI.View
        let error: AdaptyError
    }

    class AdaptyUIDelegateWrapper: NSObject, AdaptyPaywallControllerDelegate {
        private func invokeMethod<T: Encodable>(_ methodName: MethodName, event: T) {
            do {
                let jsonString = try AdaptyUnityPlugin.encodeToString(event)
                AdaptyUnityPlugin.messageDelegate?(methodName.rawValue, jsonString)
            } catch {
                AdaptyUI.writeLog(level: .error,
                                  message: "Plugin encoding error: \(error.localizedDescription)")
            }
        }

        func paywallController(_ controller: AdaptyPaywallController, didPerform action: AdaptyUI.Action) {
            invokeMethod(.paywallViewDidPerformAction,
                         event: AdaptyDidPerformActionEvent(view: controller.toView(),
                                                            action: action))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didSelectProduct product: AdaptyPaywallProduct) {
            invokeMethod(.paywallViewDidSelectProduct,
                         event: AdaptyPurchaseEvent(view: controller.toView(),
                                                    product: product))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didStartPurchase product: AdaptyPaywallProduct) {
            invokeMethod(.paywallViewDidStartPurchase,
                         event: AdaptyPurchaseEvent(view: controller.toView(),
                                                    product: product))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didCancelPurchase product: AdaptyPaywallProduct) {
            invokeMethod(.paywallViewDidCancelPurchase,
                         event: AdaptyPurchaseEvent(view: controller.toView(),
                                                    product: product))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didFinishPurchase product: AdaptyPaywallProduct,
                                      purchasedInfo: AdaptyPurchasedInfo) {
            invokeMethod(.paywallViewDidFinishPurchase,
                         event: AdaptyPurchaseFinishEvent(view: controller.toView(),
                                                          product: product,
                                                          profile: purchasedInfo.profile))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didFailPurchase product: AdaptyPaywallProduct,
                                      error: AdaptyError) {
            invokeMethod(.paywallViewDidFailPurchase,
                         event: AdaptyPurchaseFailEvent(view: controller.toView(),
                                                        product: product,
                                                        error: error))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didFinishRestoreWith profile: AdaptyProfile) {
            invokeMethod(.paywallViewDidFinishRestore,
                         event: AdaptyRestoreFinishEvent(view: controller.toView(), profile: profile))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didFailRestoreWith error: AdaptyError) {
            invokeMethod(.paywallViewDidFailRestore,
                         event: AdaptyErrorEvent(view: controller.toView(), error: error))
        }

        public func paywallController(_ controller: AdaptyPaywallController,
                                      didFailRenderingWith error: AdaptyError) {
            invokeMethod(.paywallViewDidFailRendering,
                         event: AdaptyErrorEvent(view: controller.toView(), error: error))
        }

        public func paywallController(_ controller: AdaptyPaywallController, didFailLoadingProductsWith error: AdaptyError) -> Bool {
            invokeMethod(.paywallViewDidFailLoadingProducts,
                         event: AdaptyErrorEvent(view: controller.toView(), error: error))

            return true
        }
    }
}
