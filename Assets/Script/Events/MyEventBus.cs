using System;
using System.Collections.Generic;
public static class MyEventBus
{
    static Dictionary<GameEventType, Action> eventAction = new Dictionary<GameEventType, Action>();
    private static Dictionary<GameEventType, Delegate> eventActions = new Dictionary<GameEventType, Delegate>();
    public static void RaiseEvent(GameEventType key)
    {
        if (eventAction.TryGetValue(key, out Action action))
        {
            action?.Invoke();
        }
    }
    public static void RaiseEvent<T>(GameEventType key, T data)
    {
        if (eventActions.TryGetValue(key, out var action))
        {
            (action as Action<T>)?.Invoke(data);
        }
    }
    public static void SubscribeEvent(GameEventType key, Action action)
    {
        if (!eventAction.ContainsKey(key))
        {
            eventAction.Add(key, action);
        }
        else
        {
            eventAction[key] += action;
        }
    }

    public static void UnSubscribeEvent(GameEventType key, Action action)
    {
        if (eventAction.ContainsKey(key))
        {
            eventAction[key] -= action;
        }

    }
    public static void SubscribeEvent<T>(GameEventType key, Action<T> action)
    {
        if (!eventActions.ContainsKey(key))
        {
            eventActions[key] = null;
        }

        eventActions[key] = (Action<T>)eventActions[key] + action;
    }

    public static void UnSubscribeEvent<T>(GameEventType key, Action<T> action)
    {
        if (eventActions.ContainsKey(key))
        {
            eventActions[key] = (Action<T>)eventActions[key] - action;

            if (eventActions[key] == null)
            {
                eventActions.Remove(key);
            }
        }
    }
}

public enum GameEventType
{
    GameStart,
    LevelFailed,
    TaskComplete,
    ShowNextChapter,
    RestartChapter,
    StarsShow,
    Pause,
    CameraShake,
    CompleteTurnEffect,
    ShakeBottle,
    Undo,
    MoveBasket,
    PlayingMergeAnimation,
    LoadCompletePanel,
    UsedLifeBooster,
    UpdateLifeBooster,
    CancelLifeBooster,
    UsedHintBooster,
    UpdateHintBooster,
    CancelHintBooster,
    RefillLife
}