using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RateUs : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject RateUsPanel;       
    public Button rateNowButton;   
    public Button laterButton;      
    public GameObject[] RateUsStars;    

    private int selectedRating = 0;

    void Start()
    {

        // Add listeners to buttons
        rateNowButton.onClick.AddListener(RateNow);
        laterButton.onClick.AddListener(RateUsClose);

        // Add listeners for star selection
        for (int i = 0; i < RateUsStars.Length; i++)
        {
            int index = i + 1;
            RateUsStars[i].transform.parent.GetComponent<Button>().onClick.AddListener(() => UserGiveStars(index));
            RateUsStars[i].GetComponent<Button>().onClick.AddListener(() => UserGiveStars(index));
        }
    }
    public void ClosePanel()
    {
        RateUsPanel.SetActive(false);
    }
    public void RateUsClose()
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayOneShot(SoundManager.instance.click);

        ClosePanel();

        PlayerPrefsHandler.SetString(PlayerPrefsHandler.RatePrefKey, "no");

        for (int i = 0; i < RateUsStars.Length; i++)
        {
            RateUsStars[i].SetActive(false);
        }
    }
    public void UserGiveStars(int j)
    {
        selectedRating = j;
        if (SoundManager.instance != null) SoundManager.instance.PlayOneShot(SoundManager.instance.click);
        for (int i = 0; i < j; i++)
        {
            RateUsStars[i].SetActive(true);
        }
        for (int i = j; i < RateUsStars.Length; i++)
        {
            RateUsStars[i].SetActive(false);
        }
    }
    private void RateNow()
    {
        if (selectedRating == 0)
        {
            Debug.LogWarning("Please select a rating before submitting.");
            return;
        }

        RateUsPanel.SetActive(false);
        // Save that the user has rated the game
        PlayerPrefsHandler.SetString(PlayerPrefsHandler.RatePrefKey, "yes");
        PlayerPrefs.Save();

        // Open the app store page
        Application.OpenURL("https://play.google.com/store/apps/details?id=your.game.package");

    }

    private void NeverShowAgain()
    {
        PlayerPrefsHandler.SetString(PlayerPrefsHandler.RatePrefKey, "yes");
        PlayerPrefs.Save();
    }

   
}
