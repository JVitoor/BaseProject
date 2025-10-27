using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager
{
    public static GameManager Instance { get; private set; }

    [Header("Game Mechanics")]
    private VeraoQuente veraoQuente;
    [Header("Collectibles")]
    public int nutsCollected = 0;
    public Text nutsCounterText; // Refer�ncia para o texto UI

    [Header("Level Management")]
    public int currentLevel = 1;
    
    [Header("Player Respawn")]
    private Player player; // Referência ao script do player
    private Vector3 initialSpawnPoint;
    public Vector3 lastCheckpointPosition { get; private set; }
        
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

        // --- LÓGICA DE SPAWN/CHECKPOINT ---
        player = FindObjectOfType<Player>(); // Encontra o player na cena
        if (player != null)
        {
            // Define o ponto de spawn inicial baseado na posição inicial do player
            initialSpawnPoint = player.transform.position;
            lastCheckpointPosition = initialSpawnPoint;
        }
        else
        {
            Debug.LogError("[GameManager] Player não encontrado na cena!");
        }

        veraoQuente = FindObjectOfType<VeraoQuente>();
        if (veraoQuente == null)
        {
            Debug.LogWarning("[GameManager] Script VeraoQuente não encontrado na cena.");
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
            Debug.LogWarning("[GameManager] Texto do contador de nozes n�o encontrado!");
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
        Debug.Log($"[GameManager] Carregando pr�xima fase: {nextLevel}");
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

        // Carrega a cena do menu principal (assumindo que seja o �ndice 0)
        SceneManager.LoadScene(0);
    }
    
    public void SetCheckpoint(Vector3 newPosition)
{
    Debug.Log($"[GameManager] Novo checkpoint definido em: {newPosition}");
    // Armazena a posição do checkpoint, elevando-a ligeiramente
    // para evitar que o player caia através do chão ao respawnar.
    lastCheckpointPosition = newPosition + Vector3.up * 2f; 
}

    public void RespawnPlayer()
    {
        Debug.Log("[GameManager] Recebida ordem de respawn...");
        if (player != null)
        {
            // Chama o método de respawn no script do Player
            player.Respawn(lastCheckpointPosition);
            if (veraoQuente != null)
            {
                veraoQuente.ResetarTemperatura();
            }
        }
        else
        {
            Debug.LogError("[GameManager] Referência do Player perdida! Não é possível respawnar.");
            // Como último recurso, recarrega a cena
            RestartCurrentLevel();
        }
    }

    #endregion Level Loading Methods
}