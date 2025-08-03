using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SettingHandler : MonoBehaviour
{
    [Header("Panels")]
    public GameObject SettingPanel;
    public GameObject TermPanel;
    public GameObject PrivacyPanel;
    public GameObject SupportPanel;

    [Header("Slider")]
    public Slider musicSlider;
    public Slider soundSlider;

    [Header("Settings Panel Buttons")]
    public Button SettingButton;
    public Button SettingCloseButton;

    [Header("Terms and Privacy Buttons")]
    public Button TermButton;
    public Button PrivacyButton;

    [Header("Support Buttons")]
    public Button SupportButton;

    [Header("Accept/Decline Buttons")]
    public Button TermDeclineButton;
    public Button PrivacyDeclineButton;
    public Button TermAcceptButton;
    public Button PrivacyAcceptButton;

    private void Start()
    {
        SettingButton?.onClick.AddListener(() => OpenSettingPanel());
        SettingCloseButton?.onClick.AddListener(() => CloseSettingPanel());
        TermButton?.onClick.AddListener(() => OnTermsOfServiceButtonPress());
        PrivacyButton?.onClick.AddListener(() => OnPrivacyButtonPress());
        SupportButton?.onClick.AddListener(() => OnSupportButtonPress());

        TermDeclineButton?.onClick.AddListener(() => TermDecline());
        PrivacyDeclineButton?.onClick.AddListener(() => PrivacyDecline());
        TermAcceptButton?.onClick.AddListener(() => TermAccept());
        PrivacyAcceptButton?.onClick.AddListener(() => PrivacyAccept());
    }

    public void OnSupportButtonPress()
    {

        string DeviceID = SystemInfo.deviceUniqueIdentifier;
        string email = "contact@infinityup.org";
        string subject = MyEscapeURL("Support for " + DeviceID);
        string body = MyEscapeURL("Please don't remove this to serve you quickly {" + DeviceID + "}");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        GA_Controller.Instance.LogDesignEvent("MainMenu:Support");
    }

    public void OnTermsOfServiceButtonPress()
    {
        AdsController.Instance.GDPRUIPanel.OpenPanel(true);
        GA_Controller.Instance.LogDesignEvent("TermsofService:Show:Manual");
        GA_Controller.Instance.LogDesignEvent("MainMenu:TermsOfService");
    }

    public void OnPrivacyButtonPress()
    {
        Application.OpenURL("https://infinityup.org/privacy-policy/");
        GA_Controller.Instance.LogDesignEvent("MainMenu:PrivacyPolicy");
    }

    static string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    private void OpenPanel(GameObject panelToOpen)
    {
        PlaySound();
        if (panelToOpen == null) return;
        if (TermPanel != null) TermPanel.SetActive(panelToOpen == TermPanel);
        if (PrivacyPanel != null) PrivacyPanel.SetActive(panelToOpen == PrivacyPanel);
        if (SupportPanel != null) SupportPanel.SetActive(panelToOpen == SupportPanel);
        Invoke(nameof(PlayPanelOpenSound),0.5f);
    }

    private  void PlaySound()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.click);
    }
    private void PrivacyAccept()
    {
        PlaySound();
    }
    private void TermDecline()
    {
        PlaySound();
    }
    private void TermAccept()
    {
        PlaySound();
    }
    private void PrivacyDecline()
    {
        PlaySound();
    }

    #region SETTING
    public void CloseSettingPanel()
    {
      //  Time.timeScale = 1;
        PlaySound();
        SetSettingValue();
        SettingPanel.SetActive(false);
    }
    public void OpenSettingPanel()
    {
      //  Time.timeScale = 0;
        PlaySound();
        SetSettingValue();
        SettingPanel.SetActive(true);
        Invoke(nameof(PlayPanelOpenSound), 0.5f);
    }
    public void PlayPanelOpenSound()
    {
        SoundManager.instance?.PlayOneShot(SoundManager.instance.move);
    }
    private void SetSettingValue()
    {
        float savedMusicVolume = PlayerPrefsHandler.GetFloat(PlayerPrefsHandler.MusicKey, 1);
        float savedSoundVolume = PlayerPrefsHandler.GetFloat(PlayerPrefsHandler.SoundKey, 1);
        musicSlider.value = savedMusicVolume;
        soundSlider.value = savedSoundVolume;
    }
    public void UpdateMusic(float value)
    {
        SoundManager.instance?.UpdateMusicVolume(value);
    }
    public void UpdateSound(float value)
    {
        SoundManager.instance?.UpdateSoundVolume(value);
    }
    #endregion
}
