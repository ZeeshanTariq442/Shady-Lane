using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorePanel : MonoBehaviour
{
    public Sprite timerSprite, watchAdSprite;
    public Image watchAdImage;
    public Button watchAdButton;
    public TMP_Text coinAndTimerText;
    public Button restorePurchaseButton;
    public TMP_Text coinsText;
    public UIHandler _handler;
    public GameObject storePanel;
    public CountdownTimer _countdownTimer;
    public float watchAdFontSizeOnCoins = 30;
    public float watchAdFontSizeOnTime = 15;
    public static StorePanel Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void OpenPopup()
    {
        ResetData();
        InitData();
        if (!HasTimerMode())
        {
            WatchAdMode(isTimer: false);
        }
        else
        {
            OnTimerElapsed();
            AdsForCoinsController.Instance.BindTimerListeners(OnTimerElapsed);
        }
#if UNITY_IOS
            EnableIOSButtons();
#endif

    }

    public void ClosePopup()
    {
        // CSM.BindEventListeners(hasBind: false, OnUpdateCoins);
        AdsForCoinsController.Instance.UnbindTimerListeners(OnTimerElapsed);
        AdsForCoinsController.Instance.UnbindAdWatchedListeners(OnAdSuccess);

    }
    public void CloseStorePanel()
    {
        _countdownTimer.ResumeTimer();
        PlayClickSound();
        ClosePopup();
        storePanel.SetActive(false);
    }
    public void OpenStorePanel()
    {
        _countdownTimer.StopTimer();
        PlayClickSound();
        OpenPopup();
        storePanel.SetActive(true);
    }
    private void PlayClickSound()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
    }

    private void InitData()
    {
        UpdateCoinsUI(false);
    }

    private void OnTimerElapsed()
    {
        if (HasTimerMode())
        {
            WatchAdMode(isTimer: true);
            UpdateTimerText();
        }
        else
        {
            WatchAdMode(isTimer: false);
        }
    }

    private void WatchAdMode(bool isTimer)
    {
        if (isTimer)
        {
            watchAdButton.interactable = false;
            if (watchAdImage != null)
            {
                watchAdImage.sprite = timerSprite;
            }
            coinAndTimerText.fontSize = watchAdFontSizeOnTime;
            coinAndTimerText.color = Color.red;
        }
        else
        {
            if (watchAdImage != null)
            {
                watchAdImage.sprite = watchAdSprite;
            }
            coinAndTimerText.fontSize = watchAdFontSizeOnCoins;
            coinAndTimerText.text = $"+{AdsForCoinsController.Instance.SelectedBO.CoinsAsReward}";
            coinAndTimerText.color = Color.white;
            AdsForCoinsController.Instance.UnbindTimerListeners(OnTimerElapsed);
            watchAdButton.interactable = true;
        }
    }

    public void OnWatchAdButtonPress()
    {
        AdsForCoinsController.Instance.BindAdWatchedListeners(OnAdSuccess);
        AdsForCoinsController.Instance.PlayRewardedAdForCoins();
    }

    public void OnRemoveAdsButtonPress()
    {

    }

    public void OnRestorePurchaseButtonPress()
    {

    }

    private void EnableIOSButtons()
    {
        restorePurchaseButton.gameObject.SetActive(true);
    }

    public void ResetData()
    {
        restorePurchaseButton.gameObject.SetActive(false);
    }

    private void UpdateTimerText()
    {
        if (coinAndTimerText == null) return;

        coinAndTimerText.text = AdsForCoinsController.Instance.GetTime(false);
    }
    public void SetButtonInteractable(bool load)
    {
        if(load)
        watchAdButton.interactable = !HasTimerMode();
        else
            watchAdButton.interactable = false;
    }
    public bool HasTimerMode()
    {
        return AdsForCoinsController.Instance.IsTimerRunning();
    }

    private void OnAdSuccess()
    {
        AdsForCoinsController.Instance.UnbindAdWatchedListeners(OnAdSuccess);
    }
    private void UpdateCoinsUI(bool playAnimation)
    {
        int coins = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins, 0);
        coinsText.text = coins.ToString();
        if(playAnimation)
        _handler?.PlayCoinsAnimation(coinsText.text);
    }
    private void AddCoins(int amount)
    {
        int current = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins, 0);
        current += amount;
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.Coins, current);
        UpdateCoinsUI(true);
    }
    public void OnUpdateCoins(int coins)
    {
        AddCoins(coins);
        if (!HasTimerMode())
        {
            WatchAdMode(isTimer: false);
        }
        else
        {
            OnTimerElapsed();
            AdsForCoinsController.Instance.BindTimerListeners(OnTimerElapsed);
        }
    }


    //public void CheckedAdPurchased()
    //{
    //    if (AdsController.Instance.IsOkToLoadShowAd())
    //    {
    //        RemoveAdButton.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        RemoveAdButton.gameObject.SetActive(false);
    //    }
    //}
}
