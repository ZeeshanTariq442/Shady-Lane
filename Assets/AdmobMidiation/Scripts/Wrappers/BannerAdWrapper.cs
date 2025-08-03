using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
namespace Unity.Advertisement.IosSupport.Samples
{
    public class BannerAdWrapper
    {
        public AdTypeBO Parent;
        private BannerView BannerAd;

        public BannerAdWrapper(AdTypeBO par)
        {
            Parent = par;
        }

        public void DestroyAd()
        {
            if (BannerAd != null)
            {
                UnBindAdEvents();
                BannerAd.Destroy();
                BannerAd = null;
            }
            
        }
        public void CallToLoadAd()
        {
            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            BannerAd = new BannerView(Parent.getAdUnitID(), adaptiveSize, AdPosition.Bottom);

            BannerAd.LoadAd(GoogleAdMobController.Instance.CreateAdRequest(GoogleAdMobController.Instance.GetGDPRValue()));

            if (AdsController.Instance.NoShowBannerAdArea())
            {
                HideAd();
            }

            //BannerAd.LoadAd(GoogleAdMobController.Instance.CreateAdRequest(GoogleAdMobController.Instance.GetGDPRValue()));
            BindAdEvents();
        }
        /// <summary>
        /// listen to events the banner view may raise.
        /// </summary>
        private void BindAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            BannerAd.OnBannerAdLoaded += OnBannerAdLoaded_fn;
            // Raised when an ad fails to load into the banner view.
            BannerAd.OnBannerAdLoadFailed += OnBannerAdLoadFailed_fn;
            // Raised when the ad is estimated to have earned money.
            BannerAd.OnAdPaid += OnAdPaid_fn;
            // Raised when an impression is recorded for an ad.
            BannerAd.OnAdImpressionRecorded += OnAdImpressionRecorded_fn;
            // Raised when a click is recorded for an ad.
            BannerAd.OnAdClicked += OnAdClicked_fn;
            // Raised when an ad opened full screen content.
            BannerAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened_fn;
            // Raised when the ad closed full screen content.
            BannerAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed_fn;
        }
        private void UnBindAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            BannerAd.OnBannerAdLoaded -= OnBannerAdLoaded_fn;
            // Raised when an ad fails to load into the banner view.
            BannerAd.OnBannerAdLoadFailed -= OnBannerAdLoadFailed_fn;
            // Raised when the ad is estimated to have earned money.
            BannerAd.OnAdPaid -= OnAdPaid_fn;
            // Raised when an impression is recorded for an ad.
            BannerAd.OnAdImpressionRecorded -= OnAdImpressionRecorded_fn;
            // Raised when a click is recorded for an ad.
            BannerAd.OnAdClicked -= OnAdClicked_fn;
            // Raised when an ad opened full screen content.
            BannerAd.OnAdFullScreenContentOpened -= OnAdFullScreenContentOpened_fn;
            // Raised when the ad closed full screen content.
            BannerAd.OnAdFullScreenContentClosed -= OnAdFullScreenContentClosed_fn;
        }
        void OnAdFullScreenContentClosed_fn()
        {
            Debug.Log("Banner view full screen content closed.");
        }
        void OnAdFullScreenContentOpened_fn()
        {
            Debug.Log("Banner view full screen content opened.");
        }
        void OnAdClicked_fn()
        {
            Debug.Log("Banner view was clicked.");
        }
        void OnAdImpressionRecorded_fn()
        {
            Debug.Log("Banner view recorded an impression.");
        }
        void OnAdPaid_fn(AdValue adValue)
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
        }
        void OnBannerAdLoadFailed_fn(LoadAdError error)
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                    + error);
        }
        void OnBannerAdLoaded_fn()
        {
            Debug.Log("Banner view loaded an ad with response : "
                    + BannerAd.GetResponseInfo());
            GoogleAdMobController.Instance.OnAdLoaded(AdTypeBO.adUnitType.MainBanner);
        }
        public void ShowAd()
        {
            if (IsAdLoaded())
            {
                GA_Controller.Instance.LogDesignEvent("Ads:BannerShow");
                GA_Controller.Instance.LogDesignEvent("Ads:Banner:Show");
                if (!AdsController.Instance.NoShowBannerAdArea())
                {
                    BannerAd.Show();
                }
              
            }
            else
            {
                GA_Controller.Instance.LogDesignEvent("Ads:BannerNotLoadedToShow");
                GA_Controller.Instance.LogDesignEvent("Ads:Banner:NotLoaded");
            }
        }

        public void HideAd()
        {
            if (IsAdLoaded())
            {
                BannerAd.Hide();
            }
            else
            {

            }
        }

        public void RequestAd()
        {
            //if (AdsController.Instance.NoShowBannerAdArea())
            //{
            //    return;
            //}

            //if (IsAdLoaded())
                //{
                //    AdsController.Instance.ResetInterstetialAdLoadingVariable();
                //    return;
                //}


                // Clean up interstitial before using it
                DestroyAd();
            CallToLoadAd();
            GA_Controller.Instance.LogDesignEvent("Ads:LoadFailedBanner");
            GA_Controller.Instance.LogDesignEvent("Ads:Banner:Request");
            Parent.ResetLoadTries();
            Parent.ResetAdLoadingVariable();
        }
       public bool IsAdLoaded()
        {
            if (BannerAd == null /*|| !ad.CanShowAd()*/)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
