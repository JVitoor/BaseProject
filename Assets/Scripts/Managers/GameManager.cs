using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : BaseManager
{
    public static GameManager Instance { get; private set; }

    [Header("Game Mechanics")]
    public VeraoQuente veraoQuente;
    [Header("Collectibles")]
    public int nutsCollected = 0;
    public TextMeshProUGUI nutsCounterText; // Refer�ncia para o texto UI

    [Header("Level Management")]
    public int currentLevel = 1;

    [Header("Season Configuration")]
    [Tooltip("Define qual estação está associada a cada fase/nível")]
    public Season[] levelSeasons = new Season[]
    {
        Season.Primavera,   // Level 1
        Season.Verao,       // Level 2
        Season.Outono,      // Level 3
        Season.Inverno     // Level 4
    };
    
    [Header("Player Respawn")]
    private Player player;
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
            nutsCounterText = GameObject.Find("NutsCounter")?.GetComponent<TextMeshProUGUI>();
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

        /*veraoQuente = FindObjectOfType<VeraoQuente>();
        if (veraoQuente == null)
        {
            Debug.LogWarning("[GameManager] Script VeraoQuente não encontrado na cena.");
        }*/
    }
    
    private void Start()
    {
        UpdateNutsUI();
        UpdateSeasonForCurrentLevel();
    }

    private void UpdateSeasonForCurrentLevel()
    {
        if (SeasonManager.Instance == null)
        {
            Debug.LogWarning("[GameManager] SeasonManager não encontrado!");
            return;
        }

        // Obtém o índice da cena atual
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

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
        
        Time.timeScale = 1f;
        
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