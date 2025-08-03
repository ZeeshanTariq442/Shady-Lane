using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Unity.Advertisement.IosSupport.Samples;
using static TimerController;

[Serializable]
public class AdsForCoinsBO
{
    public int CoinsAsReward = 0;
    public float WaitToAppear = 0;

    [HideInInspector]
    public float RemainingTime = 0;

    public bool IsTimerRunning = false;



    public void OnTimerDec(int sec)
    {
        RemainingTime -= sec;
        CheckAndSetStopTimer();
    }
    void CheckAndSetStopTimer()
    {
        if (RemainingTime <= 0)
        {
            IsTimerRunning = false;
            AdsForCoinsController.Instance.OnSelectedBOTimerStopped();
        }
    }
    public void StartTimer()
    {
        IsTimerRunning = true;
        RemainingTime = WaitToAppear;
    }

}

public class AdsForCoinsController : MonoBehaviour
{
    public static AdsForCoinsController Instance;
    public List<AdsForCoinsBO> AdsForCoinsData;

    public AdsForCoinsBO SelectedBO;

    public bool IsDayEndTimerRunning = false;

    int SelectedBOIndex = 0;

    [HideInInspector]
    public UnityEvent OnSecondPassed;
    public UnityEvent OnAdWatched;

    DateTime TimerStartTime;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
       
    }

    private void Start()
    {
        UpdateDataFromPref();
    }

    public void BindTimerListeners(UnityAction callback)
    {
        OnSecondPassed.AddListener(callback);
    }

    public void UnbindTimerListeners(UnityAction callback)
    {
        OnSecondPassed.RemoveListener(callback);
    }
    public void BindAdWatchedListeners(UnityAction callback)
    {
        OnAdWatched.AddListener(callback);
    }

    public void UnbindAdWatchedListeners(UnityAction callback)
    {
        OnAdWatched.RemoveListener(callback);
    }
    void AdConsumedGoToNext()
    {
        for (int i = 0; i < AdsForCoinsData.Count; i++)
        {
            if (AdsForCoinsData[i] == SelectedBO)
            {
                if (i < (AdsForCoinsData.Count - 1))
                {

                    NewBOSelected((i + 1));
                }
                else
                {
                    NothingLeft();
                }
                break;
            }
        }
    }
    void NothingLeft()
    {
        SelectedBO = null;
        TimerStartTime = DateTime.Now.Date;
        IsDayEndTimerRunning = true;

        SelectedBOIndex = 0;

        SaveSelectedBOIndex();
        SaveDayEndTimeState();
        // Bind Timers
          TC.BindEventListeners(hasBind: true, OnTimerTick);

    }
    void NewDayStarted()
    {
          TC.BindEventListeners(hasBind: false, OnTimerTick);
        IsDayEndTimerRunning = false;
        SaveDayEndTimeState();
        NewBOSelected(0);
    }
    void NewBOSelected(int index)
    {
        SelectedBOIndex = index;
        SelectedBO = AdsForCoinsData[index];
        if (SelectedBO.WaitToAppear > 0)
        {
            SelectedBO.StartTimer();

               TC.BindEventListeners(hasBind: true, OnTimerTick);
            // Bind Timer
        }
        SaveSelectedBOIndex();

    }

    public void OnSelectedBOTimerStopped()
    {
        // UnBind Timer
          TC.BindEventListeners(hasBind: false, OnTimerTick);
    }

    public void OnTimerTick(int sec)
    {
        if (IsDayEndTimerRunning)
        {
            if (TimerStartTime.Date != DateTime.Now.Date)
            {
                NewDayStarted();
            }
        }
        else
        {
            if (SelectedBO != null && SelectedBO.IsTimerRunning)
            {
                SelectedBO.OnTimerDec(sec);
            }
        }

        OnSecondPassed?.Invoke();
    }
    public bool IsTimerRunning()
    {
        if (IsDayEndTimerRunning)
        {
            return true;
        }
        else
        {
            if (SelectedBO != null && SelectedBO.IsTimerRunning)
            {
                return true;
            }
        }
        return false;
    }
    public string GetTime(bool secondsMode = true)
    {
        string formattedTime = "00:00:00";
        if (IsDayEndTimerRunning)
        {
            // Get current time
            DateTime now = DateTime.Now;

            // Get end of the day time (23:59:59)
            DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            // Calculate the difference
            TimeSpan difference = endOfDay - now;

            // Format the result as hh:mm:ss
            if (secondsMode)
            {
                formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                              difference.Hours,
                                              difference.Minutes,
                                              difference.Seconds);
            }
            else
            {
                if (difference.TotalHours >= 1)
                {
                    formattedTime = string.Format("{0:D2}:{1:D2}",
                                                  (int)difference.TotalHours,
                                                  difference.Minutes);
                }
                else
                {
                    formattedTime = string.Format("{0:D2}:{1:D2}",
                                                  difference.Minutes,
                                                  difference.Seconds);
                }
            }
            return formattedTime;
        }
        else
        {
            if (SelectedBO != null && SelectedBO.IsTimerRunning)
            {
                TimeSpan time = TimeSpan.FromSeconds(SelectedBO.RemainingTime);

                if (secondsMode)
                {
                    formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                                  time.Hours,
                                                  time.Minutes,
                                                  time.Seconds);
                }
                else
                {
                    if (time.TotalHours >= 1)
                    {
                        formattedTime = string.Format("{0:D2}:{1:D2}",
                                                      (int)time.TotalHours,
                                                      time.Minutes);
                    }
                    else
                    {
                        formattedTime = string.Format("{0:D2}:{1:D2}",
                                                      time.Minutes,
                                                      time.Seconds);
                    }
                }
                return formattedTime;

            }
        }

        return "00:00:00";
    }
    public void OnAdWatchedFn()
    {
        int coinsToAd = SelectedBO.CoinsAsReward;

        AdConsumedGoToNext();
        StorePanel.Instance.OnUpdateCoins(coinsToAd);


        OnAdWatched.Invoke();

    }
    public void PlayRewardedAdForCoins()
    {
        AdsController.Instance.SelectedRewardedAd = AdsController.SelectedRewardedAdType.IncCoin;
        AdsController.Instance.ShowAd(AdTypeBO.adUnitType.LevelEndRewardedVideo);
    }

    #region // Prefs
    string SelectBOIndexKey = "SelectedBOIndex";
    string IsDayEndTimerRunningKey = "IsDayEndTimerRunning";
    string DayEndStartTimerDateKey = "DayEndStartTimerDate";

    void SaveSelectedBOIndex()
    {
        PlayerPrefs.SetInt(SelectBOIndexKey, SelectedBOIndex);
        PlayerPrefs.Save();
    }

    void SaveDayEndTimeState()
    {
        if (IsDayEndTimerRunning)
        {
            PlayerPrefs.SetInt(IsDayEndTimerRunningKey, 1);
            PlayerPrefs.SetString(DayEndStartTimerDateKey, TimerStartTime.Date.ToString());
        }
        else
        {
            PlayerPrefs.SetInt(IsDayEndTimerRunningKey, 0);
            PlayerPrefs.SetString(DayEndStartTimerDateKey, "");
        }
        PlayerPrefs.Save();
    }

    void UpdateDataFromPref()
    {
        SelectedBOIndex = PlayerPrefs.GetInt(SelectBOIndexKey, 0);
        SelectedBO = AdsForCoinsData[SelectedBOIndex];

        string DayEndStartTimerDateStringVal = PlayerPrefs.GetString(DayEndStartTimerDateKey, "");
        if (DayEndStartTimerDateStringVal != "")
        {
            if (!DateTime.TryParse(DayEndStartTimerDateStringVal, out TimerStartTime))
            {
                TimerStartTime = DateTime.Now.Date;
            }
        }
        else
        {
            TimerStartTime = DateTime.Now.Date;
        }


        int IsDayEndTimerRunningIntVal = PlayerPrefs.GetInt(IsDayEndTimerRunningKey, 0);
        if (IsDayEndTimerRunningIntVal == 1)
        {
            IsDayEndTimerRunning = true;
             TC.BindEventListeners(hasBind: true, OnTimerTick);
        }
        else
        {
            IsDayEndTimerRunning = false;
        }

    }


    #endregion
}