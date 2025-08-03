using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Advertisement.IosSupport.Samples;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI Elements")]
    public RectTransform levelsParent;
    public GameObject levelPrefab;
    public GameObject levelParent;
    public GameObject container;
    public GameObject loadingScreen;
    public Image progressBar;
    public Button nextButton;
    public Button backButton;

    [Header("Level Data")]
    public LevelStage levelData;
    public float itemDistanceSpace = 50f;

    private List<GameObject> levelPages = new List<GameObject>();
    private int currentIndex = 0;
    private int pagesCount = 1;
    private int levelsPerPage;

    private void Awake()
    {
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.Coins, PlayerPrefsHandler.GetInt(PlayerPrefsHandler.Coins, 10));
        Instance = this;
        CalculatePageData();
        GenerateLevels();
        UpdateNavigationButtons();
        ClearTrials();
    }
    private void OnEnable()
    {
        AdsController.Instance?.HideBannerAd();
    }
    private void CalculatePageData()
    {
        float containerHeight = container.GetComponent<RectTransform>().rect.height;
        float levelHeight = 0;

        levelsPerPage = 0;
        while (levelHeight <= containerHeight)
        {
            levelHeight += 140 + 50;
            levelsPerPage++;
        }
        if (levelHeight != containerHeight)
            levelsPerPage--;


        int items = levelsPerPage * 3;
        while (items < levelData.levels.Length)
        {
            items += levelsPerPage * 3;
            pagesCount++;
        }
    }

    private void GenerateLevels()
    {
        for (int i = 0; i < pagesCount; i++)
        {
            GameObject page = Instantiate(levelParent, container.transform);
            RectTransform rect = page.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(i * levelsParent.rect.width, 0);
            rect.sizeDelta = new Vector2(levelsParent.rect.width, levelsParent.rect.height);
            levelPages.Add(page);  
        }

        int levelIndex = 0;
        foreach (GameObject page in levelPages)
        {
            for (int i = 0; i < levelsPerPage * 3 && levelIndex < levelData.levels.Length; i++)
            {
                GameObject level = Instantiate(levelPrefab, page.transform);
                level.GetComponent<Level>().Number = levelIndex + 1;
                levelIndex++;
            }
        }
        container.GetComponent<SwipeItemController>().SetItemsInList(levelPages);
        container.GetComponent<SwipeItemController>().enabled = true;
    }

    public void MoveNext()
    {
        PlayClickSound();
        if (currentIndex < pagesCount - 1)
        {
            currentIndex++;
            MoveToPage(-levelsParent.rect.width * currentIndex);
        }
        UpdateNavigationButtons();
    }

    public void MoveBack()
    {
        PlayClickSound();
        if (currentIndex > 0)
        {
            currentIndex--;
            MoveToPage(-levelsParent.rect.width * currentIndex);
        }
        UpdateNavigationButtons();
    }

    private void MoveToPage(float targetX)
    {
        StartCoroutine(MoveRectTransform(container.GetComponent<RectTransform>(), new Vector2(targetX, 0), 0.3f));
    }

    private IEnumerator MoveRectTransform(RectTransform rectTransform, Vector2 target, float duration)
    {
        Vector2 start = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = target;
    }

    private void UpdateNavigationButtons()
    {
        nextButton.interactable = currentIndex < pagesCount - 1;
        backButton.interactable = currentIndex > 0;
    }

    public void LoadScene(int sceneIndex)
    {
        PlayClickSound();
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float targetProgress = 0f; 
        float smoothSpeed = 3f;    

        while (!operation.isDone)
        {

            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, targetProgress, Time.deltaTime * smoothSpeed);

            if (operation.progress >= 0.9f && progressBar.fillAmount >= 0.99f)
            {
                yield return new WaitForSeconds(.5f); 
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void ClearTrials()
    {
        for (int i = 0; i < levelData.levels.Length; i++)
        {
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.TotalTrialKey + $"{i}");
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.UsedTrialKey + $"{i}");
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.ITrialKey + $"{i}");
        }
        PlayerPrefs.Save();
    }
    private void PlayClickSound()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
    }
}
