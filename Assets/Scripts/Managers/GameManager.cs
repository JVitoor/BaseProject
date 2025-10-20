using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager
{
    public static GameManager Instance { get; private set; }
    
    [Header("Collectibles")]
    public int nutsCollected = 0;
    public Text nutsCounterText; // Referência para o texto UI
    
    [Header("Level Management")]
    public int currentLevel = 1;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Tenta encontrar o texto do contador se não foi atribuído
        if (nutsCounterText == null)
        {
            nutsCounterText = GameObject.Find("NutsCounter")?.GetComponent<Text>();
        }
    }
    
    private void Start()
    {
        UpdateNutsUI();
    }
    
    public void AddNut()
    {
        nutsCollected++;
        UpdateNutsUI();
        Debug.Log($"Noz coletada! Total: {nutsCollected}");
    }
    
    private void UpdateNutsUI()
    {
        if (nutsCounterText != null)
        {
            nutsCounterText.text = nutsCollected.ToString();
        }
        else
        {
            Debug.LogWarning("[GameManager] Texto do contador de nozes não encontrado!");
        }
    }
    
    #region Level Loading Methods
    
    public void LoadLevel(int levelIndex)
    {
        Debug.Log($"[GameManager] Carregando fase {levelIndex}...");
        currentLevel = levelIndex;
        
        // Garante que o tempo esteja normal antes de carregar
        Time.timeScale = 1f;
        
        // Carrega a cena
        SceneManager.LoadScene(levelIndex);
    }
    
    public void LoadNextLevel()
    {
        int nextLevel = currentLevel + 1;
        Debug.Log($"[GameManager] Carregando próxima fase: {nextLevel}");
        LoadLevel(nextLevel);
    }
    
    public void RestartCurrentLevel()
    {
        Debug.Log($"[GameManager] Reiniciando fase atual: {currentLevel}");
        LoadLevel(currentLevel);
    }
    
    public void LoadMainMenu()
    {
        Debug.Log("[GameManager] Voltando ao menu principal...");
        
        // Garante que o tempo esteja normal
        Time.timeScale = 1f;
        
        // Carrega a cena do menu principal (assumindo que seja o índice 0)
        SceneManager.LoadScene(0);
    }
    
    #endregion Level Loading Methods
}