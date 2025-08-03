using System.Collections;
using UnityEngine;

public class ClockHandMover : MonoBehaviour
{
    public Transform clockHand;
    public float rotationAngle = 90f;
    public float delay = 1f;
    private Coroutine rotateCoroutine;

    private void Awake()
    {
        if (clockHand == null) clockHand = transform;
    }

    void OnEnable()
    {
        StartRotation();
    }

    public void StartRotation()
    {
        if (rotateCoroutine == null)
        {
            rotateCoroutine = StartCoroutine(RotateClockHand());
        }
    }

    IEnumerator RotateClockHand()
    {
        while (true)
        {
            clockHand.Rotate(0, 0, -rotationAngle);
            yield return new WaitForSeconds(delay);
        }
    }

    public void StopRotation()
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }
    }
}
