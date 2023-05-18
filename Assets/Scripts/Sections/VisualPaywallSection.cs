using System.Collections;
using System.Collections.Generic;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static AdaptySDK.Adapty;

public class VisualPaywallSection : MonoBehaviour
{
    public AdaptyListener Listener;
    public AdaptyRouter Router;

    public string PaywallId;

    public RectTransform ContainerTransform;
    public TextMeshProUGUI PaywallNameText;
    public TextMeshProUGUI LoadingStatusText;
    public TextMeshProUGUI VariationIdText;
    public TextMeshProUGUI RevisionText;

    public TMP_InputField LocaleTextField;

    private Adapty.Paywall m_paywall;

    void Start() {
        this.LocaleTextField.text = "en";
    }

    public void ResetPaywallView() {
        this.LoadPaywall();
    }

    public void LoadPaywall() {
        this.PaywallNameText.SetText(PaywallId);

        this.Listener.GetPaywall(PaywallId, "en", (paywall) => {
            if (paywall == null) {
                this.UpdatePaywallFail();
            } else {
                this.m_paywall = paywall;

                this.UpdatePaywall(paywall);
            }
        });
    }

    public void LoadAndPresentPaywall(bool preloadProducts) {
        if (m_paywall == null) return;

        var locale = this.LocaleTextField.text;

        this.Listener.CreatePaywallView(this.m_paywall, locale: locale, preloadProducts: false, (view) => {
            if (view == null) {
                //this.UpdateViewFail(paywall);
            } else {
                view.Present(null);
                //this.Listener.PresentPaywallView(view, (error) => { });
                //this.m_view = view;
                //this.UpdatePaywallData(paywall, null, view);
            }
        });
    }

    private void UpdatePaywallInitial() {
        this.PaywallNameText.SetText("null");
        this.LoadingStatusText.SetText("WAIT");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
    }

    private void UpdatePaywallFail() {
        this.LoadingStatusText.SetText("FAIL");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
    }

    private void UpdatePaywall(Adapty.Paywall paywall) {
        this.LoadingStatusText.SetText(paywall.HasViewConfiguration ? "HAS VIEW" : "NO VIEW");
        this.LoadingStatusText.color = paywall.HasViewConfiguration ? Color.green : Color.red;
        
        this.PaywallNameText.SetText(paywall.Id);
        this.VariationIdText.SetText(paywall.VariationId);
        this.RevisionText.SetText(paywall.Revision.ToString());
    }
}
