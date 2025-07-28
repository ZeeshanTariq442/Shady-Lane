using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class movementsound : MonoBehaviour
{
    public GameObject soundbutton;
    public static movementsound ins;
   // public AudioSource audioSource;
 //   public AudioClip audioClip;

    public GameObject musicSourcee;

    public AudioSource musicSource;
    public AudioClip musicClip;
    public AudioClip punch;

    

    // Start is called before the first frame update
    void Start()
    {
        ins = this;
        //  StartCoroutine(playAudioSequentially());
        //musicSource.clip = musicClip;
        //musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //if( gamecontrol4players.ins.playSound == true)
        //{
        //     playMovmentSound();
        //}
        // else
        // {
        //     StopMovmentSound();
        // }
        //if(gamecontrol4players.ins.fight == true)
        // {
        //     audioSource.clip = punch;
        //     audioSource.Play();
        // }
        if (soundbutton.GetComponent<Toggle>().isOn == true)
        {
            
            musicSource.UnPause();
            if (musicSource.isPlaying)
            {
                
            }
            //musicSource.enabled = true;



        }
        else
        {
            
            musicSource.Pause();
            if (!musicSource.isPlaying)
            {
               
            }
            //musicSource.enabled = false;

        }
    }
    //public void playMovmentSound()
    //{
    //    audioSource.clip = audioClip;
    //    audioSource.Play();
    //}
    //public void StopMovmentSound()
    //{
    //    audioSource.clip = audioClip;
    //    audioSource.Stop();
    //}
}
