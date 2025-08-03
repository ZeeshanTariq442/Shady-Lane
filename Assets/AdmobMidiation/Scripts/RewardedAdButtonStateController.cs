using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
namespace Unity.Advertisement.IosSupport.Samples {
    public class RewardedAdButtonStateController : MonoBehaviour
    {
        public Button AdButton;
        public GameObject Button;
        public AdTypeBO.adUnitType RewardedAdType = AdTypeBO.adUnitType.LevelEndRewardedVideo;
      //  bool IsAdAvailable = false;
        Tween ButtonTween;
        public float Scale = 1.1f;
        public float AnimTime = 0.2f;

        public int loopCount = -1;
        public bool usedAnimation;

        public BoolEvent onIntractableFalse;
        public bool usedOnlyEvent;

        private Coroutine checkAdCoroutine;

        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        public void OnEnable()
        {
            UpdateStatusOfTweening(AdsController.Instance.IsAdLoaded(RewardedAdType));
            checkAdCoroutine = StartCoroutine(CheckAdAvailabilityLoop());
        }
        public void OnDisable()
        {
            UpdateStatusOfTweening(false);
            if (checkAdCoroutine != null)
                StopCoroutine(checkAdCoroutine);
        }
        void UpdateStatusOfTweening(bool ShouldTween)
        {
            if (!usedAnimation) return;
            if (ShouldTween)
            {
                ButtonTween = Button.transform.DOScale(Scale, AnimTime).SetEase(Ease.Linear).SetLoops(loopCount, LoopType.Yoyo);
            }
            else
            {
                if (ButtonTween != null)
                {
                    ButtonTween.Kill();
                    Button.transform.localScale = new Vector3(1, 1, 1);
                }
            }

        }
        //public void Update()
        //{
        //    if (AdsController.Instance.IsAdLoaded(RewardedAdType) != AdButton.interactable)
        //    {
        //        IsAdAvailable = AdsController.Instance.IsAdLoaded(RewardedAdType);
        //        AdButton.interactable = IsAdAvailable;
        //        UpdateStatusOfTweening(IsAdAvailable);
        //    }
        //}

        IEnumerator CheckAdAvailabilityLoop()
        {
            while (true)
            {
                bool isAvailable = AdsController.Instance.IsAdLoaded(RewardedAdType);
                if (usedOnlyEvent)
                {
                    onIntractableFalse?.Invoke(isAvailable);
                }
                else if (isAvailable != AdButton.interactable)
                {
                    AdButton.interactable = isAvailable;
                    UpdateStatusOfTweening(isAvailable);
                }
              

                yield return new WaitForSeconds(2f); // Check every 1 second 
            }
        }

    }
}
