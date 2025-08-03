using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class InactivityDetector : MonoBehaviour
{
    public float inactivityThreshold = 5f; // 5 seconds
    private float inactivityTimer = 0f;

    public Action OnInactivityDetected;

    void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            inactivityTimer = 0f;
        }
        else
        {
            inactivityTimer += Time.deltaTime;

            if (inactivityTimer >= inactivityThreshold)
            {
                inactivityTimer = -999f;
                OnInactivityDetected?.Invoke();
            }
        }
    }

    private void OnEnable()
    {
        inactivityTimer = 0;
    }

 
}
