using UnityEngine;
using DG.Tweening;

public class TutorialHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform hand;
    [SerializeField] private RectTransform text;
    [SerializeField] private RectTransform target;
    [SerializeField] private Vector2 offset;
    [SerializeField] private GameObject tutorialItems;

    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private bool loop = true;

    private Vector2 originalPosition;
    private Tween moveTween;
    private Tween textTween;
    private Vector3 originalTextScale;

    void Start()
    {
        if (hand == null) 
            hand = GetComponent<RectTransform>();
        else
        originalPosition = hand.anchoredPosition;

        if (text != null) originalTextScale = text.localScale;

    }

    public void PlayHandToTarget()
    {
        if (target != null)
        {
            if (moveTween != null && moveTween.IsActive()) return;

            if (text != null) text.gameObject.SetActive(true);

            // Hand move animation
            moveTween = hand.DOAnchorPos(target.anchoredPosition + offset, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(loop ? -1 : 2, LoopType.Yoyo)
                .SetUpdate(true);
        }
       


        // Text scale pulse animation
        if (text != null)
        {
            textTween = text.DOScale(originalTextScale * 1.3f, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(loop ? -1 : 2, LoopType.Yoyo)
                .SetUpdate(true);
        }

        tutorialItems.gameObject.SetActive(true);
    }

    public void StopAnimation()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
            moveTween = null;
            hand.anchoredPosition = originalPosition;
            tutorialItems.gameObject.SetActive(false);
        }

        if (textTween != null)
        {
            textTween.Kill();
            textTween = null;
            text.localScale = originalTextScale;
            text.gameObject.SetActive(false);
        }
    }
}
