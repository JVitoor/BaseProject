using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : BaseManager
{
    #region Properties

    [Header(" └─ Manager")]
    public static AudioManager instance;

    [Header(" └─ Music")]
    public AudioClip[] musics;

    public AudioSource musicSource;

    [Header(" └─ SFX")]
    public AudioClip[] sfx;

    public AudioSource sfxSource;
    public AudioSource sfxSourceLooped;

    [Header(" └─ Tools")]
    public AudioMixer mixer;

    #endregion Properties

    #region Unity Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    #endregion Unity Methods

    #region Music Methods

    public void PlayMusic(AudioClip musicClip)
    {
        Debug.Log("Tocando música: " + musicClip.name);
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            Debug.Log("Parando música: " + musicSource.name);
            musicSource.Stop();
        }
    }

    public void SwitchMusic(AudioClip musicClip)
    {
        Debug.Log("Trocando música para: " + musicClip.name);
        StopMusic();
        PlayMusic(musicClip);
    }

    #endregion Music Methods

    #region SFX Methods

    public void PlaySFX(AudioClip clip)
    {
        Debug.Log("Tocando efeito sonoro: " + clip);

        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void StopLoopedSFX()
    {
        sfxSourceLooped.Stop();
    }

    public void PlayLoopedSFX(AudioClip clip)
    {
        Debug.Log("Tocando efeito sonoro em loop: " + clip);

        sfxSourceLooped.clip = clip;
        sfxSourceLooped.Play();
    }

    #endregion SFX Methods

    #region Volume Methods

    private void ChangeVolume(string parameter, float volume, float minLimit)
    {
        mixer.SetFloat(parameter, volume > minLimit ? volume : -80);
    }

    public void ChangeMasterVolume(float volume)
    {
        ChangeVolume("MasterVol", volume, -30);
    }

    public void ChangeMusicVolume(float volume)
    {
        ChangeVolume("MusicVol", volume, -30);
    }

    public void ChangeSFXVolume(float volume)
    {
        ChangeVolume("SFXVol", volume, -20);
    }

    #endregion Volume Methods
}