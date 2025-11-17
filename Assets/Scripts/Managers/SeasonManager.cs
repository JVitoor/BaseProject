using UnityEngine;
using TMPro;

public enum Season
{
    Primavera = 0,  // Spring - Apenas andar e pular
    Verao = 1,      // Summer - Andar, pular e duplo pulo
    Outono = 2,     // Fall - Todas as mecânicas liberadas
    Inverno = 3     // Winter - Todas as mecânicas liberadas
}

public class SeasonManager : BaseManager
{
    #region Singleton

    public static SeasonManager Instance { get; private set; }

    #endregion

    #region Properties

    [Header(" ?? Configuração da Estação Atual")]
    [Tooltip("Define a estação/fase atual do jogo")]
    public Season currentSeason = Season.Primavera;

    [Header(" ?? Configuração de Inverno")]
    [Tooltip("Tempo limite em segundos para a fase de Inverno (padrão: 300s = 5 minutos)")]
    public float winterTimeLimitInSeconds = 300f;

    [Tooltip("Referência para o TextMeshProUGUI que mostra o temporizador")]
    public TextMeshProUGUI winterTimerText;

    [Tooltip("Painel de Game Over a ser exibido quando o tempo acabar")]
    public GameObject gameOverPanel;

    private float winterTimeRemaining;
    private bool winterTimerActive = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
    // Padrão Singleton
   if (Instance != null && Instance != this)
        {
     Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log($"[SeasonManager] Estação atual: {currentSeason}");
    }

    private void Start()
    {
      // Inicializa o timer se for Inverno
        if (currentSeason == Season.Inverno)
  {
            StartWinterTimer();
        }
   else
        {
// Esconde o timer e o painel de game over se não for Inverno
            if (winterTimerText != null)
                winterTimerText.gameObject.SetActive(false);
       
  if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Atualiza o timer do Inverno
        if (winterTimerActive && currentSeason == Season.Inverno)
        {
UpdateWinterTimer();
        }
    }

    #endregion

    #region Public Methods

    public Season GetCurrentSeason()
    {
        return currentSeason;
    }

  public bool IsJumpEnabled()
    {
        // Pulo está disponível em todas as estações
     return true;
    }

 public bool IsDoubleJumpEnabled()
    {
        // Duplo pulo disponível a partir do Verão
        return currentSeason >= Season.Verao;
    }

    public bool IsGlideEnabled()
    {
        // Planeio disponível apenas no Outono e Inverno
   return currentSeason >= Season.Outono;
    }

    public string GetAvailableAbilities()
    {
switch (currentSeason)
        {
            case Season.Primavera:
              return "Andar, Pular";
     case Season.Verao:
    return "Andar, Pular, Duplo Pulo";
      case Season.Outono:
            case Season.Inverno:
           return "Andar, Pular, Duplo Pulo, Planar";
            default:
 return "Desconhecido";
        }
    }

    #endregion

    #region Winter Timer Methods

    private void StartWinterTimer()
    {
        winterTimeRemaining = winterTimeLimitInSeconds;
        winterTimerActive = true;

        // Ativa o texto do timer
    if (winterTimerText != null)
        {
            winterTimerText.gameObject.SetActive(true);
            UpdateTimerDisplay();
        }
        else
{
            Debug.LogWarning("[SeasonManager] Referência para winterTimerText não está atribuída!");
        }

   // Garante que o painel de game over esteja desativado no início
        if (gameOverPanel != null)
        {
        gameOverPanel.SetActive(false);
        }
        else
        {
       Debug.LogWarning("[SeasonManager] Referência para gameOverPanel não está atribuída!");
   }

        Debug.Log($"[SeasonManager] Timer de Inverno iniciado: {winterTimeLimitInSeconds} segundos");
    }

    private void UpdateWinterTimer()
    {
        // Decrementa o tempo
        winterTimeRemaining -= Time.deltaTime;

        // Atualiza o display
  UpdateTimerDisplay();

        // Verifica se o tempo acabou
        if (winterTimeRemaining <= 0)
     {
            winterTimeRemaining = 0;
            winterTimerActive = false;
  OnWinterTimeExpired();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (winterTimerText == null)
            return;

 // Converte segundos para minutos:segundos
        int minutes = Mathf.FloorToInt(winterTimeRemaining / 60f);
  int seconds = Mathf.FloorToInt(winterTimeRemaining % 60f);

  // Formata o texto como MM:SS
 winterTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Opcional: Muda a cor do texto quando estiver próximo do fim
        if (winterTimeRemaining <= 30f)
   {
  winterTimerText.color = Color.red;
        }
        else if (winterTimeRemaining <= 60f)
        {
    winterTimerText.color = Color.yellow;
        }
     else
     {
     winterTimerText.color = Color.white;
        }
    }

    private void OnWinterTimeExpired()
{
        Debug.Log("[SeasonManager] Tempo de Inverno esgotado! Game Over!");

        // Pausa o jogo
      Time.timeScale = 0f;

   // Exibe o painel de Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
 }
        else
        {
            Debug.LogError("[SeasonManager] Painel de Game Over não está atribuído!");
        }

     // Chama o método OnGameOver do BaseManager
        OnGameOver();
    }

    /// <summary>
    /// Reinicia o timer do Inverno (útil para quando o jogador respawna em um checkpoint)
    /// </summary>
    public void ResetWinterTimer()
    {
        if (currentSeason == Season.Inverno)
  {
   winterTimeRemaining = winterTimeLimitInSeconds;
         winterTimerActive = true;
          UpdateTimerDisplay();
       Debug.Log("[SeasonManager] Timer de Inverno resetado!");
        }
    }

    /// <summary>
    /// Para o timer do Inverno (útil para quando o nível é concluído)
    /// </summary>
    public void StopWinterTimer()
    {
    winterTimerActive = false;
        Debug.Log("[SeasonManager] Timer de Inverno parado!");
    }

    #endregion
}
