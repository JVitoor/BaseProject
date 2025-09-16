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

    [Header(" └─ Player SFX")]
    [Tooltip("Array de sons de pulo - será sorteado aleatoriamente")]
    public AudioClip[] jumpSFX; // Array de sons de pulo para variação

    [Header(" └─ Planar SFX")]
    public AudioClip glideSFX;

    [Header(" └─ Tools")]
    public AudioMixer mixer;

    #endregion Properties

    #region Unity Methods

    private void Awake()
    {
        Debug.Log("[AudioManager] Awake() chamado!");
        
        if (instance == null)
        {
            instance = this;
            Debug.Log("[AudioManager] Instance inicializada com sucesso!");
        }
        else
        {
            Debug.LogWarning("[AudioManager] Instance já existe, destruindo duplicata!");
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        Debug.Log("[AudioManager] DontDestroyOnLoad aplicado!");
    }

    private void Start()
    {
        Debug.Log("[AudioManager] Start() chamado!");
        
        // Validação dos componentes necessários
        ValidateAudioSources();
    }

    private void ValidateAudioSources()
    {
        if (musicSource == null)
            Debug.LogWarning("[AudioManager] Music Source não está atribuído!");
        
        if (sfxSource == null)
            Debug.LogWarning("[AudioManager] SFX Source não está atribuído!");
        
        if (sfxSourceLooped == null)
            Debug.LogWarning("[AudioManager] SFX Source Looped não está atribuído!");
        
        if (mixer == null)
            Debug.LogWarning("[AudioManager] Audio Mixer não está atribuído!");
        
        if (jumpSFX == null || jumpSFX.Length == 0)
            Debug.LogWarning("[AudioManager] Jump SFX array está vazio!");
        
        Debug.Log($"[AudioManager] Validação concluída. Instance ativo: {instance != null}");
    }

    #endregion Unity Methods

    #region Music Methods

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource == null)
        {
            Debug.LogError("[AudioManager] Music Source não está configurado!");
            return;
        }
        
        Debug.Log("Tocando música: " + musicClip.name);
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            Debug.LogError("[AudioManager] Music Source não está configurado!");
            return;
        }
        
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
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] AudioClip é null!");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogError("[AudioManager] SFX Source não está configurado!");
            return;
        }

        Debug.Log("Tocando efeito sonoro: " + clip.name);
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void StopLoopedSFX()
    {
        if (sfxSourceLooped == null)
        {
            Debug.LogError("[AudioManager] SFX Source Looped não está configurado!");
            return;
        }
        
        sfxSourceLooped.Stop();
    }

    public void PlayLoopedSFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] AudioClip é null!");
            return;
        }

        if (sfxSourceLooped == null)
        {
            Debug.LogError("[AudioManager] SFX Source Looped não está configurado!");
            return;
        }

        Debug.Log("Tocando efeito sonoro em loop: " + clip.name);
        sfxSourceLooped.clip = clip;
        sfxSourceLooped.Play();
    }

    #endregion SFX Methods

    #region Player Specific SFX Methods

    public void PlayJumpSound()
    {
        Debug.Log("[AudioManager] PlayJumpSound() chamado!");
        
        AudioClip selectedSound = GetRandomJumpSound();
        
        if (selectedSound != null)
        {
            Debug.Log($"[AudioManager] Reproduzindo som de pulo: {selectedSound.name}");
            PlaySFX(selectedSound);
        }
        else
        {
            Debug.LogWarning("[AudioManager] Nenhum som de pulo válido disponível!");
        }
    }

    public void PlayGlideSound()
    {
        Debug.Log("[AudioManager] PlayJumpSound() chamado!");

        AudioClip selectedSound = glideSFX;

        if (selectedSound != null)
        {
            Debug.Log($"[AudioManager] Reproduzindo som de pulo: {selectedSound.name}");
            PlayLoopedSFX(selectedSound);
        }
        else
        {
            Debug.LogWarning("[AudioManager] Nenhum som de pulo válido disponível!");
        }
    }

    #endregion Player Specific SFX Methods

    #region Static Helper Methods

    /// <summary>
    /// Método estático para verificar se o AudioManager está disponível
    /// </summary>
    public static bool IsAvailable()
    {
        return instance != null;
    }

    /// <summary>
    /// Método estático para obter a instância do AudioManager de forma segura
    /// </summary>
    public static AudioManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("[AudioManager] Tentando acessar instance que é null!");
            
            // Tenta encontrar o AudioManager na cena
            AudioManager found = FindObjectOfType<AudioManager>();
            if (found != null)
            {
                Debug.LogWarning("[AudioManager] AudioManager encontrado na cena, mas instance não estava definida!");
                instance = found;
            }
            else
            {
                Debug.LogError("[AudioManager] Nenhum AudioManager encontrado na cena!");
            }
        }
        
        return instance;
    }

    #endregion Static Helper Methods

    #region Helper Methods

    /// <summary>
    /// Seleciona aleatoriamente um som de pulo válido do array
    /// </summary>
    /// <returns>AudioClip selecionado ou null se nenhum for válido</returns>
    private AudioClip GetRandomJumpSound()
    {
        // Verifica se o array existe e não está vazio
        if (jumpSFX == null || jumpSFX.Length == 0)
        {
            Debug.LogWarning("[AudioManager] Jump SFX array é null ou vazio!");
            return null;
        }

        // Filtra apenas os elementos válidos (não-null)
        List<AudioClip> validSounds = new List<AudioClip>();
        foreach (AudioClip clip in jumpSFX)
        {
            if (clip != null)
            {
                validSounds.Add(clip);
            }
        }

        // Retorna um som aleatório ou null se não houver sons válidos
        if (validSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, validSounds.Count);
            return validSounds[randomIndex];
        }

        Debug.LogWarning("[AudioManager] Nenhum Jump SFX válido encontrado no array!");
        return null;
    }

    /// <summary>
    /// Retorna quantos sons de pulo válidos estão configurados
    /// </summary>
    /// <returns>Número de sons válidos</returns>
    public int GetValidJumpSoundsCount()
    {
        if (jumpSFX == null) return 0;

        int count = 0;
        foreach (AudioClip clip in jumpSFX)
        {
            if (clip != null) count++;
        }
        return count;
    }

    #endregion Helper Methods

    #region Volume Methods

    private void ChangeVolume(string parameter, float volume, float minLimit)
    {
        if (mixer == null)
        {
            Debug.LogError("[AudioManager] Audio Mixer não está configurado!");
            return;
        }
        
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