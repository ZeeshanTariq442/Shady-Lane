using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Unity.Advertisement.IosSupport.Samples
{
    public class GDPRUIController : MonoBehaviour
    {
        public GameObject NextButton;
        public GameObject AcceptButton;
        public GameObject DeclineButton;
        public GameObject ExitButton;

        bool CanClickExitButton = false;
        // Start is called before the first frame update
        void Start()
        {
            DisableAllButtons();
#if UNITY_IOS
        EnableIOSButtons();
#else
            EnableAndroidButtons();
#endif
        }
        void DisableAllButtons()
        {
            NextButton.SetActive(false);
            AcceptButton.SetActive(false);
            DeclineButton.SetActive(false);
        }
        void EnableAndroidButtons()
        {
            AcceptButton.SetActive(true);
            DeclineButton.SetActive(true);
        }
        void EnableIOSButtons()
        {
            NextButton.SetActive(true);
        }
        public void OpenPanel(bool NeedToShowCrossButton)
        {
            this.gameObject.SetActive(true);
            ExitButton.SetActive(NeedToShowCrossButton);
            CanClickExitButton = NeedToShowCrossButton;
        }
        public void ClosePanel()
        {
            this.gameObject.SetActive(false);
        }

        public void OnAcceptButtonPressed()
        {
            if (!AdsController.Instance.CanClick)
            {
                return;
            }
            AdsController.Instance.OnButtonClick();
            AdsController.Instance.PlayButtonClickSound();
            GoogleAdMobController.Instance.setGDPR(true);
            ClosePanel();
            GA_Controller.Instance.LogDesignEvent("TermsofService:Accept");
        }
        public void OnDeclineButtonPressed()
        {
            if (!AdsController.Instance.CanClick)
            {
                return;
            }
            AdsController.Instance.OnButtonClick();
            AdsController.Instance.PlayButtonClickSound();
            GoogleAdMobController.Instance.setGDPR(false);
            ClosePanel();
            GA_Controller.Instance.LogDesignEvent("TermsofService:Decline");
        }
        public void OnNextButtonPressed()
        {
            if (!AdsController.Instance.CanClick)
            {
                return;
            }
            AdsController.Instance.OnButtonClick();
            AdsController.Instance.PlayButtonClickSound();
#if UNITY_IOS
            ATTrackingStatusBinding.RequestAuthorizationTracking();
#endif
            ClosePanel();
            GA_Controller.Instance.LogDesignEvent("TermsofService:Next");
        }
        public void OnExitButtonPressed()
        {
            if (!AdsController.Instance.CanClick)
            {
                return;
            }
            AdsController.Instance.OnButtonClick();
            if (CanClickExitButton)
            {
                AdsController.Instance.PlayButtonClickSound();
                ClosePanel();
            }
            GA_Controller.Instance.LogDesignEvent("TermsofService:Close");

        }
    }
}
