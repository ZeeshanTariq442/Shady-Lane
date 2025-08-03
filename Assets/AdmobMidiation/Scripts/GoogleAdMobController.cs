using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Advertisement.IosSupport.Samples
{
    public class GoogleAdMobController : MonoBehaviour
    {
        public static GoogleAdMobController Instance;
        public List<AdTypeBO> AdsData;

        public GDPRUIController GDPR_UI;
        public GameObject CommercialBreakUI;
        public float CommercialBreakDisplayTime = 2;
        

        string GDPR = "";


        bool isInitDone = false;
        bool isInitCallSent = false;

        AdTypeBO.adUnitType CurrentAdType;
        bool IsCurrentAdRequestInSession = false;

        #region UNITY MONOBEHAVIOR METHODS
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Destroy duplicate
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitAllAdsData();
        }
        bool checkGDPRStatus()
        {
#if UNITY_IOS
            // check with iOS to see if the user has accepted or declined tracking
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                return false;
            }
            return true;

#else
            if (PlayerPrefs.GetString("GDPR", "") == "")
            {
                return false;

            }
            return true;
#endif


        }
        public void Start()
        {
            if (!checkGDPRStatus())
            {
                showGDPRPanel();
            }
            else
            {
                updateFromPrefGDPRStatus();
                callToInitMobileAds();
            }

        }
        void InitAllAdsData()
        {
            for(int i = 0; i < AdsData.Count; i++)
            {
                AdsData[i].Init();
            }
        }
        public void GDPRStatusUpdatedProceed()
        {

            updateFromPrefGDPRStatus();
            if (!isInitDone)
            {
                callToInitMobileAds();
            }

        }
        public void callToInitMobileAds()
        {
            MobileAds.SetiOSAppPauseOnBackground(true);



            // Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration = new RequestConfiguration();
            requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.Unspecified;
            requestConfiguration.TestDeviceIds.Add(AdRequest.TestDeviceSimulator);

            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            StartCoroutine(InternetConnectionChecker.Instance.CheckInternetConnection(isConnected =>
            {
                if (isConnected)
                {
                    if (!isInitCallSent && !isInitDone)
                    {
                        MobileAds.Initialize(HandleInitCompleteAction);
                        StartCoroutine(waitForTimeOut());
                    }

                }
                else
                {
                    // Set GA No Internet On Initialize
                }
            }));


        }
        IEnumerator waitForTimeOut()
        {
            yield return new WaitForSeconds(60);
            isInitCallSent = false;
        }



        // create your handler
        private void OnSdkInitializedEvent(string adUnitId)
        {
            // The SDK is initialized here. Ready to make ad requests.
        }

        private void HandleInitCompleteAction(InitializationStatus initStatus)
        {
            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.



            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
            //AppLovin.Initialize();
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
                {
                    string className = keyValuePair.Key;
                    AdapterStatus status = keyValuePair.Value;
                    switch (status.InitializationState)
                    {
                        case AdapterState.NotReady:
                        // The adapter initialization did not complete.
                        MonoBehaviour.print("Adapter: " + className + " not ready.");
                            break;
                        case AdapterState.Ready:
                        // The adapter was successfully initialized.
                        MonoBehaviour.print("Adapter: " + className + " is initialized.");
                            break;
                    }
                }
                isInitDone = true;
                RequestAllRewardedAds();
                if (AdsController.Instance != null && AdsController.Instance.IsOkToLoadShowAd())
                {
                    AdsController.Instance.RequestAd(AdTypeBO.adUnitType.MainBanner);
                }
            });
        }
        void RequestAllRewardedAds()
        {
            for(int i = 0; i < AdsData.Count; i++)
            {
                if(AdsData[i].Ad_Type== AdTypeBO.AdType.RewardedAd)
                {
                    if (AdsController.Instance != null)
                    {
                        AdsController.Instance.RequestAd(AdsData[i].Type);
                    }
                    else
                    {
                        RequestAdV2(AdsData[i].Type);
                    }
                }
            }
        }
        
        public void setGDPR(bool flag)
        {

            if (flag)
            {
                GDPR = "1";
                //Chartboost.AddDataUseConsent(CBGDPRDataUseConsent.Behavioral);
                //Vungle.UpdateConsentStatus(VungleConsent.ACCEPTED);
                //AdColonyAppOptions.SetGDPRRequired(true);
                //AdColonyAppOptions.SetGDPRConsentString("1");
            }
            else
            {
                GDPR = "0";
                //Chartboost.AddDataUseConsent(CBGDPRDataUseConsent.NonBehavioral);
                //Vungle.UpdateConsentStatus(VungleConsent.DENIED);
                //AdColonyAppOptions.SetGDPRRequired(false);
                //AdColonyAppOptions.SetGDPRConsentString("0");
            }
            // Unity 
            //UnityAds.SetGDPRConsentMetaData(flag);
            //AppLovin.SetHasUserConsent(flag);


            PlayerPrefs.SetString("GDPR", GDPR);
            GDPRStatusUpdatedProceed();
        }
        void updateFromPrefGDPRStatus()
        {
#if !UNITY_IOS
            GDPR = PlayerPrefs.GetString("GDPR", "");
#endif


        }
        public string GetGDPRValue()
        {
#if UNITY_IOS
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED || status == ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED)
            {
                return "1";
            }
            else
            {
                return  "0";
            }
#else

            return GDPR;
#endif
        }
        public void showGDPRPanel()
        {
            if (GDPR_UI != null)
            {
                GDPR_UI.OpenPanel(false);
                GA_Controller.Instance.LogDesignEvent("TermsofService:Show:Auto");
            }
            else
            {
                updateFromPrefGDPRStatus();
                callToInitMobileAds();
            }
        }


#endregion

#region HELPER METHODS

        public AdRequest CreateAdRequest(string gdprConcent)
        {
            Dictionary<string, string> ReqExtras = new Dictionary<string, string>();
            ReqExtras.Add("npa", gdprConcent);
            AdRequest AdReq = new AdRequest();
            AdReq.Extras = ReqExtras;

            return AdReq;
        }

        
        void ShowCommercialBreakUI()
        {
            IsCurrentAdRequestInSession = true;
            if (CommercialBreakUI != null)
            {
                CommercialBreakUI.SetActive(true);
                Invoke(nameof(ShowAd_Local), CommercialBreakDisplayTime);
                GA_Controller.Instance.LogDesignEvent("AdMobController:CommercialBreakUI");
            }
            else
            {
                ShowAd_Local();
            }
        }
        void ShowAd_Local()
        {
            IsCurrentAdRequestInSession = false;
            if (CommercialBreakUI != null )
            {
                CommercialBreakUI.SetActive(false);
            }

            AdTypeBO AdData = GetAdData(CurrentAdType);
            if (AdData != null)
            {
                AdData.ShowAd();
            }
        }

#endregion

#region TESTING_FUNCTIONS

        public void loadInterStetialAd()
        {
            AdsController.Instance.RequestAd(AdTypeBO.adUnitType.LevelEndInterstetial);
        }
        public void showInterStetialAd()
        {
            AdsController.Instance.ShowAd(AdTypeBO.adUnitType.LevelEndInterstetial);
        }

        public void loadRewardedAd()
        {
            AdsController.Instance.RequestAd(AdTypeBO.adUnitType.LevelEndRewardedVideo);
        }
        public void showRewardedAd()
        {
            AdsController.Instance.ShowAd(AdTypeBO.adUnitType.LevelEndRewardedVideo);
        }

        public void LoadBannerAd()
        {
            AdsController.Instance.RequestAd(AdTypeBO.adUnitType.MainBanner);
        }
        public void ShowBannerAd()
        {
            AdsController.Instance.ShowAd(AdTypeBO.adUnitType.MainBanner);
        }

        public void ShowMediationTestSuite()
        {
            //MediationTestSuite.Show();
        }

#endregion





        public bool GetAdLoadingVariable(AdTypeBO.adUnitType Type)
        {
            AdTypeBO AdData = GetAdData(Type);
            if (AdData != null)
            {
                return AdData.LoadingVariable;
            }
            return true;
        }
        public AdTypeBO GetAdData(AdTypeBO.adUnitType type)
        {
            for(int i=0;i< AdsData.Count; i++)
            {
                if(AdsData[i].Type== type)
                {
                    return AdsData[i];
                }
            }
            return null;
        }
        public void SetAdLoadingVariable(AdTypeBO.adUnitType Type)
        {
            AdTypeBO AdData = GetAdData(Type);
            if (AdData != null)
            {
                AdData.SetAdloadingVariable();
            }
        }
        public void ReSetAdLoadingVariable(AdTypeBO.adUnitType Type)
        {
            AdTypeBO AdData = GetAdData(Type);
            if (AdData != null)
            {
                AdData.ResetAdLoadingVariable();
            }
        }
        public bool IsAdLoaded(AdTypeBO.adUnitType Type)
        {
            AdTypeBO AdData = GetAdData(Type);
            if (AdData != null)
            {
                return AdData.IsAdLoaded();
            }
            return false;
        }
        public void OnAdLoaded(AdTypeBO.adUnitType Type){
             AdTypeBO AdData = GetAdData(Type);
                if (AdData != null)
                {
                    AdData.OnAdLoaded();
                }
        }
        public void RequestAdV2(AdTypeBO.adUnitType Type)
        {
            if (isInitDone)
            {
                AdTypeBO AdData = GetAdData(Type);
                if (AdData != null)
                {
                    AdData.RequestAd();
                }
            }
            else
            {
                GA_Controller.Instance.LogDesignEvent("Ads:RequestedButNotInitialized");
                callToInitMobileAds();
                ReSetAdLoadingVariable(Type);
            }
        }

        public void ShowAdV2(AdTypeBO.adUnitType type)
        {
            if (!IsCurrentAdRequestInSession)
            {
                if(type== AdTypeBO.adUnitType.MainBanner)
                {
                    AdTypeBO AdData = GetAdData(type);
                    if (AdData != null)
                    {
                        AdData.ShowAd();
                    }
                }
                else
                {
                    CurrentAdType = type;
                    ShowCommercialBreakUI();
                }
                
            }
            else
            {
                GA_Controller.Instance.LogDesignEvent("AdMobController:AdSessionInProgress");
            }

        }
        public bool IsGoodToCallRequestAd(AdTypeBO.adUnitType Type)
        {
            AdTypeBO AdData = GetAdData(Type);
            if (AdData != null)
            {
                return AdData.IsGoodToCallRequestAd();
            }
            return false;
        }

        public void HideBannerAd()
        {
            AdTypeBO AdData = GetAdData(AdTypeBO.adUnitType.MainBanner);
            if (AdData != null)
            {
                AdData.HideBannerAd();
            }
        }
    }
}
