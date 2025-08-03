using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class HandIndicator : MonoBehaviour
{
    public RectTransform handImage; 
    public float moveSpeed = 1.5f;

    private Vector3 targetPosition;

    public void ShowHandAt(Vector3 position)
    {
        gameObject.SetActive(true);
        targetPosition = position;
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        targetPosition += new Vector3(.4f,0.5f,0);
        while (Vector3.Distance(handImage.position, targetPosition) > 0.1f)
        {
            handImage.position = Vector3.Lerp(handImage.position, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        GetComponent<Image>().enabled = true;
        GetComponent<DOTweenAnimation>().DOPlay();
    }

    public void HideHand()
    {
        gameObject.SetActive(false);
    }

  
}
