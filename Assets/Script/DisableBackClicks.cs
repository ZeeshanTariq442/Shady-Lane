using UnityEngine;

public class DisableBackClicks : MonoBehaviour
{

    private void OnEnable()
    {
        GameManager.Instance.IsGameOver = true;
    }

    private void OnDisable()
    {
        GameManager.Instance.IsGameOver = false;
    }
}
