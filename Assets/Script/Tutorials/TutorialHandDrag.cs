using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialHandDrag : MonoBehaviour
{
    public Transform hand;
    public SpriteRenderer bg;
    public Text msg;
    public GameObject tutorialPanel;


    private Transform item;
    private Transform item2;
    private Transform slab;

    public float moveDuration = 1f;
    public float waitTime = 0.5f;
    public bool loop = true;

    private Vector3 handStartPos;
    private Vector3 itemStartPos;

    private Sequence tutorialSequence;
    public bool isNeg;

    private string[] tutorialMsg = {"Tap and hold the item.","Drag it to the slab area","Release it","Great" };

    private void Awake()
    {
        if (PlayerPrefsHandler.GetInt(PlayerPrefsHandler.NewTutorialPrefKey, 0) != 0)
        {
            tutorialPanel.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
      
        msg.text = tutorialMsg[0];
        tutorialPanel.gameObject.SetActive(true);
    }
    public void Init(Transform item, Transform slab, Transform item2)
    {
        hand.gameObject.SetActive(false);
        this.item = item;
        this.item2 = item2;
        this.slab = slab;
        Invoke(nameof(StartTutorial),2);
    }
    public void DisableTutorial(bool active)
    {
        hand.gameObject.SetActive(active);
    }
    public void OnCompleteTutorial()
    {
        hand.gameObject.SetActive(false);
        ChangeMsg(3);
        msg.transform.DOScale(1.5f, 2);
        item2.GetComponent<ItemController>().HandleState();
        tutorialPanel.transform.DOScale(Vector3.zero, 0).SetDelay(2).OnComplete(() =>
        {
            tutorialPanel.SetActive(false);
            this.gameObject.SetActive(false);
            tutorialPanel.transform.DOKill();
        });
      
    }
    public void ChangeMsg(int index = 0)
    {
        msg.text = tutorialMsg[index];
    }
    private void StartTutorial()
    {
        Vector3 offsetForItem = new Vector3(0.3f, -0.3f, 0); // Customize offset (e.g. above and to the right)
        Vector3 offsetForSlab = new Vector3(0.3f, 0, 0); // Customize offset (e.g. above and to the right)

        handStartPos = item.position + offsetForItem;

        hand.transform.position = handStartPos;
        itemStartPos = item.position;
        hand.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        hand.gameObject.SetActive(true);

        if (tutorialSequence != null && tutorialSequence.IsActive())
        {
            tutorialSequence.Kill();
        }

        tutorialSequence = DOTween.Sequence();

        // Move to item + offset
        tutorialSequence.Append(hand.DOMove(item.position + offsetForItem, moveDuration).SetEase(Ease.InOutSine));

        tutorialSequence.AppendInterval(waitTime);

        // Move to slab + offset
        tutorialSequence.Append(hand.DOMove(slab.position + offsetForSlab, moveDuration).SetEase(Ease.InOutSine));

        tutorialSequence.AppendInterval(waitTime);
        if (loop)
        {
            tutorialSequence.SetLoops(-1, LoopType.Restart);
        }

        bg.DOFade(1f, 5).SetDelay(4).SetEase(Ease.Linear).OnStart(()=>
        {
            item.GetComponent<SpriteRenderer>().sortingOrder = 13;
        });
    }


    private void OnDisable()
    {
        if (tutorialSequence != null && tutorialSequence.IsActive())
        {
            tutorialSequence.Kill();
        }
    }

    private void OnDestroy()
    {
        if (tutorialSequence != null && tutorialSequence.IsActive())
        {
            tutorialSequence.Kill();
        }
    }
}
