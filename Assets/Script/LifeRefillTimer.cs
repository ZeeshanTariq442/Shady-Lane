using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LifeRefillTimer : MonoBehaviour
{
    public GameObject fillGameObject;
    public DOTweenAnimation clockHandAni;
    public TMP_Text timerText;
    public float lifeRefillTime = 300f;
    private float remainingTime = 0;
    private bool stopTimer = true;
    private UIHandler _UIHandler;
    void Start() 
    {
        _UIHandler = FindFirstObjectByType<UIHandler>();
        remainingTime = lifeRefillTime;
        stopTimer = false;
    }

    private void OnEnable()
    {
        MyEventBus.SubscribeEvent(GameEventType.LoadCompletePanel, () => remainingTime = lifeRefillTime);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent(GameEventType.LoadCompletePanel, () => remainingTime = lifeRefillTime);
    }

    void Update()
    {
        if (stopTimer) return;

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            remainingTime = 0;
            fillGameObject.SetActive(true);
            StopTimer();
            RestartTimer();
            //  MyEventBus.RaiseEvent(GameEventType.RefillLife);
            _UIHandler.UpdateLifeBooster(false);
        }

    }
    private void RestartTimer()
    {
        remainingTime = lifeRefillTime;
        ResumeTimer();
    }
    public void StopTimer()
    {
        stopTimer = true;
        clockHandAni.DOPause();
    }
    public void ResumeTimer()
    {
        stopTimer = false;
        clockHandAni.DOPlay();
    }
    void UpdateTimerUI()
    {
        TimeSpan time = TimeSpan.FromSeconds(remainingTime);
        if (time.Hours >= 1)
            timerText.text = string.Format("{0:D1}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        else
            timerText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }

}
