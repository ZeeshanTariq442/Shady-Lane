using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerate : MonoBehaviour
{
    #region Fields
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject bottlePrefab;
    public GameObject tileParentPrefab;
    public GameObject bottleParentPrefab;
    public GameObject groundPrefab;
    public GameObject basketPrefab;
    public GameObject basketParentPrefab;
    public Transform gameplayParent;
    public Animation itemMergeBoom;


    [Header("Positions")]
    public Vector2 tileParentPosition;
    public Vector2 bottleParentPosition;
    public Vector2 groundSlabOffset;
    public Vector2 wrongTileOffset = new Vector2(0, 2f);
    private Vector3 itemsOffset;

    private GameObject basketParent;
    private List<GameObject> basket = new List<GameObject>();
    private Transform tileParent;
    private Transform bottleParent;
    private List<Transform> tilesParent = new List<Transform>();
    private List<Transform> tilesParentForHint = new List<Transform>();
    private string colorOrder;
    private bool destroyObject;
    int distance = 10;
    int distanceForHint = 0;
    List<int> rn = new List<int>();


    [Header("Hint Tutorial")]
    public HintTutorialHandler _HintTutorialHandler;
    #endregion

    #region UNITY Methods
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.LevelFailed, FailedTurn);
        MyEventBus.SubscribeEvent(GameEventType.TaskComplete, LoadNextLevel);
        MyEventBus.SubscribeEvent(GameEventType.ShowNextChapter, ShowNextChapter);
     //   MyEventBus.SubscribeEvent(EventType.UsedHintBooster, UsedHintBooster); Life Booster
    }

    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.LevelFailed, FailedTurn);
        MyEventBus.UnSubscribeEvent(GameEventType.TaskComplete, LoadNextLevel);
        MyEventBus.UnSubscribeEvent(GameEventType.ShowNextChapter, ShowNextChapter);
        // MyEventBus.UnSubscribeEvent(EventType.UsedHintBooster, UsedHintBooster);Life Booster
    }

    private void Start()
    {
        ShowNextChapter();
        if (!IsShowNewTutorial())
        {
            if (GameManager.Instance.currentActiveLevel == 0 && GameManager.Instance.currentTask == 0)

                if(HintTutorialShow())
                 ChangeItemLayer(11);

                _HintTutorialHandler.ShowTutorialPanel();
        }
    }
    #endregion
    private void ChangeItemLayer(int layerOrder)
    {
        for (int i = 0; i < tileParent.childCount; i++)
            tileParent.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = layerOrder;
    }
    public void ChangeItemToDefaultLayer(int layerOrder)
    {
        for (int i = 0; i < tileParent.childCount; i++)
            tileParent.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = layerOrder;
    }
    private void ShowNextChapter()
    {
        FindFirstObjectByType<SafeAreaDetector>().SetGamePlayYPosition();
        GameManager.Instance.isChapterComplete = false;
        GameManager.Instance?.SetUIData();
        ShuffleItems();
        BasketSpawn();
        GenerateLevel();
        if (GameManager.Instance.showCorrectOrder)
            ShowCorrectOrderColor();
    }
    private void BasketSpawn()
    {
        basket.Clear();
        basketParent = Instantiate(basketParentPrefab, gameplayParent);
        int basketCount = GameManager.Instance.AllLevels.GetUniqueItemCountsPerLevel()[GameManager.Instance.currentActiveLevel];
        for (int i = 0; i < basketCount; i++)
        {
            GameObject gb = Instantiate(basketPrefab, basketParent.transform);
            gb.GetComponent<BasketHandler>().id = i;
            basket.Add(gb);
            if (basketCount == 3)
            {
                basketParent.transform.localScale = new Vector2(0.95f,0.95f);
                basketParent.transform.position = new Vector2(0, 0.2f);
            }
        }
    }
    private void SetOffsetPosition()
    {
        switch ((int)GameManager.Instance.CurrentLevel.ItemName)
        {
            case 0:
                itemsOffset = GameManager.Instance.itemsOffsets[0].offsetForBottomSlab;
                break;
            case 1:
                itemsOffset = GameManager.Instance.itemsOffsets[1].offsetForBottomSlab;
                break;
            case 2:
                itemsOffset = GameManager.Instance.itemsOffsets[2].offsetForBottomSlab;
                break;
            case 3:
                itemsOffset = GameManager.Instance.itemsOffsets[3].offsetForBottomSlab;
                break;
            case 4:
                itemsOffset = GameManager.Instance.itemsOffsets[4].offsetForBottomSlab;
                break;
        }
    }
    private bool HintTutorialShow()
    {
        return PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintTutorial, 0) == 0;
    }
    private bool IsShowTutorial()
    {
        return PlayerPrefsHandler.GetInt(PlayerPrefsHandler.TutorialPrefKey, 0) == 0;
    }
    private bool IsShowNewTutorial()
    {
        return PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) == 0;
    }
    private void GenerateLevel()
    {
      
        SetOffsetPosition();
        LevelData levelData = GameManager.Instance.CurrentLevel;
        tileParent = Instantiate(tileParentPrefab, tileParentPosition, Quaternion.identity).transform;
        bottleParent = Instantiate(bottleParentPrefab, bottleParentPosition, Quaternion.identity).transform;
        Instantiate(groundPrefab, groundSlabOffset, Quaternion.identity);
        GameManager.Instance.ItemParent = bottleParent;

        if (levelData.totalItems == 6)
        {
            if (itemsOffset.y != 0)
                itemsOffset += new Vector3(itemsOffset.x, GetPercentage(itemsOffset.y, 20), 0);
            else
                itemsOffset += new Vector3(0,0.1f,0);
        }
        else if (levelData.totalItems == 7)
        {
            if (itemsOffset.y != 0)
                itemsOffset += new Vector3(itemsOffset.x, GetPercentage(itemsOffset.y, 32), 0);
            else
                itemsOffset += new Vector3(0, 0.15f, 0);
        }


        for (int i = 0; i < levelData.totalItems; i++)
        {
            // Tiles
            GameObject tile = Instantiate(tilePrefab, tileParent);
            if (levelData.totalItems == 6)
                tile.transform.localScale = new Vector2(0.55f,0.55f);
            else if(levelData.totalItems == 7)
                tile.transform.localScale = new Vector2(0.48f, 0.48f);

            tile.GetComponent<TileData>().TrueColor = levelData.bottleColors[i];
            tile.GetComponent<TileData>().index = i;

            //Items
            GameObject bottle = Instantiate(bottlePrefab, bottleParent);
            if (levelData.totalItems == 6)
            {
                bottle.transform.localScale = new Vector2(0.45f, 0.45f);
                bottle.GetComponent<ItemController>().ChangeOffset(10);
            }
            else if (levelData.totalItems == 7)
            {
                bottle.transform.localScale = new Vector2(0.38f, 0.38f);
                bottle.GetComponent<ItemController>().ChangeOffset(22);
            }


            bottle.GetComponent<ItemController>().ColorSet(levelData.bottleColors[rn[i]], (int)levelData.ItemName);
            bottle.transform.position = new Vector3(bottle.transform.position.x, bottle.transform.position.y - (itemsOffset.y), -0.5f);
            bottle.GetComponent<ItemController>().id = i;
            colorOrder += $" {levelData.bottleColors[i]}";
        }

        ArrangeSprites();
        GameManager.Instance.TileAdd(tileParent);
        tilesParent.Add(tileParent);
        FindFirstObjectByType<CountdownTimer>().ResumeTimer();
        UsedHintBooster();
        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) == 0)
            ShowNewGamePlayTutorial();

        if (GameManager.Instance.currentActiveLevel == 0 && GameManager.Instance.currentTask == 2)
        {
            if (HintTutorialShow())
                ChangeItemLayer(11);

            _HintTutorialHandler.ShowTutorialPanel();
        }
    }
    private void ShowNewGamePlayTutorial()
    {
        TutorialHandDrag tutorialHand = FindFirstObjectByType<TutorialHandDrag>();

        if (tutorialHand == null)
        {
            Debug.LogWarning("TutorialHandDrag not found in scene.");
            return;
        }

        ////for (int i = 0; i < bottleParent.childCount; i++)
        ////{
        ////    ItemController item = bottleParent.GetChild(i).GetComponent<ItemController>();

        ////    for (int j = 0; j < tileParent.childCount; j++)
        ////    {
        ////        TileData tile = tileParent.GetChild(j).GetComponent<TileData>();

        ////        if (item.bottleColor == tile.TrueColor)
        ////        {
        ////           // tutorialHand.Init(item.transform, tile.transform);
        ////            return; // Exit both loops after first match
        ////        }
        ////    }
        ////}
        ItemController item = bottleParent.GetChild(0).GetComponent<ItemController>();
        ItemController item2 = bottleParent.GetChild(1).GetComponent<ItemController>();
        TileData tile = tileParent.GetChild(1).GetComponent<TileData>();
        tutorialHand.Init(item.transform, tile.transform, item2.transform);
        item.HandleState();
        Debug.Log("No matching bottle and tile found for tutorial.");
    }

    private float GetPercentage(float value, float percent)
    {
        return value * (percent / 100f);
    }
    private void UsedHintBooster()
    {
        if (GameManager.Instance.showWrongTurn) return;
        GameManager.Instance.showWrongTurn = true;
        tilesParentForHint.Add(tilesParent[tilesParent.Count - 1]);
        tilesParent.RemoveAt(tilesParent.Count - 1);
    }
    private void ArrangeSprites()
    {
        bottleParent.GetComponent<SpriteHorizontalLayout>().ArrangeSpritesHorizontally();
        tileParent.GetComponent<SpriteHorizontalLayout>().ArrangeSpritesHorizontally();
        basketParent.GetComponent<SpriteHorizontalLayout>().ArrangeSpritesHorizontally();
    }

    private void ShowCorrectOrderColor()
    {
      //  Debug.Log("Correct Order: " + colorOrder);
    }

    private void ShuffleItems()
    {
        rn.Clear();
        LevelData levelData = GameManager.Instance.CurrentLevel;
        RandomNumberGenerator randomNumber = new RandomNumberGenerator(0, levelData.totalItems - 1);
        for (int i = 0; i < levelData.totalItems; i++)
        {
            rn.Add(randomNumber.GetNextUniqueNumber());
        }
    }
    private void LoadNextLevel()
    {
        ShuffleItems();
        StartCoroutine(CompleteLevelAnimation());
    }

    private int CheckEmptyBasket()
    {
        int task = GameManager.Instance.currentTask;
        int chapter = GameManager.Instance.currentActiveLevel;
        if (!GameManager.Instance.isChapterComplete)
        {
            task--;
        }
        else
        {
            if(chapter != 0)
            chapter--;

            task = GameManager.Instance.AllLevels.levels[chapter].task.Length - 1;
        }
        for (int i = 0; i < basket.Count; i++)
        {
            if (!basket[i].GetComponent<BasketHandler>().taskComplete
                && basket[i].GetComponent<BasketHandler>().MatchItemIndex(GameManager.Instance.AllLevels.levels[chapter].task[task].ItemName,chapter))
                return i;
        }
        return 0;
    }
    private IEnumerator CompleteLevelAnimation()
    {  
        yield return StartCoroutine(MergeItems(GetTheChilds(), basket[CheckEmptyBasket()].transform.position));
        basket[CheckEmptyBasket()].transform.GetChild(0).gameObject.SetActive(true);
        basket[CheckEmptyBasket()].GetComponent<BasketHandler>().ItemTaskComplete();
        yield return new WaitForSeconds(.5f);
        basket[CheckEmptyBasket()].transform.GetChild(0).gameObject.SetActive(false);
        destroyObject = true;
        for (int i = tilesParent.Count - 1; i >= 0; i--)
        {
            StartCoroutine(MoveObject(tilesParent[i], new Vector2(tilesParent[i].position.x, tilesParent[i].position.y + 10f)));
        }
        for (int i = tilesParentForHint.Count - 1; i >= 0; i--)
        {
            StartCoroutine(MoveObject(tilesParentForHint[i], new Vector2(tilesParentForHint[i].position.x, tilesParentForHint[i].position.y + 10f)));
        }
        destroyObject = false;
        Destroy(bottleParent.gameObject);
        tilesParent.Clear();
        tilesParentForHint.Clear();
        GameManager.Instance.TilePosition.Clear();

        if (CheckIfChapterIsComplete())
            GenerateLevel();

        GameManager.Instance.usedSlotBooster = false;

    }
    private bool CheckIfChapterIsComplete()
    {
        if (GameManager.Instance.currentTask != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private List<Transform> GetTheChilds()
    {
        // for (int i = 0; i < tilesParentForHint.Count; i++)
        //    tilesParent.Add(tilesParentForHint[i]);
        if (tilesParent.Count == 0 && GameManager.Instance.AreAllBottlesCorrect())
        {
            tilesParent.Add(tilesParentForHint[tilesParentForHint.Count - 1]);
        }
        GameManager.Instance.showWrongTurn = false;
        Transform lastTileParent = tilesParent[tilesParent.Count - 1];

        List<Transform> childTransforms = new List<Transform>();

        for (int i = 0; i < lastTileParent.childCount; i++)
        {
            childTransforms.Add(lastTileParent?.GetChild(i)?.GetChild(0));
            if (lastTileParent.GetChild(i).childCount >= 1)
                lastTileParent.GetChild(i).GetChild(0).gameObject.GetComponent<ItemController>().isTurn = false;
            lastTileParent.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 2;
            lastTileParent.GetChild(i).GetChild(0).transform.parent = null;
        }
        return childTransforms;
    }

    private void FailedTurn()
    {
        if(GameManager.Instance.currentItemTurnCount != 0)
        Invoke(nameof(RetryTurn), 1.5f);
    }

    public void RetryTurn()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.move);
        if (!GameManager.Instance.showWrongTurn)
            MoveTiles(tilesParent, wrongTileOffset);
        else
        {
            MoveTiles(tilesParentForHint, wrongTileOffset);
          //  PlayerPrefsHandler.SetInt(PlayerPrefsHandler.HintBooster, PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintBooster) - 1);
        }


        distanceForHint = 0;
     //   MyEventBus.RaiseEvent(EventType.UpdateHintBooster);

        GameManager.Instance.showWrongTurn = false;
        // tilesParentForHint.Clear();
        Destroy(bottleParent.gameObject);
        GameManager.Instance.TilePosition.Clear();
        if (GameManager.Instance.CheckTrialAvaiable())
        {
            GenerateLevel();
        }
        else
        {
            GameManager.Instance?.ShowFailedPanel();
        }


        GameManager.Instance.usedSlotBooster = false;
    }

    #region ANIMATION 
    private void MoveTiles(List<Transform> tilesParent, Vector2 wrongTileOffset)
    {
        for (int i = tilesParent.Count - 1; i >= 0; i--)
        {
            if (GameManager.Instance.showWrongTurn)
            {
                StartCoroutine(MoveObject(tilesParent[i], new Vector2(tilesParent[i].position.x, wrongTileOffset.y + distanceForHint)));
                distanceForHint++;
            }
            else
            {
                StartCoroutine(MoveObject(tilesParent[i], new Vector2(tilesParent[i].position.x, wrongTileOffset.y + distance)));
                distance++;
            }

            for (int j = tilesParent[i].childCount - 1; j >= 0; j--)
            {
                if (tilesParent[i].GetChild(j).childCount > 0)
                {
                    tilesParent[i].GetChild(j).GetChild(0).gameObject.GetComponent<ItemController>().isTurn = false;
                }
            }
        }
    }
    public IEnumerator MoveAndScaleUpSprites(List<Transform> sprites, Vector3 targetPosition, float targetScale, float duration)
    {
        float elapsedTime = 0f;

        List<Vector3> initialPositions = new List<Vector3>();
        List<Vector3> initialScales = new List<Vector3>();

        foreach (var sprite in sprites)
        {
            initialPositions.Add(sprite.position);
            initialScales.Add(sprite.localScale);
        }

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].position = Vector3.Lerp(initialPositions[i], targetPosition, t);

                float scaleValue = Mathf.Lerp(initialScales[i].x, targetScale, t);
                sprites[i].localScale = new Vector3(scaleValue, scaleValue, 1f);
            }
            if (elapsedTime >= duration / 3 && elapsedTime <= duration / 2)
            {
                yield return new WaitForSeconds(.5f);
                elapsedTime = duration / 2;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].position = targetPosition + new Vector3(0, 5);
            sprites[i].localScale = new Vector3(targetScale, targetScale, 1f);
        }
    }

    private IEnumerator MoveObject(Transform targetObject, Vector3 targetPosition)
    {
        float speed = 15f;
        while (Vector3.Distance(targetObject.position, targetPosition) > 0.01f)
        {
            targetObject.position = Vector3.Lerp(targetObject.position, targetPosition, speed * Time.deltaTime);
            if (targetObject.position.y >= 1.8f)
            {
                foreach (var spriteRenderer in targetObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (spriteRenderer != null)
                        spriteRenderer.sortingOrder = -6;
                }

            }

            yield return null;
        }
        targetObject.position = targetPosition;
        if (destroyObject)
            Destroy(targetObject.gameObject);
    }

    [Header("Item Settings")]
    public Transform mergePoint;
    public GameObject newItemPrefab;
    public GameObject merge_fx;

    [Header("Animation Settings")]
    public float moveDuration = .2f;
    public float newItemPopDuration = 0.2f;
    public AnimationCurve moveCurve;

    IEnumerator MergeItems(List<Transform> sprites, Vector3 targetPosition)
    {
        // Move and scale all items to the merge point
        foreach (var item in sprites)
        {
            StartCoroutine(MoveToPoint(item, mergePoint.position, moveDuration));
        }

        yield return new WaitForSeconds(moveDuration / 2);

        foreach (var item in sprites)
        {
            item.gameObject.SetActive(false);
        }
        GameObject newItem = Instantiate(newItemPrefab, mergePoint.position, Quaternion.identity);
        newItem.transform.GetComponent<SpriteRenderer>().sortingOrder = 3;
        newItem.GetComponent<SpriteRenderer>().sprite = basket[CheckEmptyBasket()].transform.GetChild(3).GetComponent<SpriteRenderer>().sprite;
        yield return StartCoroutine(PopEffect(newItem.transform));
        // yield return StartCoroutine(MoveToPoint(newItem.transform, basket.transform.position, moveDuration));
        yield return StartCoroutine(MoveAnimation(newItem.transform, basket[CheckEmptyBasket()].transform.position));
        newItem.transform.SetParent(basket[CheckEmptyBasket()].transform, true);
    }
    private IEnumerator MoveAnimation(Transform start, Vector3 to)
    {
        float duration = .2f;
        Vector3 startPos = start.position;
        Vector3 targetPos = to + new Vector3(0, 1f, 0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = moveCurve.Evaluate(t);

            start.position = Vector3.Lerp(startPos, targetPos, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0;
        duration = .2f;
        startPos = start.position;
        targetPos = start.position - new Vector3(0, .85f, 0);
        start.transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = moveCurve.Evaluate(t);

            start.position = Vector3.Lerp(startPos, targetPos, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        start.position = targetPos;
    }
    IEnumerator MoveToPoint(Transform obj, Vector3 target, float duration)
    {
        Vector3 start = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveValue = moveCurve.Evaluate(t);
            obj.position = Vector3.Lerp(start, target, curveValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = target; // ends exactly at the merge point
    }

    IEnumerator PopEffect(Transform obj)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one / 2;

        float elapsed = 0f;

        while (elapsed < newItemPopDuration)
        {
            float t = elapsed / newItemPopDuration;
            t = moveCurve.Evaluate(t);

            obj.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.localScale = endScale;
    }
    #endregion


}

