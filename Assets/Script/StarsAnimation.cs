using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StarsAnimation : MonoBehaviour
{
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public Button next,home,retry;
    private void OnEnable()
    {
        MyEventBus.SubscribeEvent<int>(GameEventType.StarsShow, Show);
    }
    private void OnDisable()
    {
        MyEventBus.UnSubscribeEvent<int>(GameEventType.StarsShow, Show);
    }
    void Show(int show)
    {
        StartCoroutine(StarAni(show));
    }
    IEnumerator StarAni(int show)
    {
        yield return new WaitForSeconds(1f);

        if (show >= 1)
        {
            if (SoundManager.instance != null) SoundManager.instance.PlayOneShot(SoundManager.instance.Star1);
            star1.transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        if (show >= 2)
        {
            if (SoundManager.instance != null) SoundManager.instance.PlayOneShot(SoundManager.instance.Star2);
            star2.transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        if (show >= 3)
        {
            if (SoundManager.instance != null) SoundManager.instance.PlayOneShot(SoundManager.instance.Star3);
            star3.transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        home.interactable = true;
        next.interactable = true;
        retry.interactable = true;
    }


}
