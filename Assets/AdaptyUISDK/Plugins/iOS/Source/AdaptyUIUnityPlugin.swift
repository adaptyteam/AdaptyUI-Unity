import Adapty
import AdaptyUI
import UIKit

extension UIViewController {
    var isOrContainsAdaptyController: Bool {
        guard let presentedViewController = presentedViewController else {
            return self is AdaptyPaywallController
        }
        return presentedViewController is AdaptyPaywallController
    }
}

@objc public class AdaptyUIUnityPlugin: NSObject {
    public typealias JSONStringCompletion = (String?) -> Void

    @objc public static let shared = AdaptyUIUnityPlugin()

    override public init() {
        super.init()
    }

    private var paywallControllers = [UUID: AdaptyPaywallController]()

    private func cachePaywallController(_ controller: AdaptyPaywallController, id: UUID) {
        paywallControllers[id] = controller
    }

    private func deleteCachedPaywallController(_ id: String) {
        guard let uuid = UUID(uuidString: id) else { return }
        paywallControllers.removeValue(forKey: uuid)
    }

    private func cachedPaywallController(_ id: String) -> AdaptyPaywallController? {
        guard let uuid = UUID(uuidString: id) else { return nil }
        return paywallControllers[uuid]
    }

    private func createView(
        paywall: AdaptyPaywall,
        products: [AdaptyPaywallProduct]?,
        viewConfiguration: AdaptyUI.LocalizedViewConfiguration
    ) -> AdaptyUI.View {
        let vc = AdaptyUI.paywallController(for: paywall,
                                            products: nil,
                                            viewConfiguration: viewConfiguration,
                                            delegate: Self.delegate)

        cachePaywallController(vc, id: vc.id)

        return vc.toView()
    }

    private func preloadProductsAndCreateView(
        paywall: AdaptyPaywall,
        preloadProducts: Bool,
        viewConfiguration: AdaptyUI.LocalizedViewConfiguration,
        completion: @escaping (Result<AdaptyUI.View, AdaptyError>) -> Void
    ) {
        guard preloadProducts else {
            let view = createView(paywall: paywall,
                                  products: nil,
                                  viewConfiguration: viewConfiguration)
            completion(.success(view))
            return
        }

        Adapty.getPaywallProducts(paywall: paywall) { [weak self] result in
            guard let self = self else { return }

            switch result {
            case let .success(products):
                let view = self.createView(paywall: paywall,
                                           products: products,
                                           viewConfiguration: viewConfiguration)
                completion(.success(view))
            case .failure:
                // TODO: log error
                let view = self.createView(paywall: paywall,
                                           products: nil,
                                           viewConfiguration: viewConfiguration)
                completion(.success(view))
            }
        }
    }

    @objc public func handleCreateView(
        _ paywallJson: String,
        locale: String,
        preloadProducts: Bool,
        completion: JSONStringCompletion? = nil
    ) {
        let paywall: AdaptyPaywall

        do {
            paywall = try AdaptyUnityPlugin.decode(AdaptyPaywall.self, from: paywallJson)
        } catch {
            let error = AdaptyUnityPlugin.PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        AdaptyUI.getViewConfiguration(forPaywall: paywall, locale: locale) { [weak self] result in
            switch result {
            case let .success(config):
                self?.preloadProductsAndCreateView(
                    paywall: paywall,
                    preloadProducts: preloadProducts,
                    viewConfiguration: config,
                    completion: { result in
                        switch result {
                        case let .success(view):
                            completion?(AdaptyUnityPlugin.encodeToString(result: .success(view)))
                        case let .failure(error):
                            completion?(AdaptyUnityPlugin.encodeToString(result: AdaptyResult<[String: String]>.failure(error)))
                        }
                    }
                )
            case let .failure(error):
                completion?(AdaptyUnityPlugin.encodeToString(result: AdaptyResult<[String: String]>.failure(error)))
            }
        }
    }

    @objc public func handlePresentView(_ id: String, completion: JSONStringCompletion? = nil) {
        presentViewImpl(id, completion: { result in
            let pluginResult = AdaptyUnityPlugin.PluginResult<Bool, AdaptyError>.from(result)
            completion?(AdaptyUnityPlugin.encodeToString(result: pluginResult))
        })
    }
    
    @objc public func handleDismissView(_ id: String, completion: JSONStringCompletion? = nil) {
        dismissViewImpl(id, completion: { result in
            let pluginResult = AdaptyUnityPlugin.PluginResult<Bool, AdaptyError>.from(result)
            completion?(AdaptyUnityPlugin.encodeToString(result: pluginResult))
        })
    }

    private func presentViewImpl(_ id: String, completion: @escaping AdaptyResultCompletion<Bool>) {
        guard let vc = cachedPaywallController(id) else {
            completion(.failure(AdaptyError(AdaptyUIUnityError.viewNotFound(id))))
            return
        }

        vc.modalPresentationCapturesStatusBarAppearance = true
        vc.modalPresentationStyle = .overFullScreen

        guard let rootVC = UIApplication.shared.windows.first?.rootViewController else {
            completion(.failure(AdaptyError(AdaptyUIUnityError.viewPresentationError(id))))
            return
        }

        guard !rootVC.isOrContainsAdaptyController else {
            completion(.failure(AdaptyError(AdaptyUIUnityError.viewAlreadyPresented(id))))
            return
        }

        rootVC.present(vc, animated: true) {
            completion(.success(true))
        }
    }

    private func dismissViewImpl(_ id: String, completion: @escaping AdaptyResultCompletion<Bool>) {
        guard let vc = cachedPaywallController(id) else {
            completion(.failure(AdaptyError(AdaptyUIUnityError.viewNotFound(id))))
            return
        }

        vc.dismiss(animated: true) { [weak self] in
            self?.deleteCachedPaywallController(id)
            completion(.success(true))
        }
    }
}
