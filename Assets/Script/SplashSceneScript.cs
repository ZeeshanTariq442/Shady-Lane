using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashSceneScript : MonoBehaviour
{
    public Image Bar;
    public float fillSpeed = 0.5f;

    private void Awake()
    {
        PlayerPrefsHandler.SetInt(PlayerPrefsHandler.TutorialPrefKey, 1);
       // PlayerPrefsHandler.SetInt(PlayerPrefsHandler.HintTutorial, 1);
    }
    private void Update()
    {
        Bar.fillAmount = Mathf.MoveTowards(Bar.fillAmount, 1f, fillSpeed * Time.deltaTime);
        if (Bar.fillAmount >= 1f)
        {
            SoundManager.instance?.Vibrate();
            LoadMenu();
        }
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(1); 
    }
}
