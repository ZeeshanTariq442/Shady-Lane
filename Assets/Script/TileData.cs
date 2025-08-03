using DG.Tweening;
using System.Collections;
using UnityEngine;
using System;

public class TileData : MonoBehaviour
{
    public GameManager.ItemColor TrueColor;
    public bool isRight;
    public bool isPlaced;
    public int index;
    public GameObject itemExploPrefab;
    public GameObject itemSmokePrefab;
    private bool showParticlesFX;
    private SpriteRenderer spriteRenderer;
    public Color blinkColor = Color.yellow;
    public float blinkDuration = 0.5f;
    public bool isBlinking = false;
    public Action OnHintTutorialAction;
    private BoxCollider2D collider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
    }
    public void ChangeTileColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
    public void DisableTileCollider()
    {
        collider.enabled = false;
    }
    #region Click Event
    void OnMouseDown()
    {
        if (!isPlaced && GameManager.Instance.usedSlotBooster == true)
        {
            StopFloat();
            MyEventBus.RaiseEvent<TileData>(GameEventType.UsedHintBooster, this);
            SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
        }
    }
    #endregion

    #region Blinking Tiles
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
        StopFloat();
        StopCoroutine(Blink());
        spriteRenderer.color = Color.white;
        OnHintTutorialAction?.Invoke();
    }

    private void ShowAvaliableFreeSlot()
    {
        if (!isRight && !isPlaced)
        {
            StartBlinking();
            PlayFloat();
            GameManager.Instance.usedSlotBooster = true;
        }
           
    }

    private void CancelBooster()
    {
        StopBlinking();
        GameManager.Instance.usedSlotBooster = false;
    }

    #endregion

    #region ANIMATION

    [Header("Floating Settings")]
    public float floatAmount = 0.05f;
    public float floatDuration = 1f;

    private Tween floatTween;
    private Vector2 originalPos;

    public void PlayFloat()
    {
        originalPos = transform.localPosition;
        // If already playing, skip
        if (floatTween != null && floatTween.IsActive()) return;

        floatTween = transform
            .DOLocalMoveY(transform.localPosition.y + floatAmount, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true); // for timescale = 0
    }

    public void StopFloat()
    {
        if (floatTween != null)
        {
            floatTween.Kill();
            floatTween = null;
            transform.localPosition = originalPos;
        }
        
    }

    private IEnumerator Blink()
    {
        Color originalColor = spriteRenderer.color;

        while (isBlinking)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
    private void ExploParticles()
    {
        if (!isRight && !showParticlesFX) 
        { 
            StartCoroutine(InvokeParticlesFX()); 
        }
        
    }

    IEnumerator InvokeParticlesFX()
    {
        transform.GetChild(0)?.gameObject.SetActive(false);
        Instantiate(itemSmokePrefab, transform).transform.localPosition = new Vector2(0, .7f);
        yield return new WaitForSeconds(.5f);
        SoundManager.instance?.PlayOneShot(SoundManager.instance.item_destroy);
        Instantiate(itemExploPrefab, transform).transform.localPosition = new Vector2(0, .7f);
        showParticlesFX = true;
    }
    #endregion

    #region EVENT SUBSCRIBE & UNSUBSCRIBE
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.LevelFailed, ExploParticles);
        MyEventBus.SubscribeEvent(GameEventType.CancelHintBooster, CancelBooster);
        MyEventBus.SubscribeEvent(GameEventType.UsedHintBooster, ShowAvaliableFreeSlot);
        MyEventBus.SubscribeEvent(GameEventType.UpdateHintBooster, StopBlinking);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.LevelFailed, ExploParticles);
        MyEventBus.UnSubscribeEvent(GameEventType.UsedHintBooster, ShowAvaliableFreeSlot);
        MyEventBus.UnSubscribeEvent(GameEventType.CancelHintBooster, CancelBooster);
        MyEventBus.UnSubscribeEvent(GameEventType.UpdateHintBooster, StopBlinking);
    }
    #endregion
}

