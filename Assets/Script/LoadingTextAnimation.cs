using UnityEngine;
using TMPro; 
using System.Collections;

public class LoadingTextAnimation : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public float dotSpeed = 0.5f; 

    private string baseText = "Loading";
    private int dotCount = 0;

    private void OnEnable()
    {
        StartCoroutine(AnimateDots());
    }

    private void OnDisable()
    {
        StopCoroutine(AnimateDots());
    }

    private IEnumerator AnimateDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4;
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(dotSpeed);
        }
    }
}
