using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{

    #region Fields
    [HideInInspector]
    public int Number;
    public bool IsLocked;
    public GameObject LockLevel;
    public GameObject OpenLevel;
    public GameObject ActiveLevel;
    public GameObject tutorialHand;
    public Transform tutorialHandPoint;
    public TextMeshProUGUI levelNumber;

    public int StarsCount;
    public Transform StarsHolder;
    public Transform Star1;
    public Transform Star2;
    public Transform Star3;
    #endregion


    public void Start()
    {
        StarsCount = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentLevelStarKey + (Number - 1));

        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentLevelKey) >= Number - 1)
            UpdateState(StarsCount, false);
        else
            UpdateState(StarsCount, true);

        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentLevelKey) == Number - 1)
        {
            ShowActiveLevel();
            SetupTutorialOnInactivity();
        }

    }
    private void SetupTutorialOnInactivity()
    {
        // Cache TutorialHand reference
        TutorialHand _tutorialHand = FindFirstObjectByType<TutorialHand>();
        if (_tutorialHand == null)
        {
            Debug.LogWarning("TutorialHand not found.");
            return;
        }


        // Cache InactivityDetector reference
        InactivityDetector detector = FindFirstObjectByType<InactivityDetector>();
        if (detector == null)
        {
            return;
        }

        // Set the inactivity callback
        detector.OnInactivityDetected = () =>
        {
            _tutorialHand.PlayHandToTarget();
            Instantiate(tutorialHand, tutorialHandPoint);
        };


    }

    #region click

    public void OnClick()
    {
        OnLevelSelected();
    }



    #endregion

    #region Check & Load Scene
    private void OnLevelSelected()
    {
        if (!IsLocked)
        {
            //  if(PlayerPrefsHandler.GetString(PlayerPrefsHandler.ChapterComplete + "" + (Number - 1)) == "YES")

            ResetPrefsData();
            PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentActiveLevelKey, Number - 1);
            LevelManager.Instance?.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }

    private void ResetPrefsData()
    {
        // PlayerPrefsHandler.SetInt(PlayerPrefsHandler.UsedTrialKey + $"{Number - 1}", 0);
        // PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentTaskKey, 0);
        PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.ItemCounter);
        PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.CurrentTaskKey);
        PlayerPrefsHandler.SetString(PlayerPrefsHandler.LastSavedTime + (Number - 1), "0");
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.BasketItemProgress + i.ToString());
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.BasketProgress + i.ToString());
        }
        PlayerPrefs.Save();

    }

    #endregion

    #region Update Level Data
    public void UpdateState(int starsCount, bool isLocked)
    {
        UpdateStars(starsCount);
        IsLocked = isLocked;
        levelNumber.text = Number.ToString();
        LockLevel.gameObject.SetActive(isLocked);
        OpenLevel.gameObject.SetActive(!isLocked);
    }

    public void UpdateStars(int starsCount)
    {
        Star1?.gameObject.SetActive(starsCount >= 1);
        Star2?.gameObject.SetActive(starsCount >= 2);
        Star3?.gameObject.SetActive(starsCount >= 3);
    }

    private void ShowActiveLevel()
    {
        StarsHolder.gameObject.SetActive(false);
        ActiveLevel.gameObject.SetActive(true);

    }
    #endregion


}
