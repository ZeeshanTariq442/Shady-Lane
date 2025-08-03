using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
namespace Unity.Advertisement.IosSupport.Samples
{
    public class InterstetialAdWrapper
    {
        public AdTypeBO Parent;
        private InterstitialAd IntAd;

        public InterstetialAdWrapper(AdTypeBO par)
        {
            Parent = par;
        }


        public void DestroyAd()
        {
            if (IntAd != null)
            {
                IntAd.Destroy();
                IntAd = null;
            }
        }
        public void ShowAd()
        {
            
            if (IsAdLoaded())
            {
                GA_Controller.Instance.LogDesignEvent("Ads:InterstetialShow");
                GA_Controller.Instance.LogDesignEvent("Ads:Interstetial:Show");
                AdsController.Instance.ResetLevelLastPlayedCount();
                AdsController.Instance.RegisterAdBeingPlayed();
                IntAd.Show();
            }
            else
            {
                GA_Controller.Instance.LogDesignEvent("Ads:InterstetialNotLoadedToShow");
                GA_Controller.Instance.LogDesignEvent("Ads:Interstetial:NotLoaded");
            }
        }
        public void RequestAd()
        {
            if (IsAdLoaded())
            {
                Parent.ResetAdLoadingVariable();
                return;
            }


            // Clean up interstitial before using it

            GA_Controller.Instance.LogDesignEvent("Ads:LoadInterstetial");
            GA_Controller.Instance.LogDesignEvent("Ads:Interstetial:Request");
            DestroyAd();
            CallToLoadAd();
        }
        public bool IsAdLoaded()
        {
            if (IntAd == null || !IntAd.CanShowAd())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void CallToLoadAd()
        {
            Parent.CurrentTriesToLoadAd++;
            InterstitialAd.Load(Parent.getAdUnitID(), GoogleAdMobController.Instance.CreateAdRequest(GoogleAdMobController.Instance.GetGDPRValue()), InterstetialAdLoadCallBack);
        }
        void AdFailedToLoad(string Message)
        {
            GA_Controller.Instance.LogDesignEvent("Ads:LoadFailedInterstetial");
            GA_Controller.Instance.LogDesignEvent("Ads:Interstetial:RequestFailed");
            Debug.LogError("Interstetial Ad Request Failed: " + Message);
        }

        void InterstetialAdLoadCallBack(InterstitialAd ad, LoadAdError error)
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.LogError("interstitial ad failed to load an ad " +
                               "with error : " + error);

                if (error != null)
                {
                    AdFailedToLoad(error.GetMessage());
                }
                else
                {
                    if (ad == null)
                    {
                        AdFailedToLoad("Ad is Null");
                    }
                    else
                    {
                        AdFailedToLoad("Unknown");
                    }
                }
                if (ad != null)
                {
                    ad.Destroy();
                }
                Parent.ResetAdLoadingVariable();
                return;
            }
            Debug.Log("Interstitial ad loaded with response : "
                      + ad.GetResponseInfo());

            IntAd = ad;
            GA_Controller.Instance.LogDesignEvent("Ads:LoadDoneInterstetial");
            GA_Controller.Instance.LogDesignEvent("Ads:Interstetial:RequestCompleted");
            Parent.ResetLoadTries();
            RegisterInterstetialEventHandlers(IntAd);
            Parent.ResetAdLoadingVariable();
        }

        private void RegisterInterstetialEventHandlers(InterstitialAd interstitialAd)
        {
            // Raised when the ad is estimated to have earned money.
            interstitialAd.OnAdPaid += OnAdPaid_fn;
            // Raised when an impression is recorded for an ad.
            interstitialAd.OnAdImpressionRecorded += OnAdImpressionRecorded_fn;
            // Raised when a click is recorded for an ad.
            interstitialAd.OnAdClicked += OnAdClicked_fn;
            // Raised when an ad opened full screen content.
            interstitialAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened_fn;
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
        }

        #region EVENTS

        // Raised when the ad is estimated to have earned money.
        void OnAdPaid_fn(AdValue adValue)
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                          adValue.Value,
                          adValue.CurrencyCode));
        }
        // Raised when an impression is recorded for an ad.
        void OnAdImpressionRecorded_fn()
        {
            Debug.Log("Interstitial ad recorded an impression.");
        }
        // Raised when a click is recorded for an ad.
        void OnAdClicked_fn()
        {
            Debug.Log("Interstitial ad was clicked.");
        }
        // Raised when an ad opened full screen content.
        void OnAdFullScreenContentOpened_fn()
        {
            Debug.Log("Interstitial ad full screen content opened.");
        }
        // Raised when the ad closed full screen content.
        void OnAdFullScreenContentClosed()
        {
            AdsController.Instance.RequestAd(Parent.Type);
            AdsController.Instance.OnInterstetialFinished();
        }
        // Raised when the ad failed to open full screen content.
        void OnAdFullScreenContentFailed(AdError error)
        {
            // Reload the ad so that we can show another as soon as possible.
            AdsController.Instance.RequestAd(Parent.Type);
            AdsController.Instance.OnInterstetialFinished();
        }
        #endregion
    }
}
