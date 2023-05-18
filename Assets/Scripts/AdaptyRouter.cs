using System.Collections.Generic;
using AdaptySDK;
using UnityEngine;

namespace AdaptyExample {
    public class AdaptyRouter : MonoBehaviour {
        public RectTransform LoadingPanel;
        public AlertPanel AlertPanel;

        [Header("Sections Prefabs")]
        public RectTransform ContentTransform;

        public GameObject ProfileIdSectionPrefab;
        public GameObject ProfileInfoSectionPrefab;
        public GameObject VisualPaywallSectionPrefab;

        [HideInInspector]
        public ProfileIdSection ProfileIdSection;

        [HideInInspector]
        public ProfileInfoSection ProfileInfoSection;

        [HideInInspector]
        public List<VisualPaywallSection> VisualPaywallSections;

        private AdaptyListener listener;
        private Adapty.Profile profile;

        void Start() {
            this.listener = GetComponent<AdaptyListener>();
            this.ConfigureLayout();
            this.VisualPaywallSections = new List<VisualPaywallSection>();
        }

        void ConfigureLayout() {
            var offset = 20.0f;

            var profileIdSectionObj = Instantiate(this.ProfileIdSectionPrefab);
            var profileIdSectionRect =
                profileIdSectionObj.GetComponent<RectTransform>();
            profileIdSectionRect.SetParent(this.ContentTransform);
            profileIdSectionRect.anchoredPosition =
                new Vector3(profileIdSectionRect.position.x, -offset);

            offset += profileIdSectionRect.rect.height + 20.0f;

            var profileInfoSectionObj =
                Instantiate(this.ProfileInfoSectionPrefab);
            var profileInfoSectionRect =
                profileInfoSectionObj.GetComponent<RectTransform>();
            profileInfoSectionRect.SetParent(this.ContentTransform);
            profileInfoSectionRect.anchoredPosition =
                new Vector3(profileInfoSectionRect.position.x, -offset);

            offset += profileInfoSectionRect.rect.height + 20.0f;


            var profileIdSection = profileIdSectionObj.GetComponent<ProfileIdSection>();
            var profileInfoSection = profileInfoSectionObj.GetComponent<ProfileInfoSection>();

            profileInfoSection.Listener = this.listener;

            this.ProfileIdSection = profileIdSection;
            this.ProfileInfoSection = profileInfoSection;

            string[] paywallsIds = { "test_alexey", "volkswagen", "mazda" };

            foreach(var paywallId in paywallsIds)
            {
                var paywallSectionObj = Instantiate(this.VisualPaywallSectionPrefab);
                var paywallSectionRect = paywallSectionObj.GetComponent<RectTransform>();
                paywallSectionRect.SetParent(this.ContentTransform);

                paywallSectionRect.anchoredPosition = new Vector3(paywallSectionRect.position.x, -offset);

                offset += paywallSectionRect.rect.height + 20.0f;

                var paywallSection = paywallSectionObj.GetComponent<VisualPaywallSection>();
                paywallSection.Listener = this.listener;
                paywallSection.Router = this;

                paywallSection.PaywallId = paywallId;
                paywallSection.LoadPaywall();

                this.VisualPaywallSections.Add(paywallSection);
            }
        }

        public void ResetVisualPaywallsView() {
            this.VisualPaywallSections.ForEach((section) => { section.ResetPaywallView(); });
        }

        public void SetProfile(Adapty.Profile profile) {
            if (this.ProfileInfoSection != null && profile != null) {
                this.ProfileInfoSection.SetProfile(profile);
            }
            if (this.ProfileIdSection != null && profile != null) {
                this.ProfileIdSection.SetProfile(profile);
            }

            this.profile = profile;
        }

        public void SetIsLoading(bool isLoading) {
            this.LoadingPanel.gameObject.SetActive(isLoading);
        }

        public void ShowAlertPanel(string text) {
            this.AlertPanel.Text.SetText(text);
            this.AlertPanel.gameObject.SetActive(true);
        }

        public void HideAlertPanel() {
            this.AlertPanel.gameObject.SetActive(false);
        }
    }
}
