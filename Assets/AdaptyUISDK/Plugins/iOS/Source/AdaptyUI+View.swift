//
//  AdaptyUI+View.swift
//  UnityFramework
//
//  Created by Alexey Goncharov on 11.10.23..
//

import Adapty
import AdaptyUI
import Foundation

extension AdaptyUI {
    struct View: Encodable {
        let id: String
        let templateId: String
        let paywallId: String
        let paywallVariationId: String

        enum CodingKeys: String, CodingKey {
            case id
            case templateId = "template_id"
            case paywallId = "paywall_id"
            case paywallVariationId = "paywall_variation_id"
        }
    }
}

extension AdaptyPaywallController {
    func toView() -> AdaptyUI.View {
        AdaptyUI.View(id: id.uuidString,
                      templateId: viewConfiguration.templateId,
                      paywallId: paywall.id,
                      paywallVariationId: paywall.variationId)
    }
}
