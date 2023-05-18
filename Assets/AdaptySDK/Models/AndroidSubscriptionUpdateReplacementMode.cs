﻿//
//  AndroidSubscriptionUpdateReplacementMode.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum AndroidSubscriptionUpdateReplacementMode
        {
            WithTimeProration,
            ChargeProratedPrice,
            WithoutProration,
            Deferred,
            ChargeFullPrice
        }
    }
}