using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CharacterEyeAnimation : MonoBehaviour
{
    public Transform LeftEyelid; 
    public Transform RightEyelid; 
    public float blinkDuration = 0.1f;
    public float blinkInterval = 3f;

    void Start()
    {
        ScheduleNextBlink();
    }

    void BlinkLeft()
    {
        LeftEyelid.DOScaleY(1f, blinkDuration) // Open
                   .SetEase(Ease.InOutQuad)
                   .OnComplete(() =>
                       LeftEyelid.DOScaleY(0.1f, blinkDuration)); // Close
    }
    void BlinkRight()
    {
        RightEyelid.DOScaleY(1f, blinkDuration) // Open
                   .SetEase(Ease.InOutQuad)
                   .OnComplete(() => {
                       RightEyelid.DOScaleY(0.1f, blinkDuration); 
                   }); // Close
     
    }
    void ScheduleNextBlink()
    {
        float randomInterval = Random.Range(blinkInterval, blinkInterval + 2);
        InvokeRepeating(nameof(BlinkLeft), randomInterval, randomInterval);
        InvokeRepeating(nameof(BlinkRight), randomInterval, randomInterval);
    }
}
