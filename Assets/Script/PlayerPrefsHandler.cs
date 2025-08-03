using UnityEngine;

public static class PlayerPrefsHandler
{
    // For Setting
    #region KEYS
    public const string SoundKey = "sound";
    public const string MusicKey = "music";
    public const string VibrateKey = "vibrate";

    public const string CurrentLevelKey = "currentlevel";
    public const string CurrentLevelStarKey = "currentlevelstars";
    public const string CurrentActiveLevelKey = "currentactivelevel";
    public const string CurrentTaskKey = "currenttask";
    public const string UsedTrialKey = "UsedTrialCount";
    public const string TotalTrialKey = "TotalTrailCount";
    public const string ITrialKey = "ITrailCount";
    public const string SlotExposeBoosterKey = "SlotExposeKey";
    public const string HintBooster = "HintBooster";
    public const string HintTutorial = "HintTutorial";
    public const string ItemCounter = "itemcounter";
    public const string GameComplete = "gamecomplete";
    public const string BasketProgress = "bp";
    public const string BasketItemProgress = "bip";
    public const string Coins = "coins";
    public const string ChapterComplete = "ChapterComplete";
    public const string LastSavedTime = "LastSavedTime";
    public const string RemainingTime = "RemainingTime";
    public const string AdsRemovedKey = "AdsRemovedKey";

    public const string RatePrefKey = "RatePrefKey";
    public const string TutorialPrefKey = "TutorialPrefKey";
    public const string NewTutorialPrefKey = "NewTutorialPrefKey";
    #endregion

    public static int GetInt(string key, int defaultValue = 0)
    {
        if (key == SlotExposeBoosterKey || key == HintBooster) defaultValue = 2;
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
