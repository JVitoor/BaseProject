using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    #region Properties

    [Header("UI Elements")]
    public static InterfaceManager Instance { get; private set; }

    private Dictionary<PanelsName, GameObject> uiDictionary = new Dictionary<PanelsName, GameObject>();
    public GameObject gameInterface;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public Slider sliderTemperatura;

    #endregion Properties

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sliderTemperatura == null)
        {
            Debug.LogWarning("[InterfaceManager] O Slider de Temperatura não foi atribuído no Inspector.");
        }

        

        if (gameInterface == null)
        {
            Debug.LogWarning("[UIManager] Interface GameObject is not assigned in the Inspector.");
            try
            {
                gameInterface = GameObject.Find("GameInterface");
                if (gameInterface == null)
                {
                    Debug.LogError("[UIManager] GameInterface GameObject not found in the scene.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIManager] Error finding GameInterface: {e.Message}");
            }
        }

        //DontDestroyOnLoad(gameObject);
        //DontDestroyOnLoad(gameInterface);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (AudioManager.instance.musics != null && AudioManager.instance.musics.Length > 0)
        {
            AudioManager.instance.SwitchMusic(AudioManager.instance.musics[0]);
        }
        else
        {
            Debug.LogWarning("[InterfaceManager] Nenhuma m�sica definida em AudioManager.");
        }
        SetDefaultVolume();

        InitializeUIDictionary();
    }

    #endregion Unity Methods

    #region Auxiliar Methods

    // Preenche o dicion�rio privado que relaciona o enum PanelsName com o GameObject do painel
    // O nome do painel � o nome do enum + " Panel", obtido pela extens�o de PanelsName
    private void InitializeUIDictionary()
    {
        foreach (PanelsName name in Enum.GetValues(typeof(PanelsName)))
        {
            string panelName = name.GetPanelName();
            GameObject panel = GameObject.Find(panelName);

            if (panel != null)
            {
                Debug.Log($"[InterfaceManager] Painel '{panelName}' encontrado e registrado.");
                uiDictionary[name] = panel;
                panel.SetActive(false); // Desativa o painel por padr�o
            }
            else
            {
                Debug.LogWarning($"[InterfaceManager] Painel '{panelName}' n�o encontrado na cena.");
            }
        }
        ShowPanel(PanelsName.MainMenu); // Exibe o menu principal por padr�o
    }


    public void ShowPanel(PanelsName name)
    {
        if (uiDictionary.TryGetValue(name, out GameObject panel))
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogError($"[InterfaceManager] Painel '{name.GetPanelName()}' n�o est� registrado.");
        }
    }

    public void HidePanel(PanelsName name)
    {
        if (uiDictionary.TryGetValue(name, out GameObject panel))
        {
            panel.SetActive(false);
        }
        else
        {
            Debug.LogError($"[InterfaceManager] Painel '{name.GetPanelName()}' n�o est� registrado.");
        }
    }

    public GameObject GetPanel(PanelsName name)
    {
        uiDictionary.TryGetValue(name, out GameObject panel);
        return panel;
    }

    #endregion Auxiliar Methods

    public void GameSceneLoad()
    {
        //ShowPanel(PanelsName.Loading);
        SceneManager.LoadScene(1); // Carrega a cena do jogo
        Time.timeScale = 1f; // Garante que o jogo rode na velocidade normal
    }

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene(3);
        Time.timeScale = 1f;
    }

    public void ChangeMasterVolume()
    {
        float volume = masterSlider.value;
        AudioManager.instance.ChangeMasterVolume(volume);
        PlayerPrefs.SetFloat("MasterVol", volume); // Salva
    }

    // Altera o volume da msica
    public void ChangeMusicVolume()
    {
        float volume = musicSlider.value;
        AudioManager.instance.ChangeMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVol", volume); // Salva
    }

    // Altera o volume dos efeitos sonoros
    public void ChangeSFXVolume()
    {
        float volume = sfxSlider.value;
        AudioManager.instance.ChangeSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVol", volume); // Salva
    }

    public void SetDefaultVolume()
    {
        // Carrega o valor salvo, ou usa 0 como padr�o se n�o houver nada salvo
        float masterVol = PlayerPrefs.GetFloat("MasterVol", 0);
        float musicVol = PlayerPrefs.GetFloat("MusicVol", 0);
        float sfxVol = PlayerPrefs.GetFloat("SFXVol", 0);

        // Ajusta os sliders para o valor carregado
        if (masterSlider != null) masterSlider.value = masterVol;
        if (musicSlider != null) musicSlider.value = musicVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;

        // Aplica os valores carregados ao mixer imediatamente
        AudioManager.instance.ChangeMasterVolume(masterVol);
        AudioManager.instance.ChangeMusicVolume(musicVol);
        AudioManager.instance.ChangeSFXVolume(sfxVol);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}