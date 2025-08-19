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

    //[SerializeField] private Slider masterSlider;
    //[SerializeField] private Slider musicSlider;
    //[SerializeField] private Slider sfxSlider;

    #endregion Properties

    #region Initialization Routines

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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
        // Toca a música padrão ao iniciar a interface
        //if (AudioManager.instance.musics != null && AudioManager.instance.musics.Length > 0)
        //{
        //    AudioManager.instance.SwitchMusic(AudioManager.instance.musics[0]);
        //}
        //else
        //{
        //    Debug.LogWarning("[InterfaceManager] Nenhuma música definida em AudioManager.");
        //}
        //SetDefaultVolume();

        InitializeUIDictionary();
    }

    // Preenche o dicionário privado que relaciona o enum PanelsName com o GameObject do painel
    // O nome do painel é o nome do enum + " Panel", obtido pela extensão de PanelsName
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
                panel.SetActive(false); // Desativa o painel por padrão
            }
            else
            {
                Debug.LogWarning($"[InterfaceManager] Painel '{panelName}' não encontrado na cena.");
            }
        }
        ShowPanel(PanelsName.MainMenu); // Exibe o menu principal por padrão
    }

    #endregion Initialization Routines

    #region Auxiliar Methods

    public void ShowPanel(PanelsName name)
    {
        if (uiDictionary.TryGetValue(name, out GameObject panel))
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogError($"[InterfaceManager] Painel '{name.GetPanelName()}' não está registrado.");
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
            Debug.LogError($"[InterfaceManager] Painel '{name.GetPanelName()}' não está registrado.");
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

    private void Update()
    {
        //if (UserInputManager.instance.isUsingGamepad)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //else
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}
    }

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene(3);
        Time.timeScale = 1f;
    }

    // Altera o volume master
    //public void ChangeMasterVolume()
    //{
    //    AudioManager.instance.ChangeMasterVolume(masterSlider.value);
    //}

    //// Altera o volume da música
    //public void ChangeMusicVolume()
    //{
    //    AudioManager.instance.ChangeMusicVolume(musicSlider.value);
    //}

    //// Altera o volume dos efeitos sonoros
    //public void ChangeSFXVolume()
    //{
    //    AudioManager.instance.ChangeSFXVolume(sfxSlider.value);
    //}

    //public void SetDefaultVolume()
    //{
    //    AudioManager.instance.mixer.GetFloat("MasterVol", out float aux1);

    //    if (masterSlider != null)
    //    {
    //        masterSlider.value = aux1;
    //    }

    //    AudioManager.instance.mixer.GetFloat("MusicVol", out float aux2);

    //    if (musicSlider != null)
    //    {
    //        musicSlider.value = aux2;
    //    }

    //    AudioManager.instance.mixer.GetFloat("SFXVol", out float aux3);

    //    if (sfxSlider != null)
    //    {
    //        sfxSlider.value = aux3;
    //    }
    //}

    public void QuitGame()
    {
        Application.Quit();
    }
}