using UnityEngine;

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
}
