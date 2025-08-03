using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemDetectType { Click, Drag }

public class ItemController : MonoBehaviour
{
    public Vector3 initialPosition;
    private bool isDragging = false;
    private bool isClick = false;
    private float dragThreshold = 0.1f;
    public Vector3 dragingOffset;
    public GameManager.ItemColor bottleColor;
    private Vector3 Offset;
    private Vector3 originalScale;
    private Vector3 originalScaleAtSlab;
    public int id;
    [HideInInspector]
    public bool isTurn = true;
    [HideInInspector]
    public bool isMoved = true;
    public bool enableTurn = false;
    private TutorialManager tutorialManager;
    public bool dragWhenPlace;
    public bool HintTutorialPlay;
    public ItemDetectType detectType;
    public DragItem dragItem;

    private SpriteRenderer spriteSortingOrder;
    private int originalSpriteSortingOrder;

    #region UNITY_METHOD
    void Start()
    {
        spriteSortingOrder = GetComponent<SpriteRenderer>();
        originalSpriteSortingOrder = spriteSortingOrder.sortingOrder;
        originalScale = transform.localScale;
        isTurn = true;
        initialPosition = transform.localPosition;
        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey) == 1)
        {
            if (!IsShowTutorial())
                Invoke(nameof(EnableTurn), .6f);
            else
                Invoke(nameof(ShowTutorial), .6f);
        }
    }
    public void HandleState()
    {
      Invoke(nameof(EnableTurn), .6f);
    }
    #endregion

    #region Dragging
    void OnMouseUp()
    {
     
        if (!enableTurn && !IsShowTutorial() || HintTutorialPlay) return;

        spriteSortingOrder.sortingOrder =
                    PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey) == 0 ? 10 : 0; ;

        if (isMoved)
            transform.localScale = originalScaleAtSlab;
        else
            transform.localScale = originalScale;

        if (dragItem != null)
            dragItem.MouseUp();
        else
        {
            if (isDragging && detectType == ItemDetectType.Drag)
            {
                SnapToNearestPosition();
            }
            else if (isClick && detectType == ItemDetectType.Click)
            {
                OnItemClick();
            }
        }
    }
    private void OnMouseDrag()
    {
       
        if (!enableTurn) return;
        if (GameManager.Instance.IsGameOver || IsShowTutorial()) return;

        if (!dragWhenPlace || isMoved || HintTutorialPlay)
        {
            dragItem?.MouseDrag();
            return;
        }


        if (dragItem != null)
        {
            dragItem.MouseDrag();
        }
        else
        {
            float distanceMoved = (GetMouseWorldPos() - (transform.position - dragingOffset)).magnitude;
            if (distanceMoved > 0) isClick = false;
            if (distanceMoved > dragThreshold)
            {
                isDragging = true;
                isClick = false; // If dragging occurs, it is not a click
                transform.position = GetMouseWorldPos() + dragingOffset;
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void SnapToNearestPosition(Transform closestSpot = null)
    {
        float minDistance = .6f;
        TileData tileData = null;
        tileData = closestSpot.GetComponent<TileData>();
        if (dragItem == null)
        {
            closestSpot = null;
            foreach (Transform spot in GameManager.Instance.TilePosition)
            {
                float distance = Vector3.Distance(transform.position, spot.position);

                if (distance <= minDistance)
                {
                    minDistance = distance;
                    closestSpot = spot;
                    // Debug.Log(distance + " distance if, " + spot.localPosition);
                }
                //   Debug.Log(distance + " distance e, " + spot.localPosition);
            }
        }


        if (closestSpot != null && dragItem ? true : minDistance < 2f)
        {
            if (!tileData.isPlaced || closestSpot?.GetChild(0)?.GetComponent<ItemController>().id == id)
            {
                GetTheTileUndoIndex();
                Debug.Log("Offset " + closestSpot.position + Offset);
                StartCoroutine(MoveAnimation(closestSpot.position + Offset));
                SoundManager.instance?.PlayOneShot(SoundManager.instance.item_move);
                //if (closestSpot.childCount > 0)
                //{
                //    if (closestSpot.GetChild(0).GetComponent<ItemController>().id != id)
                //    {
                //        GameManager.Instance.CheckItemsPlacement(this, closestSpot.GetComponent<TileData>().index);
                //    }
                //}
                //else
                    GameManager.Instance.CheckItemsPlacement(this, closestSpot.GetComponent<TileData>().index);

                originalScaleAtSlab = transform.localScale;
                isMoved = true;
                isDragging = false;
                isClick = false;
                Debug.Log("Upper inner In");
                if(PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey,0) == 0)
                {
                    FindFirstObjectByType<TutorialHandDrag>()?.OnCompleteTutorial();
                    PlayerPrefsHandler.SetInt(PlayerPrefsHandler.NewTutorialPrefKey, 1);
                }
            }
            else
            {
                transform.localPosition = initialPosition;

            }
        }
        else
        {
            transform.localPosition = initialPosition;

        }
    }
    #endregion

    private bool IsShowTutorial()
    {
        return PlayerPrefsHandler.GetInt(PlayerPrefsHandler.TutorialPrefKey, 0) == 0;
    }
    private void ShowTutorial()
    {
        if (IsShowTutorial())
        {
            tutorialManager = FindFirstObjectByType<TutorialManager>();
            TutorialManager.SetState = TutorialState.FirstMove;
            tutorialManager.indicatorUI.Add(gameObject.transform);
            tutorialManager.StartTutorial();
            if (tutorialManager.CurrentStep != id)
                enableTurn = false;
        }
    }

    #region CLICK EVENT
    void OnMouseDown()
    {
   
        if (GameManager.Instance.IsGameOver || HintTutorialPlay || !enableTurn) if (!IsShowTutorial()) return;

        spriteSortingOrder.sortingOrder = 
            PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey) == 0 ? 13 : 11;

        if (!isMoved)
            transform.localScale = transform.localScale * 1.1f;

        if (dragItem != null)
            dragItem.MouseDown();
        else
        {
            dragingOffset = transform.position - GetMouseWorldPos();
            isDragging = false;
            isClick = true;
        }
    }
    public void OnItemClick()
    {
        if (IsShowTutorial() && tutorialManager.CurrentStep == id)
            enableTurn = true;
        else
            if (!enableTurn) return;

        if (!GameManager.Instance.isChapterComplete)
        {
            if (!isMoved)
            {
                if (IsShowTutorial())
                {
                    if (TutorialManager.state == TutorialState.Undo)
                        UndoItem(tutorialManager.CurrentStep);
                    else if (TutorialManager.state == TutorialState.FirstMove)
                        StartCoroutine(MoveToCorrectIndex());
                    else if (TutorialManager.state == TutorialState.SecondMove)
                        StartCoroutine(MoveToCorrectIndex());
                }
                else if (dragItem == null)
                {
                    MyEventBus.RaiseEvent(GameEventType.CancelHintBooster);
                    StartCoroutine(CheckEmptyIndex());
                }
                else
                {
                    transform.localPosition = initialPosition;
                }
            }
            else if (!GameManager.Instance.CheckAllTurnComplete())
            {
                MyEventBus.RaiseEvent<int>(GameEventType.Undo, id);
                transform.localScale = originalScale;
                Debug.Log("RaiseEvent");
            }
            else
            {
                Debug.Log("Not RaiseEvent");
                transform.localPosition = initialPosition;
            }
            // MyEventBus.RaiseEvent(GameEventType.CancelHintBooster);
        }

    }
    private IEnumerator CheckEmptyIndex()
    {
        for (int i = 0; i < GameManager.Instance.TilePosition.Count; i++)
        {
            if (!GameManager.Instance.TilePosition[i].GetComponent<TileData>().isPlaced)
            {
                StartCoroutine(MoveAnimation(GameManager.Instance.TilePosition[i].position + Offset));
                SoundManager.instance?.PlayOneShot(SoundManager.instance.item_move);
                GameManager.Instance.CheckItemsPlacement(this, i);
                isMoved = true;
                yield return null;
                break;
            }
        }
    }
    private IEnumerator MoveToCorrectIndex()
    {
        for (int i = 0; i < GameManager.Instance.TilePosition.Count; i++)
        {
            if (GameManager.Instance.TilePosition[i].GetComponent<TileData>().TrueColor
                == bottleColor)
            {
                yield return StartCoroutine(MoveAnimation(GameManager.Instance.TilePosition[i].position + Offset));
                SoundManager.instance?.PlayOneShot(SoundManager.instance.item_move);
                GameManager.Instance.CheckItemsPlacement(this, i);
                isMoved = true;
                yield return null;
                break;
            }
        }
        if (TutorialManager.state == TutorialState.FirstMove && !tutorialManager.UndoTutorial)
        {
            TutorialManager.SetState = TutorialState.Undo;
            tutorialManager.indicatorUI[id] = (gameObject.transform);
            tutorialManager.StartTutorial();
            tutorialManager.UndoTutorial = true;
        }
        else if (TutorialManager.state == TutorialState.FirstMove)
        {
            tutorialManager.StepCompleted();
            TutorialManager.SetState = TutorialState.SecondMove;
        }
        else if (TutorialManager.state == TutorialState.SecondMove)
        {

            tutorialManager.StepCompleted();
        }
    }
    #endregion

    private void EnableTurn()
    {
        isMoved = false;
        enableTurn = true;
        Debug.Log("Enable");
    }
    private void SetOffsetPosition(int index)
    {
        switch (index)
        {
            case 0:
                Offset += (Vector3)GameManager.Instance.itemsOffsets[0].offsetForUpperSlab;
                break;
            case 1:
                Offset += (Vector3)GameManager.Instance.itemsOffsets[1].offsetForUpperSlab;
                break;
            case 2:
                Offset += (Vector3)GameManager.Instance.itemsOffsets[2].offsetForUpperSlab;
                break;
            case 3:
                Offset += (Vector3)GameManager.Instance.itemsOffsets[3].offsetForUpperSlab;
                break;
            case 4:
                Offset += (Vector3)GameManager.Instance.itemsOffsets[4].offsetForUpperSlab;
                break;
        }
    }
    float percent = 0;
    public void ChangeOffset(float percent)
    {
        this.percent = percent;
    }
    private float GetPercentage(float value, float percent)
    {
        return value * (percent / 100f);
    }
    private void DelayCall()
    {
        if (percent != 0)
            Offset -= new Vector3(Offset.x, GetPercentage(Offset.y, percent), 0);
    }
    public void ColorSet(GameManager.ItemColor color, int index)
    {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.bottleSprites[((int)color + (index * 7))];
        bottleColor = color;
        SetOffsetPosition(index);
        DelayCall();
    }

    #region EVENT SUBSCRIBE & UNSUBSCRIBE
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.ShakeBottle, ShakeBottle);
        MyEventBus.SubscribeEvent(GameEventType.LevelFailed, () => enableTurn = false);
        MyEventBus.SubscribeEvent<int>(GameEventType.Undo, UndoItem);
        MyEventBus.SubscribeEvent<TileData>(GameEventType.UsedHintBooster, UsedSlotExposeBoost);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.ShakeBottle, ShakeBottle);
        MyEventBus.UnSubscribeEvent(GameEventType.LevelFailed, () => enableTurn = false);
        MyEventBus.UnSubscribeEvent<int>(GameEventType.Undo, UndoItem);
        MyEventBus.UnSubscribeEvent<TileData>(GameEventType.UsedHintBooster, UsedSlotExposeBoost);
    }
    #endregion

    #region BOOSTER IMPLEMENTATION
    private void UsedSlotExposeBoost(TileData tile)
    {
        if (bottleColor == tile.TrueColor && isTurn && GameManager.Instance.usedSlotBooster && !GameManager.Instance.CheckAllTurnComplete())
        {
            StartCoroutine(MoveAnimation(tile.transform.position + Offset));
            GameManager.Instance.CheckItemsPlacement(this, tile.index);
            PlayerPrefsHandler.SetInt(PlayerPrefsHandler.HintBooster, PlayerPrefsHandler.GetInt(PlayerPrefsHandler.HintBooster) - 1);
            MyEventBus.RaiseEvent(GameEventType.UpdateHintBooster);
            GameManager.Instance.usedSlotBooster = false;
            isMoved = true;
        }
    }
    private void UndoItem(int index)
    {
        if (id == index && isTurn)
        {
            GetTheTileUndoIndex();
            SoundManager.instance?.PlayOneShot(SoundManager.instance.item_move);
            transform.SetParent(GameManager.Instance.ItemParent);
            StartCoroutine(MoveAnimationTopToBottom(transform.localPosition));
            isMoved = false;
        }

    }
    #endregion
    private void GetTheTileUndoIndex()
    {
        var tileData = transform.GetComponentInParent<TileData>();
        if (tileData == null) return;
        if (tileData.isPlaced)
        {
            tileData.isPlaced = false;
            tileData.isRight = false;
        }
    }

    #region ANIMATION
    private IEnumerator MoveAnimationTopToBottom(Vector3 to)
    {
        float moveDuration = .5f;
        float jumpHeight = 0.5f;

        Vector3 startPos = to;
        Vector3 endPos = initialPosition;

        float elapsedTime = 0f;

        // Move from top to bottom
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            float curveValue = moveCurve.Evaluate(t);

            float jumpOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight * (1f - t);

            transform.localPosition = Vector3.Lerp(startPos, endPos, curveValue) + new Vector3(0, jumpOffset, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPos;

        if (TutorialManager.state == TutorialState.Undo)
        {
            tutorialManager.indicatorUI[id] = (gameObject.transform);
            tutorialManager.StartTutorial();
            TutorialManager.SetState = TutorialState.FirstMove;
        }

    }

    [SerializeField] private AnimationCurve moveCurve;

    private IEnumerator MoveAnimation(Vector3 to)
    {
        yield return StartCoroutine(RotateShakeEffect());
        float duration = .1f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = to + new Vector3(0, 0.2f, 0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = moveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, targetPos, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0;
        duration = .1f;
        startPos = transform.position;
        targetPos = transform.position - new Vector3(0, 0.2f, 0);
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = moveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, to, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = to;

        yield return StartCoroutine(RotateShakeEffect());
    }

    private void ShakeBottle() { StartCoroutine(RotateShakeEffect()); }
    private IEnumerator RotateShakeEffect()
    {
        float shakeDuration = .2f;
        float shakeAngle = 3f;
        float elapsedTime = 0f;

        // Quaternion originalRotation = transform.rotation;

        while (elapsedTime < shakeDuration)
        {
            float zAngle = Random.Range(-shakeAngle, shakeAngle);

            transform.rotation = Quaternion.Euler(0f, 0f, zAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.identity;
    }



    #endregion
}
