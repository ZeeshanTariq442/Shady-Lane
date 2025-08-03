using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaDetector : MonoBehaviour
{
    RectTransform rectTransform;
    Rect SafeArea;
    Vector2 minAnchor;
    Vector2 maxAnchor;
    public float MaxYAnchorOffset = 0.2f;
    bool HasNotch = true;
    Vector3 changePos;
    public Transform gamePlayParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SafeArea = Screen.safeArea;

        minAnchor = SafeArea.position;
        maxAnchor = minAnchor + SafeArea.size;
        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;

     
        //Debug.LogError(maxAnchor.x+ " " + maxAnchor.y);
        //Debug.LogError(Screen.width + " " + Screen.height);
        if (maxAnchor.x == Screen.width && maxAnchor.y == Screen.height)
        {
            HasNotch = false;
        }
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        if (HasNotch)
        {
            maxAnchor = maxAnchor + new Vector2(0, MaxYAnchorOffset);
#if UNITY_IPHONE
 minAnchor = minAnchor - new Vector2(minAnchor.x, MaxYAnchorOffset);
#endif
        }
        rectTransform.anchorMax = maxAnchor;
        rectTransform.anchorMin = minAnchor;

       // SetGamePlayParentPosition();
    }
    public void SetGamePlayYPosition()
    {
        if (gamePlayParent == null) return;
        gamePlayParent.transform.position = Vector3.zero;

        if (changePos == Vector3.zero)
        changePos = new Vector3(0, -((float)((Screen.height - SafeArea.height) / 3f) / 100f), 0);

        if (changePos.y < -0.45f) changePos.y = -0.45f;

        gamePlayParent.transform.DOMoveY(changePos.y, 0.5f).SetDelay(1).SetEase(Ease.OutBounce);
    }

    private void SetGamePlayParentPosition()
    {
        if (gamePlayParent == null) return;
        changePos = new Vector3(0, -((float)((Screen.height - SafeArea.height) / 3f) / 100f), 0);
        gamePlayParent.transform.DOMoveY(changePos.y,0.5f).SetDelay(2f).SetEase(Ease.OutBounce);
    }
}
