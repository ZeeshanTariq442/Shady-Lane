using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Unity.VisualScripting;

public class CountdownTimer : MonoBehaviour
{
    #region Fields
    public Text timerText;
    public float countdownDuration = 30f;
    public bool RestartWithRemainingTime;
    private float remainingTime = 0;
    private bool stopTimer = true;
    private int currentLevel;
    #endregion

    void Start() => stopTimer = false;

    private void OnEnable()
    {
        if(GameManager.Instance != null)
        {
            currentLevel = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentActiveLevelKey, 0);
            countdownDuration = GameManager.Instance.AllLevels.levels[currentLevel].countdownTime;
            countdownDuration *= 60;
        }
            

        LoadTimer();
        StartTimer();

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
            Debug.Log("Timer Finished!");
            // timerText.text = "Timer Finished!";
            timerText.text = "00:00";
            FindAnyObjectByType<UIHandler>().ShowFailedPanel();
            StopTimer();
        }

    }
    public void StopTimer()
    {
        FindAnyObjectByType<ClockHandMover>()?.StopRotation();
        FindAnyObjectByType<LifeRefillTimer>()?.StopTimer();
        stopTimer = true;
    }
    public void ResumeTimer()
    {
        FindAnyObjectByType<ClockHandMover>()?.StartRotation();
        FindAnyObjectByType<LifeRefillTimer>()?.ResumeTimer();
        stopTimer = false;
    }
    private IEnumerator PlayTimer()
    {
        while (remainingTime > 0)
        {
            remainingTime -= 1;
            UpdateTimerUI();
            Debug.Log(Time.deltaTime);
            yield return new WaitForSeconds(1);
        }
        remainingTime = 0;
        Debug.Log("Timer Finished!");
        timerText.text = "Timer Finished!";
    }
    void UpdateTimerUI()
    {
        TimeSpan time = TimeSpan.FromSeconds(remainingTime);
        if (timerText == null || time == null)
            return;

        if (time.Hours >= 1)
            timerText.text = string.Format("{0:D1}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        else
            timerText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }

    void LoadTimer()
    {
        // Get the last saved time
        long lastSavedTime = long.Parse(PlayerPrefsHandler.GetString(PlayerPrefsHandler.LastSavedTime + currentLevel, "0"));
        float savedRemainingTime = PlayerPrefsHandler.GetFloat(PlayerPrefsHandler.RemainingTime + currentLevel, countdownDuration);

        if (lastSavedTime != 0)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            float timeDifference = currentTime - lastSavedTime;

            if (!RestartWithRemainingTime)
                remainingTime = savedRemainingTime - timeDifference;
            else
                remainingTime = savedRemainingTime;

        }
        else
        {
            remainingTime = countdownDuration;
        }
    }

    void StartTimer()
    {
        InvokeRepeating(nameof(SaveTimerData), 1f, 1f);
    }

    void SaveTimerData()
    {
        PlayerPrefsHandler.SetString(PlayerPrefsHandler.LastSavedTime + currentLevel, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        PlayerPrefsHandler.SetFloat(PlayerPrefsHandler.RemainingTime + currentLevel, remainingTime);
        PlayerPrefs.Save();
    }
    public void ResetTimer()
    {
       // Debug.Log("Timer Reset!");
        PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.LastSavedTime + currentLevel);
        PlayerPrefsHandler.DeleteKey(PlayerPrefsHandler.RemainingTime + currentLevel);
        currentLevel = PlayerPrefsHandler.GetInt(PlayerPrefsHandler.CurrentActiveLevelKey, 0);
        countdownDuration = GameManager.Instance.AllLevels.levels[currentLevel].countdownTime;
        countdownDuration *= 60;
        remainingTime = countdownDuration;
        SaveTimerData();
        UpdateTimerUI();
    }
    void OnApplicationQuit()
    {
        SaveTimerData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
       //   if (pauseStatus) SaveTimerData();
    }

    public void IncreaseTimer(int minutes)
    {
      //  countdownDuration = GameManager.Instance.AllLevels.levels[currentLevel].countdownTime;
     //   countdownDuration *= 60;
        remainingTime = 60 * minutes;
        SaveTimerData();
    }
}
