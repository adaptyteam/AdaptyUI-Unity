## ⚠️ This repository is deprecated because AdaptyUI is now part of AdaptySDK. Please consider updating. ⚠️

<h1 align="center" style="border-bottom: none">
<b>
    <a href="https://adapty.io/?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-iOS">
        <img src="https://adapty-portal-media-production.s3.amazonaws.com/github/logo-adapty-new.svg">
    </a>
</b>
<br>Adapty UI
</h1>

<p align="center">
<a href="https://go.adapty.io/subhub-community-flutter-rep"><img src="https://img.shields.io/badge/Adapty-discord-purple"></a>
<a href="https://github.com/adaptyteam/AdaptySDK-Flutter/blob/master/LICENSE"><img src="https://img.shields.io/badge/license-MIT-brightgreen.svg"></a>
</p>

**AdaptyUI** is an open-source framework that is an extension to the Adapty SDK that allows you to easily add purchase screens to your application. It’s 100% open-source, native, and lightweight.

### [1. Fetching Paywalls & ViewConfiguration](https://docs.adapty.io/docs/paywall-builder-fetching)

Paywall can be obtained in the way you are already familiar with:

```csharp
using AdaptySDK;

Adapty.GetPaywall(id, (paywall, error) => {
  // handle the error and use the paywall
});
```

After fetching the paywall call the `AdaptyUI.createPaywallView()` method to assembly the view:

```csharp
using AdaptySDK;

AdaptyUI.CreatePaywallView(paywall, locale: 'en', preloadProducts: true, (view, error) => {
  // use the view
});
```

### [2. Presenting Visual Paywalls](https://docs.adapty.io/docs/paywall-builder-presenting-unity)

In order to display the visual paywall on the device screen, you may just simply call `.Present()` method of the view, obtained during the previous step:

```csharp
view.Present((error) => {
  // handle the error
});
```

### 3. Full Documentation and Next Steps

We recommend that you read the [full documentation](https://docs.adapty.io/docs/paywall-builder-getting-started). If you are not familiar with Adapty, then start [here](https://docs.adapty.io/docs).

## Contributing

- Feel free to open an issue, we check all of them or drop us an email at [support@adapty.io](mailto:support@adapty.io) and tell us everything you want.
- Want to suggest a feature? Just contact us or open an issue in the repo.

## Like AdaptyUI?

So do we! Feel free to star the repo ⭐️⭐️⭐️ and make our developers happy!

## License

AdaptyUI is available under the MIT license. [Click here](https://github.com/adaptyteam/AdaptyUI-Unity/blob/master/LICENSE) for details.

---
