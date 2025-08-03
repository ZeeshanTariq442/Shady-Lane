using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    #region Enums & Variables
    [System.Serializable]
    public enum ItemColor { One, Two, Three, Four, Five, Six, Seven }
    public enum Items
    {
        Cake,
        Donet,
        IceCream,
        Mac,
        Tofee
    }
    public static GameManager Instance;

    [Header("Game Settings")]
    public bool showCorrectOrder;
    public bool showWrongTurn;
    public bool usedSlotBooster;
    public int fireworkShowRadius = 3;

    [Header("Level Data")]
    public LevelStage AllLevels;
    [HideInInspector]
    public LevelData CurrentLevel;
    [HideInInspector]
    public int currentLevel, currentTask, currentActiveLevel;
    [HideInInspector]
    public int currentTrial;
    [HideInInspector]
    public int currentItemTurnCount;
    [HideInInspector]
    private bool isGameComplete;
    [HideInInspector]
    public bool isChapterComplete;
    [HideInInspector]
    public bool IsGameOver;
    [HideInInspector]
    private bool waitForCheck;

    [Header("UI & Visuals")]
    public UIHandler uiHandler;
    public GameObject WinPanel;
    public Sprite[] bottleSprites;
    public GameObject fireworkPrefab;
    public Transform fireworkParent;

    [Header("Tile Management")]
    public List<Transform> TilePosition = new List<Transform>();
    public Vector3 OffsetTile;
    [HideInInspector] public Transform ItemParent;

    [Header("Items Offsets")]
    public List<ItemsOffset> itemsOffsets;

    [Header("Character")]
    public GameObject characterParent;

    #endregion

    #region Unity Methods
    private void Awake() { Instance = this; }

    private void Start()
    {
        LoadGameData();   
    }
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.Undo, CheckItemThatPlace);
        MyEventBus.SubscribeEvent(GameEventType.LoadCompletePanel, ShowCompletePanel);
        MyEventBus.SubscribeEvent(GameEventType.RestartChapter, RestartCurrentChapter);
        MyEventBus.SubscribeEvent(GameEventType.RefillLife, RefillLife);

    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.Undo, CheckItemThatPlace);
        MyEventBus.UnSubscribeEvent(GameEventType.LoadCompletePanel, ShowCompletePanel);
        MyEventBus.UnSubscribeEvent(GameEventType.RestartChapter, RestartCurrentChapter);
        MyEventBus.UnSubscribeEvent(GameEventType.RefillLife, RefillLife);

    }
    #endregion

    private void RefillLife()
    {
        
        int lifeCount = LoadTrials(currentActiveLevel).remainingTrials;
        lifeCount++;
       // uiHandler.UpdateTrial(lifeCount);

        int it = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.ITrialKey + $"{currentActiveLevel}");
        it++;
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.ITrialKey + $"{currentActiveLevel}",it);
        SaveTrials(currentActiveLevel, LoadTrials(currentActiveLevel).totalTrials - currentTrial);

        currentTrial = lifeCount;//LoadTrials(currentActiveLevel).remainingTrials;
    }
    private void RestartCurrentChapter()
    {
        ClearTrials();
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentLevelKey, currentLevel - 1);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentTaskKey, 0);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.ItemCounter, AllLevels.levels[currentActiveLevel].itemsTurn);
        currentItemTurnCount = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.ItemCounter);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentActiveLevelKey, currentActiveLevel - 1);
        ResetBasketItemData();
        FindAnyObjectByType<CountdownTimer>().ResetTimer();
        SetUIData();
    }

    private void LoadGameData()
    {
        isGameComplete = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.GameComplete, 0) == 1;
        isChapterComplete = PlayerPrefsHandler.GetString(PlayerPrefsHandler.ChapterComplete + "" + currentActiveLevel) == "YES";
    }
    public void IncreaseTrail()
    {
        AllLevels.levels[currentActiveLevel].trails = AllLevels.levels[currentActiveLevel].trails + 1;
    }
    public void SaveTrials(int levelIndex, int usedTrials)
    {
        int IncreaseTrials = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.ITrialKey + $"{currentActiveLevel}");
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.TotalTrialKey + $"{levelIndex}" , AllLevels.levels[currentActiveLevel].trails + IncreaseTrials);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.UsedTrialKey + $"{levelIndex}", usedTrials);
        PlayerPrefs.Save();
    }
    public void ClearTrials()
    {
        for(int i = 0; i < AllLevels.levels.Length; i++)
        {
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.TotalTrialKey + $"{i}");
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.UsedTrialKey + $"{i}");
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.ITrialKey + $"{i}");
        }
        PlayerPrefs.Save();
    }
    public (int totalTrials, int usedTrials, int remainingTrials) LoadTrials(int levelIndex)
    {
        int IncreaseTrials = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.ITrialKey + $"{currentActiveLevel}");
        int totalTrials = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.TotalTrialKey + $"{levelIndex}", AllLevels.levels[levelIndex].trails)  + IncreaseTrials;
        int usedTrials = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.UsedTrialKey + $"{levelIndex}", 0);
        int remainingTrials = totalTrials - usedTrials;
        return (totalTrials, usedTrials, remainingTrials);
    }


    public void SetUIData()
    {
        if (!isGameComplete)
        {
            currentLevel = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentLevelKey, 0);
            currentActiveLevel = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentActiveLevelKey, 0);

            currentTask = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentTaskKey, 0); //Assign Current Active Task

         //   currentTask = 0; // reset when reagain play level

            CurrentLevel = AllLevels.levels[currentActiveLevel].task[currentTask];

            currentItemTurnCount = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.ItemCounter, AllLevels.levels[currentActiveLevel].itemsTurn);
            uiHandler.SetItemCounter(currentItemTurnCount);

       //     int activeCount = AllLevels.GetUniqueItemCountsPerLevel()[currentLevel];
             int activeCount = AllLevels.GetUniqueItemCountsPerLevel()[currentActiveLevel];
            for (int i = 0; i < characterParent.transform.childCount; i++)
            {
                characterParent.transform.GetChild(i).gameObject.SetActive(i < activeCount);
                if(activeCount == 2)
                characterParent.GetComponent<HorizontalLayoutGroup>().spacing = -220f;
            }

            currentTrial = LoadTrials(currentActiveLevel).remainingTrials;
            uiHandler.UpdateTrial(currentTrial);
            if (isChapterComplete)
                ResetBasketItemData();
        }
        else
        {
            uiHandler.OpenCompletePanel();
        }

    }
    public void TileAdd(Transform tileParent)
    {
        foreach (Transform tile in tileParent)
        {
            TilePosition.Add(tile);
        }
    }

    public void CheckItemsPlacement(ItemController bottle, int index)
    {
        if (bottle.bottleColor == TilePosition[index].GetComponent<TileData>().TrueColor)
        {
            TilePosition[index].GetComponent<TileData>().isRight = true;
            TilePosition[index].GetComponent<TileData>().isPlaced = true;
        }
        else
        {
            TilePosition[index].GetComponent<TileData>().isPlaced = true;
        }
        if(currentItemTurnCount == 0 && !isChapterComplete)
        {
            ShowFailedPanel();
        }
        else
        {
            currentItemTurnCount--;
            PlayerPrefsHandler.SetInt(PlayerPrefsHandler.ItemCounter, currentItemTurnCount);
        }
        uiHandler.SetItemCounter(currentItemTurnCount);
        
        bottle.gameObject.transform.SetParent(TilePosition[index].transform);
        CheckFreeSlot();

        if (currentItemTurnCount == 0 && !CheckAllTurnComplete())
        {
            Invoke(nameof(ShowFailedPanel), 1f);
            return;
        }
        CheckTaskComplete();

       
    }
    private void CheckFreeSlot()
    {
        foreach (var t in TilePosition)
        {
            if (t.transform.childCount == 0)
            {
                t.GetComponent<TileData>().isPlaced = false;
            }
        }

    }
    public void CheckTaskComplete()
    {
        if (CheckAllTurnComplete())
        {
            if (AreAllBottlesCorrect())
            {
                CompleteTask();  
                Invoke(nameof(waitSomeSeconds), 2f);
                CheckIfChapterIsComplete();
               // Debug.Log("Level Completed!");
                Invoke(nameof(ShowColor), 1.2f);
                Invoke(nameof(CompleteTurnEffect), 1f);

                // Move to next level
            }
            else
            {
                //   Debug.Log("Level Failed!");
                DisableFailedTurnCollider();
                Invoke(nameof(ShowColor), 1.6f);
                Invoke(nameof(RetryTurn), 1f);
            }
            MyEventBus.RaiseEvent(GameEventType.CancelHintBooster);
        }  
    }
    private void DisableFailedTurnCollider()
    {
        foreach (var t in TilePosition)
        {
            t.GetComponent<TileData>().DisableTileCollider();
        }
    }
    private void CompleteTurnEffect()
    {
        MyEventBus.RaiseEvent(GameEventType.CompleteTurnEffect);
    }
    private void CheckIfChapterIsComplete()
    {
        if (currentTask == 0)
        {
            Invoke(nameof(CallByDelay), 4f); //6

        }
        else
        {
            Invoke(nameof(SetUIData), 1.5f);
        }
    }
    private void CallByDelay()
    {
        MyEventBus.RaiseEvent(GameEventType.PlayingMergeAnimation);
        SoundManager.instance?.StopMusic();
        SoundManager.instance?.PlayOneShot(SoundManager.instance.complete);
    }
    private void ShowCompletePanel()
    {
        int giveStars = CalculateStars(LoadTrials(currentActiveLevel - 1).remainingTrials);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentLevelStarKey + (currentActiveLevel - 1), giveStars);
        uiHandler?.ShowCCPanel(giveStars);
        StartCoroutine(FireworkParticles());
        FindAnyObjectByType<CountdownTimer>().ResetTimer();
    }
    private void waitSomeSeconds()
    {
        ChangeEventState(GameEventType.CameraShake);
        ChangeEventState(GameEventType.TaskComplete);
    }
    public int GetTheLastPlaceIndex(int index, int id)
    {
        int lastPlacedId = -1; // Use -1 as a default value 

        index = index == 0 ? 0 : index - 1;
        var tileData = TilePosition[index].GetComponent<TileData>();
        if (tileData.isPlaced)
        {
            lastPlacedId = id;
            tileData.isPlaced = false;
            tileData.isRight = false;
        }

        if (lastPlacedId == -1)
        {
            Debug.Log("No valid placement found");
        }

        return lastPlacedId;
    }

    private void ChangeEventState(GameEventType type)
    {
        MyEventBus.RaiseEvent(type);
    }
    private void ShowColor()
    {
        foreach (var t in TilePosition)
        {
            if (t.GetComponent<TileData>().isRight)
            {
                t.GetComponent<TileData>().ChangeTileColor(Color.green);
            }
            else
            {
                t.GetComponent<TileData>().ChangeTileColor(Color.red);
            }

        }
        PlayShineEffect();

        if(AreAllBottlesCorrect())
        SoundManager.instance?.PlayOneShot(SoundManager.instance.correct);
        else
        SoundManager.instance?.PlayOneShot(SoundManager.instance.wrong_item_move);

        if (currentItemTurnCount == 0)
        {
            Invoke(nameof(ShowFailedPanel), 1f);
            return;
        }
    }

    private void PlayShineEffect()
    {
        foreach (var t in TilePosition)
        {
            if (t.GetComponent<TileData>().isRight)
            {
                t.GetComponent<TileData>().transform.GetChild(0)?.transform.GetComponent<SpriteFlashTool>()?.FlashAll();
                t.GetComponent<SpriteFlashTool>()?.FlashAll();
            }
        }
    }
    public bool AreAllBottlesCorrect()
    {
        foreach (var t in TilePosition)
        {
            if (!t.GetComponent<TileData>().isRight)
            {
                return false;
            }
        }
        return true;
    }
    private void CheckItemThatPlace()
    {
        int undoPlace = 0;
        int index = 0;
        foreach (var t in TilePosition)
        {
            if (t.GetComponent<TileData>().isPlaced)
            {
                if (t.GetComponent<TileData>().transform.childCount >= 1)
                {
                    undoPlace = t.GetComponent<TileData>().transform.GetChild(0).transform.GetComponent<ItemController>().id;
                    index++;
                }

            }
        }
        if (index != 0)
        {
            GetTheLastPlaceIndex(index, undoPlace);
            MyEventBus.RaiseEvent<int>(GameEventType.Undo, undoPlace);
        }

    }
    public bool CheckAllTurnComplete()
    {
        foreach (var t in TilePosition)
        {
            if (!t.GetComponent<TileData>().isPlaced)
            {
                return false;
            }
        }
        return true;
    }

    private void RetryTurn()
    {
        if (currentTrial > 0)
        {
            currentTrial--;
            SaveTrials(currentActiveLevel, LoadTrials(currentActiveLevel).totalTrials - currentTrial);
            MyEventBus.RaiseEvent(GameEventType.UpdateLifeBooster);
        }
        // uiHandler.UpdateTrial(currentTrial);
        ChangeEventState(GameEventType.LevelFailed);
        ChangeEventState(GameEventType.CameraShake);

        if (currentItemTurnCount == 0)
            return;

    }
    public void ShowFailedPanel()
    {
        Invoke(nameof(CallFailedPanelWithDelay),0f); 
    }
    private void CallFailedPanelWithDelay()
    {
        uiHandler.ShowFailedPanel();
        SoundManager.instance?.StopMusic();
        SoundManager.instance?.PlayOneShot(SoundManager.instance.failed);
    }
    public bool CheckTrialAvaiable()
    {
        if (currentTrial <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void ResetBasketItemData()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.BasketItemProgress + i.ToString());
            PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.BasketProgress + i.ToString());
        }
        PlayerPrefs.Save();
    }
    public void CompleteTask()
    {
        if (currentTask < AllLevels.levels[currentActiveLevel].task.Length - 1)
        {
            currentTask++;
            isChapterComplete = false;
        }
        else
        {
            currentTask = 0;
            if (currentActiveLevel < currentLevel)
            {
                currentActiveLevel++;
            }
            else
            {
                currentLevel++;
                currentActiveLevel++;
            }
            FindFirstObjectByType<CountdownTimer>().StopTimer();
            ResetBasketItemData();
            isChapterComplete = true;
            if (currentActiveLevel >= AllLevels.levels.Length)
            {
                currentLevel--;
                currentActiveLevel--;
                isGameComplete = true;
                //PlayerPrefsHandler.SetInt(PlayerPrefsHandler.GameComplete, 1);
                uiHandler.OnAllLevelComplete();
                Debug.Log("Congratulations! All levels are complete.");
                return;
            }
            PlayerPrefsHandler.SetString(PlayerPrefsHandler.ChapterComplete + "" + (currentActiveLevel - 1),"YES");
        }
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentLevelKey, currentLevel);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentTaskKey, currentTask);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.CurrentActiveLevelKey, currentActiveLevel);
        CurrentLevel = AllLevels.levels[currentActiveLevel].task[currentTask];
      //  Debug.Log($"Current Level: {currentLevel}, Current Task: {currentTask}");

    }
    private int CalculateStars(int trials)
    {
        int totalTrials = AllLevels.levels[currentActiveLevel - 1].trails;  
        if (trials >= totalTrials) return 3;  // Best
        if (trials >= totalTrials - 2) return 2;  // Medium
        return 1;  // Poor
    }


    #region ANIMATION
    IEnumerator FireworkParticles()
    {
        yield return new WaitForSeconds(.5f);
        fireworkParent.gameObject.SetActive(true);
        GameObject firework = Instantiate(fireworkPrefab, UnityEngine.Random.insideUnitCircle * fireworkShowRadius, Quaternion.identity) as GameObject;
        GameObject firework1 = Instantiate(fireworkPrefab, UnityEngine.Random.insideUnitCircle * fireworkShowRadius, Quaternion.identity) as GameObject;
        firework.transform.SetParent(fireworkParent, false);
        firework1.transform.SetParent(fireworkParent, false);
        for (int i = 0; i < fireworkShowRadius; i++)
        {
            yield return new WaitForSeconds(1f);
            firework.transform.position = UnityEngine.Random.insideUnitCircle * fireworkShowRadius;
            firework.GetComponent<ParticleSystem>().Stop();
            firework.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(1f);
            firework1.transform.position = UnityEngine.Random.insideUnitCircle * fireworkShowRadius;
            firework1.GetComponent<ParticleSystem>().Stop();
            firework1.GetComponent<ParticleSystem>().Play();

        }
        Destroy(firework, 1);
        Destroy(firework1, 1);
    }

    #endregion
}
[System.Serializable]
public class ItemsOffset
{
    public Vector2 offsetForUpperSlab;
    public Vector2 offsetForBottomSlab;
    public GameManager.Items Items;
}