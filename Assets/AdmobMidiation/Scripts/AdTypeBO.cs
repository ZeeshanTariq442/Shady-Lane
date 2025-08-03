using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
namespace Unity.Advertisement.IosSupport.Samples
{
    [Serializable]
    public class AdTypeBO
    {
        public enum adUnitType : int { none = 0, LevelEndInterstetial = 1, LevelEndRewardedVideo = 2, MainBanner=3 };
        public adUnitType Type = adUnitType.none;
        public enum AdType : int { none = 0, RewardedAd = 1, InterstetialAd = 2, BannerAd = 3 };
        public AdType Ad_Type = AdTypeBO.AdType.none;
        public string AndroidKey = "";
        public string IOSKey = "";
        public int MaxTriesToLoadAd = 5;
        public float DelayInNextTryToLoad = 5;
        [HideInInspector]
        public int CurrentTriesToLoadAd = 0;

        [HideInInspector]
        public bool LoadingVariable = false;

        public object AdObject = null;
        public Coroutine AdCallCoRoutine = null;

        [Space]
        [Space]
        [Header("Test Keys: Do Not Change this")]
        public string TestAndroidKey = "";
        public string TestIOSKey = "";

        public void Init()
        {
            if (Ad_Type == AdType.InterstetialAd)
            {
                AdObject = new InterstetialAdWrapper(this);
            }
            else if (Ad_Type == AdType.RewardedAd)
            {
                AdObject = new RewardedAdWrapper(this);
            }
            else if (Ad_Type == AdType.BannerAd)
            {
                AdObject = new BannerAdWrapper(this);
            }
            AdCallCoRoutine = null;
            CurrentTriesToLoadAd = 0;
            CancelCoroutineIfAny();
        }
        public void ResetAdLoadingVariable()
        {
            LoadingVariable = false;
        }
        public void SetAdloadingVariable()
        {
            LoadingVariable = true;
        }
        public void ResetLoadTries()
        {
            CurrentTriesToLoadAd = 0;
        }
        public string getAdUnitID()
        {
#if UNITY_EDITOR
            return "unused";

#elif UNITY_ANDROID
            
            if(SDKCommons.Instance.Game_Mode== SDKCommons.GameMode.Production)
            {
                return AndroidKey;
            }
            else
            {
                return TestAndroidKey;
            }

#elif UNITY_IPHONE
            
            if(SDKCommons.Instance.Game_Mode== SDKCommons.GameMode.Production)
            {
                return IOSKey;
            }
            else
            {
                return TestIOSKey;
            }
#else
         return "unexpected_platform";
#endif
        }
        public void OnAdLoaded(){
            if (AdObject != null)
            {
                if (AdObject is InterstetialAdWrapper InterstetialAd)
                {

                }
                else if (AdObject is RewardedAdWrapper RewardedAd)
                {

                }
                else if (AdObject is BannerAdWrapper BannerAd)
                {
                    if(AdsController.Instance.IsOkToLoadShowAd()){
                        AdsController.Instance.ShowAd(AdTypeBO.adUnitType.MainBanner);
                    }
                    
                }
            }
        }

        public void RequestAd()
        {
            if (AdObject != null)
            {
                if (AdObject is InterstetialAdWrapper InterstetialAd)
                {
                    InterstetialAd.RequestAd();
                }
                else if (AdObject is RewardedAdWrapper RewardedAd)
                {
                    RewardedAd.RequestAd();
                }
                else if (AdObject is BannerAdWrapper BannerAd)
                {
                    BannerAd.RequestAd();
                }
            }
        }
        public void ShowAd()
        {
            if (AdObject != null)
            {
                if (AdObject is InterstetialAdWrapper InterstetialAd)
                {
                    InterstetialAd.ShowAd();
                }
                else if (AdObject is RewardedAdWrapper RewardedAd)
                {
                    RewardedAd.ShowAd();
                }
                else if (AdObject is BannerAdWrapper BannerAd)
                {
                    BannerAd.ShowAd();
                }
            }
        }

        public void HideBannerAd()
        {
            if (AdObject != null)
            {
                if (AdObject is BannerAdWrapper BannerAd)
                {
                    BannerAd.HideAd();
                }
            }
        }

        public bool IsAdLoaded()
        {
            if (AdObject != null)
            {
                if (AdObject is InterstetialAdWrapper InterstetialAd)
                {
                    return InterstetialAd.IsAdLoaded();
                }
                else if (AdObject is RewardedAdWrapper RewardedAd)
                {
                    return RewardedAd.IsAdLoaded();
                }
                else if (AdObject is BannerAdWrapper BannerAd)
                {
                    return BannerAd.IsAdLoaded();
                }
            }
            return false;
        }
        public void RewardGiven()
        {
            AdsController.Instance.OnRewardedAdSuccess();
        }

        public void CancelCoroutineIfAny()
        {
            if (AdCallCoRoutine != null)
            {
                AdsController.Instance.StopCoroutine(AdCallCoRoutine);
                AdCallCoRoutine = null;
            }
        }
        bool CheckAdRequestIsGoodIfIAPPurchased(){
            if(Type== adUnitType.LevelEndRewardedVideo){
                return true;
            }
            else{
                if(AdsController.Instance.IsOkToLoadShowAd()){
                    return true;
                }
            }
            return false;
        }
        public bool IsGoodToCallRequestAd()
        {
            if (CurrentTriesToLoadAd < MaxTriesToLoadAd && CheckAdRequestIsGoodIfIAPPurchased())
            {
                return true;
            }
            return false;
        }
    }
}
