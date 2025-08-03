using Unity.Advertisement.IosSupport.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button setting_button;

    private void Start()
    {
        SoundManager.instance?.PlayMusic(SoundManager.instance?.music_lite);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!AdsController.Instance.IsAdLoaded(AdTypeBO.adUnitType.MainBanner))
                AdsController.Instance?.ShowBanner();
            else
                AdsController.Instance?.ShowAd(AdTypeBO.adUnitType.MainBanner);
         
        }

        InactivityDetector detector = FindFirstObjectByType<InactivityDetector>();

        if (detector == null) return;
        detector.OnInactivityDetected = () =>
        {
            DisableButton(false);
            FindFirstObjectByType<TutorialHand>().PlayHandToTarget();
        };
    }
    public void Play()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void DisableButton(bool enable)
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
            setting_button.enabled = enable;
    }
    private void OnDisable()
    {
        SoundManager.instance?.StopMusic();
    }
}
