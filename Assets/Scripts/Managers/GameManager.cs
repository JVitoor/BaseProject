using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CheckpointEntry
{
    public string id;
    public Checkpoint checkpoint;
}

public class GameManager : BaseManager
{
    public static GameManager Instance { get; private set; }

    [Header("Game Mechanics")]
    public VeraoQuente veraoQuente;
    [Header("Collectibles")]
    public int nutsCollected = 0;
    public TextMeshProUGUI nutsCounterText; // Referência para o texto UI

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

    [Header("Checkpoint System")]
    [Tooltip("Arraste os checkpoints aqui e configure seus IDs")]
    public List<CheckpointEntry> checkpointList = new List<CheckpointEntry>();

    private Dictionary<string, Checkpoint> checkpoints = new Dictionary<string, Checkpoint>();

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

        // Converte a lista de checkpoints para o dicionário
        InitializeCheckpoints();
    }

    private void Start()
    {
        UpdateNutsUI();
        UpdateSeasonForCurrentLevel();
    }

    private void InitializeCheckpoints()
    {
        checkpoints.Clear();

        foreach (var entry in checkpointList)
        {
            if (entry.checkpoint != null && !string.IsNullOrEmpty(entry.id))
            {
                if (checkpoints.ContainsKey(entry.id))
                {
                    Debug.LogWarning($"[GameManager] Checkpoint com ID '{entry.id}' duplicado! Ignorando...");
                }
                else
                {
                    checkpoints.Add(entry.id, entry.checkpoint);
                }
            }
        }

        Debug.Log($"[GameManager] {checkpoints.Count} checkpoint(s) registrado(s)");
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
            Debug.LogWarning("[GameManager] Texto do contador de nozes não encontrado!");
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

    public void TeleportToCheckpoint(string checkpointID)
    {
        if (player == null)
        {
            Debug.LogError("[GameManager] Player não encontrado!");
            return;
        }

        if (!checkpoints.ContainsKey(checkpointID))
        {
            Debug.LogError($"[GameManager] Checkpoint '{checkpointID}' não encontrado!");
            return;
        }

        Checkpoint targetCheckpoint = checkpoints[checkpointID];
        Vector3 teleportPosition = targetCheckpoint.GetTeleportPosition();

        Debug.Log($"[GameManager] Teleportando para '{checkpointID}'");
        player.Respawn(teleportPosition);
    }

    #endregion Level Loading Methods
}