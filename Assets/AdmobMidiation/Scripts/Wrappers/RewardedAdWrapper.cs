using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
namespace Unity.Advertisement.IosSupport.Samples
{
    public class RewardedAdWrapper
    {
        public AdTypeBO Parent;
        private RewardedAd RewardAd;

        public RewardedAdWrapper(AdTypeBO par)
        {
            Parent = par;
        }


        public void DestroyAd()
        {
            if (RewardAd != null)
            {
                RewardAd.Destroy();
                RewardAd = null;
            }
            
        }
        public void ShowAd()
        {
            if (IsAdLoaded())
            {
                GA_Controller.Instance.LogDesignEvent("Ads:RewardedShow");
                GA_Controller.Instance.LogDesignEvent("Ads:Rewarded:Show");
                AdsController.Instance.ResetLevelLastPlayedCount();
                AdsController.Instance.RegisterAdBeingPlayed();
                RewardAd.Show(HandleLevelEndRewardedVideoRewarded);
            }
            else
            {
                GA_Controller.Instance.LogDesignEvent("Ads:RewardedNotLoadedToShow");
                GA_Controller.Instance.LogDesignEvent("Ads:Rewarded:NotLoaded");
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
            DestroyAd();
            GA_Controller.Instance.LogDesignEvent("Ads:LoadRewarded");
            GA_Controller.Instance.LogDesignEvent("Ads:Rewarded:Request");
            CallToLoadAd();
        }
        public bool IsAdLoaded()
        {
            if (RewardAd == null || !RewardAd.CanShowAd())
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
            RewardedAd.Load(Parent.getAdUnitID(), GoogleAdMobController.Instance.CreateAdRequest(GoogleAdMobController.Instance.GetGDPRValue()), RewardedAdLoadCallBack);
        }
        void AdFailedToLoad(string Message)
        {
            GA_Controller.Instance.LogDesignEvent("Ads:LoadFailedRewarded");
            GA_Controller.Instance.LogDesignEvent("Ads:Rewarded:RequestFailed");
            Debug.LogError("rewarded Ad Request Failed: " + Message);
        }
        void RewardedAdLoadCallBack(RewardedAd ad, LoadAdError error)
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
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
                Debug.Log("Rewarded ad failed to load an ad " +
                               "with error : " + error);
                Parent.ResetAdLoadingVariable();
                AdsController.Instance.RequestAd(Parent.Type, true);
                return;
            }

            // Debug.Log("Rewarded ad loaded with response : "
            //           + ad.GetResponseInfo());
            RewardAd = ad;

            Parent.ResetLoadTries();
            RegisterRewardedAdEventHandlers(RewardAd);
            Parent.ResetAdLoadingVariable();
            GA_Controller.Instance.LogDesignEvent("Ads:LoadDoneRewarded");
            GA_Controller.Instance.LogDesignEvent("Ads:Rewarded:RequestCompleted");
        }

        #region EVENTS
        void RegisterRewardedAdEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += OnRewardedAdPaid_fn;
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += OnRewardedAdImpressionRecorded_fn;
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += OnRewardedAdClicked_fn;
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += OnRewardedAdFullScreenContentOpened_fn;
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed_fn;
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed_fn;
        }
        // Raised when the ad is estimated to have earned money.
        void OnRewardedAdPaid_fn(AdValue adValue)
        {

            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));

        }
        // Raised when an impression is recorded for an ad.
        void OnRewardedAdImpressionRecorded_fn()
        {
            Debug.Log("Rewarded ad recorded an impression.");
        }
        // Raised when a click is recorded for an ad.
        void OnRewardedAdClicked_fn()
        {
            Debug.Log("Rewarded ad was clicked.");
        }
        // Raised when an ad opened full screen content.
        void OnRewardedAdFullScreenContentOpened_fn()
        {
            Debug.Log("Rewarded ad full screen content opened.");
        }
        // Raised when the ad closed full screen content.
        void OnAdFullScreenContentClosed_fn()
        {
            Debug.Log("Rewarded ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            AdsController.Instance.RequestAd(Parent.Type);
        }
        // Raised when the ad failed to open full screen content.
        void OnAdFullScreenContentFailed_fn(AdError error)
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            AdsController.Instance.RequestAd(Parent.Type);
        }

        public void HandleLevelEndRewardedVideoRewarded(Reward args)
        {

            //string type = args.Type;
            double amount = args.Amount;
            //MonoBehaviour.print("AM: RV Rewared"
            //+ amount.ToString() + " " + type);

            ///////////Give Reward
            Parent.RewardGiven();

        }
        #endregion
    }
}
