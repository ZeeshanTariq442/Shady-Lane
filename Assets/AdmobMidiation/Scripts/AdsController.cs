using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Advertisement.IosSupport.Samples;
using UnityEngine.SceneManagement;

[Serializable]
public class AdsStrategyBO
{
    public int StartingLevel = 0;
    public int EndLevel = 0;
    public int NoOfSkips = 3;
}

public class AdsController : MonoBehaviour
{
    public static AdsController Instance;

    public string AdSdkName = "admob";


    public List<AdsStrategyBO> AdsStrategy;
    public GDPRUIController GDPRUIPanel;

    public int LevelsSinceLastAdPlayed = 0;
    public int DelayAfterAdPlayedForApplicationPause = 10;
    public int PauseIgnoreTime = 3;
    public int MinimumLevelForPauseCommercialBreak = 5;

    bool IsAdPlaying = false;
    bool CanLevelResetData = true;
    DateTime PauseStartTime;

    public bool CanClick = true;
    public float ClickResetTime = 1;

    public enum SelectedRewardedAdType : int { none = 0, HintBooster = 1, IncCoin = 2, LifeBooster = 3 };

    public SelectedRewardedAdType SelectedRewardedAd = SelectedRewardedAdType.none;


    Action onCallBack;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    void Start()
    {
        GetAndSetNoStatus();
    }

    public void OnButtonClick()
    {
        CanClick = false;
        if (IsInvoking(nameof(ResetClick)))
        {
            CancelInvoke(nameof(ResetClick));
        }
        Invoke(nameof(ResetClick), ClickResetTime);

    }
    private void ResetClick()
    {
        CanClick = true;
    }

    public void PlayButtonClickSound()
    {
        // SfxController.SFxInstance.PlaySound(SFxType.Button);
    }
    public void OnLevelStarted()
    {
        LevelsSinceLastAdPlayed++;
    }
    public void ResetLevelLastPlayedCount()
    {
        if (CanLevelResetData)
        {
            LevelsSinceLastAdPlayed = 0;
        }
    }
    public void RegisterAdBeingPlayed()
    {
        IsAdPlaying = true;
        if (IsInvoking(nameof(ResetAdPlayedVariable)))
        {
            CancelInvoke(nameof(ResetAdPlayedVariable));
        }
        Invoke(nameof(ResetAdPlayedVariable), DelayAfterAdPlayedForApplicationPause);
    }
    void ResetAdPlayedVariable()
    {
        IsAdPlaying = false;
    }

    int GetCurrentLevel()
    {
        return 0;
    }
    bool IsPlayerInTheGameplay()
    {
        return false;
    }
    bool IsLevelCountValidToShowAdAccordingToStrategy()
    {
        int CurrentLevel = GetCurrentLevel();
        for (int i = 0; i < AdsStrategy.Count; i++)
        {
            if (CurrentLevel >= AdsStrategy[i].StartingLevel && CurrentLevel <= AdsStrategy[i].EndLevel)
            {
                if (LevelsSinceLastAdPlayed > AdsStrategy[i].NoOfSkips)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void OnApplicationPause(bool pause)
    {
        if (GA_Controller.Instance.IsSDKInitialized())
        {
            GA_Controller.Instance.LogDesignEvent("AdsController:ApplicationPause");
        }
        if (pause)
        {
            if (GA_Controller.Instance.IsSDKInitialized())
            {
                GA_Controller.Instance.LogDesignEvent("AdsController:ApplicationPause:" + pause);
            }
            PauseStartTime = DateTime.Now;
        }
        else
        {
            if (GA_Controller.Instance.IsSDKInitialized())
            {
                GA_Controller.Instance.LogDesignEvent("AdsController:ApplicationPause:" + pause);
            }
            if (!IsAdPlaying && (DateTime.Now - PauseStartTime).Seconds > PauseIgnoreTime && IsPlayerInTheGameplay()
            && GetCurrentLevel() >= MinimumLevelForPauseCommercialBreak && IsOkToLoadShowAd())
            {
                if (GA_Controller.Instance.IsSDKInitialized())
                {
                    GA_Controller.Instance.LogDesignEvent("AdsController:Ad:Show:Interstetial:AppPause");
                }
                CanLevelResetData = false;
                GoogleAdMobController.Instance.ShowAdV2(AdTypeBO.adUnitType.LevelEndInterstetial);
            }
        }
    }
    public void RequestAd(AdTypeBO.adUnitType Type, bool LoadWithDelay = false)
    {
        if (GoogleAdMobController.Instance.IsGoodToCallRequestAd(Type))
        {
            if (LoadWithDelay)
            {
                AdTypeBO AdData = GoogleAdMobController.Instance.GetAdData(Type);
                if (AdData != null)
                {
                    if (AdData.AdCallCoRoutine == null)
                    {
                        AdData.AdCallCoRoutine = AdsController.Instance.StartCoroutine(LoadRequestWithDelay(AdData));
                    }

                }
                else
                {
                    RequestAd_Local(Type);
                }
            }
            else
            {
                RequestAd_Local(Type);
            }
        }
        else
        {
            // GA: Max Tries Reached
        }

    }
    IEnumerator LoadRequestWithDelay(AdTypeBO AdData)
    {
        yield return new WaitForSeconds(AdData.DelayInNextTryToLoad);

        RequestAd_Local(AdData.Type);

        AdData.CancelCoroutineIfAny();
    }
    void RequestAd_Local(AdTypeBO.adUnitType Type)
    {
        if (!GoogleAdMobController.Instance.GetAdLoadingVariable(Type))
        {
            GoogleAdMobController.Instance.SetAdLoadingVariable(Type);
            StartCoroutine(InternetConnectionChecker.Instance.CheckInternetConnection(isConnected =>
            {
                if (isConnected)
                {
                    if (!IsAdLoaded(Type))
                    {
                        GoogleAdMobController.Instance.RequestAdV2(Type);
                    }
                    else
                    {
                        GoogleAdMobController.Instance.ReSetAdLoadingVariable(Type);
                    }

                }
                else
                {
                    GoogleAdMobController.Instance.ReSetAdLoadingVariable(Type);
                }
            }));
        }
        else
        {
            //Ga Log
        }
    }

    public void ShowAd(AdTypeBO.adUnitType Type, Action onCallBack)
    {

        if (IsAdLoaded(Type))
        {
            if (Type == AdTypeBO.adUnitType.LevelEndInterstetial)
            {
                if ((IsLevelCountValidToShowAdAccordingToStrategy() || SDKCommons.Instance.IsSdkTestingEnvironment) && IsOkToLoadShowAd())
                {
                    CanLevelResetData = true;
                    GoogleAdMobController.Instance.ShowAdV2(Type);
                }
            }
            else
            {
                CanLevelResetData = true;
                GoogleAdMobController.Instance.ShowAdV2(Type);
            }
            this.onCallBack = onCallBack;
        }
        else
        {
            Debug.Log("Not Load");
        }

    }

    public void ShowAd(AdTypeBO.adUnitType Type)
    {
        if (IsAdLoaded(Type))
        {
            if (Type == AdTypeBO.adUnitType.LevelEndInterstetial)
            {
                if ((IsLevelCountValidToShowAdAccordingToStrategy() || SDKCommons.Instance.IsSdkTestingEnvironment) && IsOkToLoadShowAd())
                {
                    CanLevelResetData = true;
                    GoogleAdMobController.Instance.ShowAdV2(Type);
                }
            }
            else
            {
                CanLevelResetData = true;
                GoogleAdMobController.Instance.ShowAdV2(Type);
            }
        }
        else
        {
           
        }

    }
    public bool IsAdLoaded(AdTypeBO.adUnitType Type)
    {
        return GoogleAdMobController.Instance.IsAdLoaded(Type);
    }

    public void HideBannerAd()
    {
        var Type = AdTypeBO.adUnitType.MainBanner;
        if (IsAdLoaded(Type))
        {
            GoogleAdMobController.Instance.HideBannerAd();
        }
        else
        {

        }
    }
    public void ShowBanner()
    {
        RequestAd(AdTypeBO.adUnitType.MainBanner);
    }
    public void OnRewardedAdSuccess()
    {
        if (SelectedRewardedAd == SelectedRewardedAdType.IncCoin)
        {
            AdsForCoinsController.Instance.OnAdWatchedFn();
        }
        else if (SelectedRewardedAd == SelectedRewardedAdType.none)
        {
            if (onCallBack != null)
            {
                onCallBack?.Invoke();
                onCallBack = null;
            }
        }

        Debug.Log("On Call Back");
    }
    public void OnInterstetialFinished()
    {
        if (onCallBack != null)
        {
            onCallBack?.Invoke();
        }
    }
    public bool NoShowBannerAdArea()
    {
        return SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3;
        //  return PanelType.Loading == GameController.instance.currentPanelType;
    }

    #region 
    public bool IsNoAdPurchased = false;
    string NOAdsStatus = "NOAdsStatusInt";
    public void OnNoAdIAPPurchased()
    {
        SaveNoAdIAPPurchaseStatus(true);
        UpdateIsNoAdPurchasedStatus(true);
    }
    void SaveNoAdIAPPurchaseStatus(bool status)
    {
        if (status)
        {
            PlayerPrefs.SetInt(NOAdsStatus, 1);
        }
        else
        {
            PlayerPrefs.SetInt(NOAdsStatus, 0);
        }
    }
    void UpdateIsNoAdPurchasedStatus(bool status)
    {
        IsNoAdPurchased = status;
    }
    void GetAndSetNoStatus()
    {
        UpdateIsNoAdPurchasedStatus(GetNoAdIAPPurchaseStatusFromPref());
    }
    bool GetNoAdIAPPurchaseStatusFromPref()
    {
        int RetValue = PlayerPrefs.GetInt(NOAdsStatus, 0);
        if (RetValue == 1)
        {
            return true;
        }
        return false;
    }
    public bool IsOkToLoadShowAd()
    {
        return !IsNoAdPurchased;
    }

    #endregion


}
