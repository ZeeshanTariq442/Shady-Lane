using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class BasketHandler : MonoBehaviour
{
    #region Field
    public SpriteRenderer mask;
    public SpriteRenderer item;
    public GameObject itemPrefab;
    public Text itemCount;
    public Sprite[] itemsSprite_Mask;
    public Sprite[] itemsSprite;
    public int id;

    //Item Animation Setting
    public float scaleDuration = .5f;
    public float scaleFactor = 1.2f;
    public AnimationCurve scaleCurve;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    public float popUpDuration = 0.5f;


    public bool taskComplete;
    private int itemsCount;
    private int currentItemsDone;
    private int counter;
    #endregion

    void Start()
    {
        LoadProgress();
        
        mask.sprite = itemsSprite_Mask[(int)GameManager.Instance.AllLevels.GetItemImageIndex(GameManager.Instance.currentActiveLevel)[id]];
        item.sprite = itemsSprite[(int)GameManager.Instance.AllLevels.GetItemImageIndex(GameManager.Instance.currentActiveLevel)[id]];
        itemsCount = (GameManager.Instance.AllLevels.GetEachUniqueItemCountsPerLevel(GameManager.Instance.currentActiveLevel)[id]);
        itemCount.text = itemsCount.ToString();
        initialScale = item.transform.localScale;
        counter = itemsCount;

        SetDefaultItemValue();
        StartCoroutine(BasketPopAnimation(0.2f));
    }
    public bool MatchItemIndex(GameManager.Items item,int chapter)
    {
        return item == GameManager.Instance.AllLevels.GetItemImageIndex(chapter)[id];
    }
    public void ItemTaskComplete()
    {
        if (itemsCount == currentItemsDone)
        {
            taskComplete = true;
        }
        else
        {
            counter--;
            currentItemsDone++;
            if (itemsCount == currentItemsDone) taskComplete = true;
        }

        itemCount.text = counter.ToString();
        item.gameObject.SetActive(true);
        StartCoroutine(FadeSprite(item.color.a, (float)currentItemsDone / (float)itemsCount, 1f));
        BasketShakeAnimation();
    }
    private void SetDefaultItemValue()
    {
        if (currentItemsDone >= 1)
        {
            GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            newItem.GetComponent<SpriteRenderer>().sprite = item.sprite;
            newItem.transform.SetParent(transform, true);
            newItem.transform.localPosition = new Vector3(0, .25f, 0);
            newItem.transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        itemCount.text = counter.ToString();
        item.gameObject.SetActive(true);
        StartCoroutine(FadeSprite(item.color.a, (float)currentItemsDone / (float)itemsCount, 1f));
        if (itemsCount == currentItemsDone) taskComplete = true;
    }
    private void SaveProgress()
    {
        if (!PlayerPrefsHandler.HasKey(PlayerPrefsHandler.BasketProgress)) return;
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.BasketProgress + "" + id, counter);
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.BasketItemProgress + "" + id, currentItemsDone);
    }
    private void LoadProgress()
    {
        counter = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.BasketProgress + id.ToString(), counter);
        currentItemsDone = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.BasketItemProgress + id.ToString(), currentItemsDone);
     }

    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.MoveBasket, MoveAndDestroy);
        MyEventBus.SubscribeEvent(GameEventType.PlayingMergeAnimation, StartPlayingMergeAnimation);
    }
    private void OnDisable()
    {
        if(!GameManager.Instance.isChapterComplete)
        SaveProgress();

        MyEventBus.UnSubscribeEvent(GameEventType.MoveBasket, MoveAndDestroy);
        MyEventBus.UnSubscribeEvent(GameEventType.PlayingMergeAnimation, StartPlayingMergeAnimation);
    }

    private void InvisibleBasketItems(bool diaplay = false)
    {
        for (int i = 6; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(diaplay);
        }
    }

    #region ANIMATIONS
    IEnumerator FadeSprite(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = item.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            color.a = alpha;
            item.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        item.color = color;
    }
    private void StartPlayingMergeAnimation()
    {
        // StartCoroutine(ScaleUpAndDown());
        StartCoroutine(BasketMoveOnCenter(0.5f));
    }

    private void MoveAndDestroy()
    {
        StartCoroutine(MoveObject(transform, new Vector3(-10, transform.position.y, 0)));

    }

    private IEnumerator MoveObject(Transform targetObject, Vector3 targetPosition)
    {
        float speed = 5f;
        while (Vector3.Distance(targetObject.position, targetPosition) > 0.01f)
        {
            targetObject.position = Vector3.Lerp(targetObject.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        targetObject.position = targetPosition;
        Destroy(gameObject);
    }

    IEnumerator BasketPopAnimation(float moveDuration)
    {
        yield return transform.DOMove(new Vector2(transform.position.x, 3.5f), moveDuration)
             .SetEase(Ease.Linear)
             .WaitForCompletion();

        yield return transform.DOMove(new Vector2(transform.position.x, 2.77f), moveDuration)
            .SetEase(Ease.Linear)
            .WaitForCompletion();
    }

    IEnumerator ScaleUpAndDown()
    {
        InvisibleBasketItems(true);
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(1).gameObject.SetActive(false);
        // Scale up
        yield return StartCoroutine(ScaleTo(Vector3.one * scaleFactor, scaleDuration));

        // Optional delay between scale up and down
        yield return new WaitForSeconds(0.5f);
        // Scale down
        yield return StartCoroutine(ScaleTo(initialScale, scaleDuration));

    }
    IEnumerator BasketMoveOnCenter(float moveDuration)
    {
        initialPosition = transform.position;
        transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        transform.GetChild(1).gameObject.SetActive(false);
        //ChangeSortingLayer(this.gameObject,7);
        StartCoroutine(FadeSprite(item.color.a, 1,.5f));
        yield return transform.DOMove(new Vector2(transform.position.x, -1), moveDuration)
             .SetEase(Ease.InBack)
             .WaitForCompletion();

        // Wait for ScaleUpAndDown to complete
        yield return StartCoroutine(ScaleUpAndDown());

        // Move back to initial position
        yield return transform.DOMove(new Vector2(transform.position.x, initialPosition.y), moveDuration)
            .SetEase(Ease.OutBack)
            .WaitForCompletion();  
        yield return new WaitForSeconds(moveDuration);
         MyEventBus.RaiseEvent(GameEventType.LoadCompletePanel);
    }
    IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = item.transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            t = scaleCurve.Evaluate(t); // Use curve for smooth scaling
            item.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            time += Time.deltaTime;
            yield return null;
        }

        item.transform.localScale = targetScale; // Ensure final scale is exact
    }

    IEnumerator PopUpBasket()
    {
        Vector3 originalScale = new Vector3(0.2f,0.2f,0.2f);
        Vector3 targetScale = Vector3.one * 0.5f; // Slightly larger

        float elapsedTime = 0f;
        while (elapsedTime < popUpDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / popUpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; 
    }

    public void BasketShakeAnimation()
    {
        GetComponent<DOTweenAnimation>().DORestartById("shake");
    }
    #endregion
}

