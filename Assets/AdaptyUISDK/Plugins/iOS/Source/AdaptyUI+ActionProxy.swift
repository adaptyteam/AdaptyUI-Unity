//
//  AdaptyUI+ActionProxy.swift
//  UnityFramework
//
//  Created by Alexey Goncharov on 11.10.23..
//

import Adapty
import AdaptyUI
import Foundation

extension AdaptyUI.Action: Encodable {
    enum CodingKeys: String, CodingKey {
        case type
        case value
    }

    public func encode(to encoder: Encoder) throws {
        var container = encoder.container(keyedBy: CodingKeys.self)
        
        switch self {
        case .close:
            try container.encode("close", forKey: .type)
        case let .openURL(url):
            try container.encode("open_url", forKey: .type)
            try container.encode(url.absoluteString, forKey: .value)
        case let .custom(id):
            try container.encode("custom", forKey: .type)
            try container.encode(id, forKey: .value)
        }
    }
}
