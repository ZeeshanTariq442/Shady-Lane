using Unity.Advertisement.IosSupport.Samples;
using UnityEngine;

public class Panel : MonoBehaviour
{
  
    void OnEnable()
    {
       // CallInterstetialAd();
    }
    private void CallInterstetialAd()
    {
        AdsController.Instance.SelectedRewardedAd = AdsController.SelectedRewardedAdType.none;
        AdsController.Instance.ShowAd(AdTypeBO.adUnitType.LevelEndInterstetial);
    }

}
