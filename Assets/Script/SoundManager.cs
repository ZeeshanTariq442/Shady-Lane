using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip click;
    public AudioClip correct;
    public AudioClip Star1;
    public AudioClip Star2;
    public AudioClip Star3;
    public AudioClip move;
    public AudioClip item_move;
    public AudioClip wrong_item_move;
    public AudioClip item_destroy;
    public AudioClip complete;
    public AudioClip failed;


    public AudioClip music;
    public AudioClip music_lite;



    public AudioSource soundSource;
    public AudioSource musicSource;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        // Load the saved volume values
        float savedMusicVolume = PlayerPrefsHandler.GetFloat(PlayerPrefsHandler.MusicKey, 1);
        float savedSoundVolume = PlayerPrefsHandler.GetFloat(PlayerPrefsHandler.SoundKey, 1);
        // Set the initial volume 
        musicSource.volume = savedMusicVolume;
        soundSource.volume = savedSoundVolume;
    }

    public void PlayOneShot(AudioClip clip)
    {
        if(soundSource.clip != clip || soundSource.clip == null)
        soundSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource?.Stop();

    }
    public void Vibrate()
    {
        if (PlayerPrefsHandler.GetString(PlayerPrefsHandler.VibrateKey,"ON") == "ON")
        {
            Handheld.Vibrate();
        }

    }

    #region Update Sound & Music
    public void UpdateMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            PlayerPrefsHandler.SetFloat(PlayerPrefsHandler.MusicKey, volume);
        }
    }
    public void UpdateSoundVolume(float volume)
    {
        
        if (soundSource != null)
        {
            soundSource.volume = volume;
            PlayerPrefsHandler.SetFloat(PlayerPrefsHandler.SoundKey, volume);
        }
    }
    #endregion
}
