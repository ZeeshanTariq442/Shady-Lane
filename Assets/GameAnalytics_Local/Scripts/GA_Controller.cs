using GameAnalyticsSDK;
using UnityEngine;
using System.Collections.Generic;

public class GA_Controller : MonoBehaviour
{
    
    public static GA_Controller Instance;
    
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if(IsValidToLogEvent())
        {            
            GameAnalytics.Initialize();
        }
    }

    public void LogBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, string receipt, string signature=null)
    {
        if(IsValidToLogEvent())
        {
#if UNITY_ANDROID
            GameAnalytics.NewBusinessEventGooglePlay( currency,  amount,  itemType,  itemId,  cartType,  receipt,  signature);
 #endif
#if UNITY_IPHONE
            GameAnalytics.NewBusinessEventIOS( currency,  amount,  itemType,  itemId,  cartType,  receipt);
#endif


        }

    }
    public void LogResourceEvent(GAResourceFlowType flowType,string currency,float amount,string itemType,string itemID)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewResourceEvent(flowType, currency, amount, itemType, itemID);
        }

        //SomeExamples
        //GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSource, “Gems”, 400, “IAP”, “Coins400”);
        //GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “Gems”, 400, “Weapons”, “SwordOfFire”);
        //GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “Gems”, 100, “Boosters”, “BeamBooster5Pack”);
        //GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSource, “BeamBooster”, 5, “Gems”, “BeamBooster5Pack”);
        //GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “BeamBooster”, 3, “Gameplay”, “BeamBooster5Pack”);
    }
    public void LogProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewProgressionEvent(progressionStatus, progression01, progression02, progression03, score);
        }
    }
    public void LogProgressionEvent(GAProgressionStatus progressionStatus, string progression01)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewProgressionEvent(progressionStatus, progression01);
        }
    }
    public void LogProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewProgressionEvent(progressionStatus, progression01, progression02);
        }
    }
    public void LogProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewProgressionEvent(progressionStatus, progression01, progression02, progression03);
        }
    }
    public void LogProgressionEvent(GAProgressionStatus progressionStatus, string progression01, int score)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewProgressionEvent(progressionStatus, progression01, score);
        }
    }
    public void LogProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, int score)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewProgressionEvent(progressionStatus, progression01, progression02, score);
        }
    }
    public void LogDesignEvent(string eventName, float eventValue)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewDesignEvent(eventName, eventValue);
        }

    }
    public void LogDesignEvent(string eventName)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewDesignEvent(eventName);
        }

    }
    public void LogErrorEvent(GAErrorSeverity severity, string message)
    {
        if(IsValidToLogEvent())
        {
            GameAnalytics.NewErrorEvent(severity, message);
        }
    }
    public void NewAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement, long duration)
    {
        if (IsValidToLogEvent())
        {
            GameAnalytics.NewAdEvent(adAction, adType, adSdkName, adPlacement, duration, null, false);
        }
        
    }

    public void NewAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement, long duration, IDictionary<string, object> customFields, bool mergeFields = false)
    {
        if (IsValidToLogEvent())
        {
            GameAnalytics.NewAdEvent(adAction, adType, adSdkName, adPlacement, duration, customFields, mergeFields);
        }
        
    }

    public void NewAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement, GAAdError noAdReason)
    {
        if (IsValidToLogEvent())
        {
            GameAnalytics.NewAdEvent(adAction, adType, adSdkName, adPlacement, noAdReason, null, false);
        }
        
    }

    public void NewAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement, GAAdError noAdReason, IDictionary<string, object> customFields, bool mergeFields = false)
    {
        if (IsValidToLogEvent())
        {
            GameAnalytics.NewAdEvent(adAction, adType, adSdkName, adPlacement, noAdReason, customFields, mergeFields);
        }
        
    }

    public void NewAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement)
    {
        if (IsValidToLogEvent())
        {
            GameAnalytics.NewAdEvent(adAction, adType, adSdkName, adPlacement, null, false);
        }
       
    }

    public void NewAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement, IDictionary<string, object> customFields, bool mergeFields = false)
    {
        if (IsValidToLogEvent())
        {
            GameAnalytics.NewAdEvent(adAction, adType, adSdkName, adPlacement, customFields, mergeFields);
        }
        
    }
    public bool IsSDKInitialized()
    {
        return GameAnalytics.Initialized;
    }
    bool IsValidToLogEvent()
    {
        if (!Application.isEditor && SDKCommons.Instance!=null && SDKCommons.Instance.Game_Mode == SDKCommons.GameMode.Production)
        {
            return true;
        }

        return false;
    }

}
