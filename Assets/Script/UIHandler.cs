using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using Unity.Advertisement.IosSupport.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIHandler : MonoBehaviour
{
    #region Fields
    [Header("Win Panel Setting")]
    public Button nextButton;
    public Button homeButton;
    public Button restartButton;
    public Button rewardButtonOnWin;
    public GameObject ChapterCompletePanel;

    [Header("Failed Panel Setting")]
    public GameObject failedPanel;
    public Button homeButton2;
    public Button restartButton2;
    public Button rewardButtonOnFailed;

    [Header("Setting Panel")]
    public Button setting_home;
    public Button setting_restart;

    [Header("Booster")]
    public Text lifeBoosterText;
    public GameObject lifeBooster;
    public GameObject lifeHeart;
    public GameObject hintBooster;
    public Button hintBoosterButton;

    [Header("Coins")]
    public TMP_Text gamePlayCoins;
    public Text winPanelCoins;


    [Header("Hint Booster Blinking Setting")]
    public Color blinkColor = Color.yellow;
    public float blinkDuration = 0.5f;
    public bool isBlinking = false;
    private Image blinkSprite;

    [Header("Items Counter")]
    public Text iCounter;

    [Header("BG")]
    public Transform bg;
    bool playHeartAnimation;

    bool IsAdAvailable;

    [Header("Hint Booster References")]
    [SerializeField] private GameObject hintBoosterHasBoosterUI;    // hintBooster -> Child(1) -> Child(1)
    [SerializeField] private GameObject hintBoosterNoBoosterUI;     // hintBooster -> Child(1) -> Child(0)
    [SerializeField] private TextMeshProUGUI boosterCountText;      // inside hasBoosterUI -> Text
    [SerializeField] private GameObject adIcon;                     // noBoosterUI -> Child(1)
    [SerializeField] private GameObject coinIcon;                   // noBoosterUI -> Child(0)

    #endregion


    private void Awake()
    {
        blinkSprite = hintBooster.GetComponent<Image>();
        AjustBG();
    }
    private void Start()
    {

        OnLevelStartSetThingsForAds();

        LifeBooster();
        HintBooster();
        
        nextButton.onClick.AddListener(() => CallInterstetialAd(ShowNextChapterItems));
        homeButton.onClick.AddListener(() => GoToScene());
        homeButton2.onClick.AddListener(() => GoToScene());
        rewardButtonOnWin.onClick.AddListener(() => UpdateLifeBooster(true));


        restartButton2.onClick.AddListener(() => RestartGame());
        restartButton.onClick.AddListener(() => RestartChapter());
        rewardButtonOnFailed.onClick.AddListener(() => UpdateLifeBooster(true));

        //Setting
        setting_home.onClick.AddListener(() => CallInterstetialAd(GoToScene));
        setting_restart.onClick.AddListener(() => CallInterstetialAd(RestartGame));

        hintBoosterButton.onClick.AddListener(() => UsedHintBooster());
        SetCoins(PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins).ToString());

       
    }
    private void OnLevelStartSetThingsForAds()
    {
        //Debug.LogError("Level Started");
        AdsController.Instance.OnLevelStarted();
        AdsController.Instance.RequestAd(AdTypeBO.adUnitType.LevelEndInterstetial);
    }
    private void Update()
    {
           
    }
    private void CallInterstetialAd(Action onAdFinished)
    {
          var adType = AdTypeBO.adUnitType.LevelEndInterstetial;

          if (!AdsController.Instance.IsAdLoaded(adType))
          {
              Debug.LogWarning("Interstitial ad not loaded — skipping ad.");
              onAdFinished?.Invoke();
              onAdFinished = null;
              return;
          }

          AdsController.Instance.SelectedRewardedAd = AdsController.SelectedRewardedAdType.none;

          AdsController.Instance.ShowAd(adType, () =>
          {
              onAdFinished?.Invoke();
              onAdFinished = null;
          });
    }

    #region BOOSTER
    public void BuyHintBooster()
    {
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.Coins, PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins) - 5);
        SetCoins(PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins).ToString());
        UpdateHintBooster();

    }
    public void UpdateHintBooster()
    {
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.HintBooster, PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintBooster) + 1);
        HintBooster();
    }
    private void UsedHintBooster()
    {
        ButtonSound();

        if (isBlinking)
        {
            MyEventBus.RaiseEvent(GameEventType.CancelHintBooster);
            StopBlinking();
            return;
        }

        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintBooster) <= 0)
        {
            if (GetCoinCount() >= 5)
                BuyHintBooster();
            else
            {
                IsAdAvailable = AdsController.Instance.IsAdLoaded(AdTypeBO.adUnitType.LevelEndRewardedVideo);
                if (!IsAdAvailable) return;
                AdsController.Instance.SelectedRewardedAd = AdsController.SelectedRewardedAdType.none;
                AdsController.Instance.ShowAd(AdTypeBO.adUnitType.LevelEndRewardedVideo);
                Invoke(nameof(UpdateHintBooster),1);
            }

            return;
        }   

        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintBooster) > 0)
        {
            StartBlinking();
            MyEventBus.RaiseEvent(GameEventType.UsedHintBooster);
        }

    }
    private void HintBooster()
    {
        int boosterCount = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintBooster);

        if (boosterCount > 0)
        {
            hintBoosterHasBoosterUI.SetActive(true);
            hintBoosterNoBoosterUI.SetActive(false);
            boosterCountText.text = boosterCount.ToString();
        }
        else
        {
            hintBoosterHasBoosterUI.SetActive(false);
            hintBoosterNoBoosterUI.SetActive(true);

            bool canAfford = GetCoinCount() >= 5;
            adIcon.SetActive(!canAfford);
            coinIcon.SetActive(canAfford);

            if (GameManager.Instance != null)
                GameManager.Instance.usedSlotBooster = false;
        }

        StopBlinking();
    }


    public void UpdateLifeBooster(bool callAd)
    {
        if (callAd)
        {
            IsAdAvailable = AdsController.Instance.IsAdLoaded(AdTypeBO.adUnitType.LevelEndRewardedVideo);
            if (!IsAdAvailable) return;
            AdsController.Instance.SelectedRewardedAd = AdsController.SelectedRewardedAdType.none;
            AdsController.Instance.ShowAd(AdTypeBO.adUnitType.LevelEndRewardedVideo, () =>
            {
                CallRefillLifeEvent();
                LifeBooster();
            });
        }
        else
        {
            CallRefillLifeEvent();
            LifeBooster();
        }
    }
    private void CallRefillLifeEvent()
    {
        MyEventBus.RaiseEvent(GameEventType.RefillLife);
    }
    private void LifeBooster()
    {

        if (playHeartAnimation)
        {
            int trial = GameManager.Instance.LoadTrials(GameManager.Instance.currentActiveLevel).remainingTrials;
            lifeHeart.GetComponent<DOTweenAnimation>().DORestartById("heart");
            lifeHeart.GetComponent<DOTweenAnimation>().onComplete.AddListener(() =>
            {
                if (trial >= 0)
                    lifeBoosterText.text = trial.ToString();
            });
        }
        else
        {
            playHeartAnimation = true;
        }

        if (GameManager.Instance != null)
        {
            // GameManager.Instance.showWrongTurn = false;
        }

    }
    private void CancelBooster()
    {
        //GameManager.Instance.showWrongTurn = false;
        GameManager.Instance.usedSlotBooster = false;
        StopBlinking();
    }
    #endregion

    #region EVENT SUBSCRIBE & UNSUBSCRIBE
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.UpdateHintBooster, HintBooster);
        MyEventBus.SubscribeEvent(GameEventType.CancelHintBooster, CancelBooster);
        MyEventBus.SubscribeEvent(GameEventType.TaskComplete, CancelBooster);
        MyEventBus.SubscribeEvent(GameEventType.UpdateLifeBooster, LifeBooster);
        SoundManager.instance?.PlayMusic(SoundManager.instance.music);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.UpdateHintBooster, HintBooster);
        MyEventBus.UnSubscribeEvent(GameEventType.CancelHintBooster, CancelBooster);
        MyEventBus.UnSubscribeEvent(GameEventType.TaskComplete, CancelBooster);
        MyEventBus.UnSubscribeEvent(GameEventType.UpdateLifeBooster, LifeBooster);
        SoundManager.instance?.StopMusic();
    }
    #endregion

    public void OnAllLevelComplete()
    {
        nextButton.gameObject.SetActive(false);
        restartButton.transform.position = nextButton.transform.position;
    }
    public void SetItemCounter(int remaining)
    {
        iCounter.text = remaining.ToString();
    }
    public int SetItemCounter()
    {
        return int.Parse(iCounter.text);
    }
    private void AjustBG()
    {
        if (Screen.width < 1500 && Screen.width > 1250)
        {
            bg.localScale = new Vector2(bg.localScale.x + 0.05f, bg.localScale.y);
        }
        else if (Screen.width > 1500)
        {
            bg.localScale = new Vector2(bg.localScale.x + 0.15f, bg.localScale.y);
        }
    }
    private void RestartChapter()
    {
        GameManager.Instance?.ClearTrials();
        ChapterCompletePanel.SetActive(false);
        MyEventBus.RaiseEvent(GameEventType.RestartChapter);
        MyEventBus.RaiseEvent(GameEventType.MoveBasket);
        MyEventBus.RaiseEvent(GameEventType.ShowNextChapter);
    }
    public void GoToScene()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void ShowCCPanel(int starsShow)
    {
        int coinCount = 0;

        if (starsShow == 1)
            coinCount = 10;
        else if (starsShow == 2)
            coinCount = 25;
        else if (starsShow == 3)
            coinCount = 40;

        winPanelCoins.text = coinCount.ToString();
        coinCount += PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.Coins, coinCount);
        SetCoins(coinCount.ToString());
        ChapterCompletePanel.SetActive(true);
        MyEventBus.RaiseEvent<int>(GameEventType.StarsShow, starsShow);

    }
    private void ShowNextChapterItems()
    {
        ChapterCompletePanel.SetActive(false);
        MyEventBus.RaiseEvent(GameEventType.MoveBasket);
        ResetPrefsData();
        MyEventBus.RaiseEvent(GameEventType.ShowNextChapter);
    }
    private void UndoItems()
    {
        MyEventBus.RaiseEvent(GameEventType.Undo);
    }
    public void ButtonSound()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
    }
    public void MoveSound()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.move);
    }
    public void UpdateTrial(int current)
    {
        if (current >= 0)
            lifeBoosterText.text = $"{current}";
    }
    public void OpenCompletePanel()
    {

    }
    public void SetTaskProgress(float p)
    {
    }
    public void SetMaxProgress(float p)
    {

    }
    public void SetCurrentChapter(string c)
    {

    }

    public void ShowFailedPanel()
    {
        GameManager.Instance.IsGameOver = true;
        failedPanel.SetActive(true);
    }
    public void RestartGame()
    {
        ResetPrefsData();
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetPrefsData()
    {
        GameManager.Instance?.ClearTrials();
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentTaskKey, 0);
        PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.ItemCounter);
        FindAnyObjectByType<CountdownTimer>().ResetTimer();
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.BasketItemProgress + i.ToString());
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.BasketProgress + i.ToString());
        }
        PlayerPrefs.Save();

    }
    public void SetSliderValue(float targetValue, float duration)
    {

    }
    private void SetCoins(string coins)
    {
        PlayCoinsAnimation(coins);
    }
    public void PlayCoinsAnimation(string coins)
    {
        RectTransform rectTransform = gamePlayCoins.GetComponent<RectTransform>();
        Vector2 originalPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(-50, rectTransform.anchoredPosition.y);

        // Move to target Y
        rectTransform.DOAnchorPos(targetPos, .3f)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gamePlayCoins.text = coins;
                // Return to original position
                rectTransform.DOAnchorPos(originalPos, .3f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
                HintBooster();
            });
    }
    private int GetCoinCount()
    {
        return PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins);
    }
    public void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(Blink());
        }
    }

    public void StopBlinking()
    {
        isBlinking = false;
        StopCoroutine(Blink());
        blinkSprite.color = Color.white;
    }
    private IEnumerator Blink()
    {
        Color originalColor = blinkSprite.color;

        while (isBlinking)
        {
            blinkSprite.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);

            blinkSprite.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

}
