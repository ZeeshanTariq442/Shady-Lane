using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimerController : MonoBehaviour
{
    public static TimerController TC;

    public float elapsedTime = 1;

    [Header("Events")]
    public UnityEvent<int> onEverySecondElapsed;

    private Coroutine _coroutine;

    public void Awake()
    {
        if (TC != null && TC != this)
        {
            Destroy(gameObject); 
            return;
        }
        TC = this;
        DontDestroyOnLoad(TC);

    }

    private void Start()
    {
        if (_coroutine == null)
        {
            StartEverySecondElapsed();
        }
    }

    private void OnDisable()
    {
        StopCoroutine();
    }

    public void BindEventListeners(bool hasBind, UnityAction<int> callback)
    {
        if (hasBind)
        {
            BindListeners(callback);
        }
        else
        {
            UnbindListeners(callback);
        }
    }

    private void BindListeners(UnityAction<int> callback)
    {
        onEverySecondElapsed.AddListener(callback);
    }

    private void UnbindListeners(UnityAction<int> callback)
    {
        onEverySecondElapsed.RemoveListener(callback);
    }

    public void StartEverySecondElapsed()
    {
        _coroutine = StartCoroutine(EverySecondElapsed());
    }

    public void StopCoroutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;

        }
    }

    private IEnumerator EverySecondElapsed()
    {
        while (true)
        {
            yield return new WaitForSeconds(elapsedTime);
            onEverySecondElapsed?.Invoke((int)elapsedTime);
        }
    }
}

