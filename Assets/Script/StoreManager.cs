using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Advertisement.IosSupport.Samples;

public class StoreManager : MonoBehaviour
{
    [Header("Ad Reward Data")]
    public List<AdsForCoinsBO> AdsForCoinsData;
    private int currentAdIndex = 0;
    private float remainingTime;
    private bool isWaiting;

    [Header("UI References")]
    public GameObject storePanel;
    public TMP_Text coinsText;
    public TMP_Text timerText;
    public Button watchAdButton;
    public Button removeAdsButton;
    public GameObject watchAd;
    public GameObject timer;
    public UIHandler _handler;

    private void OnEnable()
    {
        AdsForCoinsController.Instance.BindAdWatchedListeners(HandleAdReward);
        AdsForCoinsController.Instance.BindTimerListeners(()=> { });
    }

    private void OnDisable()
    {
        AdsForCoinsController.Instance.UnbindAdWatchedListeners(HandleAdReward);
        AdsForCoinsController.Instance.UnbindTimerListeners(() => { });

    }

    private void Start()
    {
        watchAdButton.onClick.AddListener(OnWatchAdClicked);
        UpdateCoinsUI();
    }

    private void OnWatchAdClicked()
    {
        if (isWaiting) return;

        PlayClickSound();
        AdsController.Instance.SelectedRewardedAd = AdsController.SelectedRewardedAdType.IncCoin;
        AdsController.Instance.ShowAd(AdTypeBO.adUnitType.LevelEndRewardedVideo);
    }

    private void HandleAdReward()
    {
        if (currentAdIndex >= AdsForCoinsData.Count) return;

        int reward = AdsForCoinsData[currentAdIndex].CoinsAsReward;
        AddCoins(reward);
        Debug.Log($"Player rewarded with {reward} coins.");

        if (currentAdIndex < AdsForCoinsData.Count - 1)
            currentAdIndex++;

        remainingTime = AdsForCoinsData[currentAdIndex].WaitToAppear;

        UpdateCoinsUI();

        if (remainingTime > 0)
            StartCoroutine(StartCooldownTimer());
        else
            ToggleAdState(true);
    }

    private IEnumerator StartCooldownTimer()
    {
        isWaiting = true;
        ToggleAdState(false);

        while (remainingTime > 0)
        {
            timerText.text = $"{Mathf.CeilToInt(remainingTime)}s";
            yield return new WaitForSecondsRealtime(1f);
            remainingTime -= 1f;
        }

        ToggleAdState(true);
        timerText.text = "";
        isWaiting = false;
    }

    private void ToggleAdState(bool showAd)
    {
        watchAd.SetActive(showAd);
        timer.SetActive(!showAd);
    }

    private void AddCoins(int amount)
    {
        int current = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins, 0);
        current += amount;
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.Coins, current);
    }

    private void UpdateCoinsUI()
    {
        int coins = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins, 0);
        coinsText.text = coins.ToString();
        _handler?.PlayCoinsAnimation(coinsText.text);
    }

    public void CloseStorePanel()
    {
        PlayClickSound();
        storePanel.SetActive(false);
    }

    private void PlayClickSound()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
    }
}
