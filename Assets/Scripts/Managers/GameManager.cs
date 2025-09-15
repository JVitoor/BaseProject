using UnityEngine;
using UnityEngine.UI;

public class GameManager : BaseManager
{
    public static GameManager Instance { get; private set; }
    
    [Header("Collectibles")]
    public int nutsCollected = 0;
    public Text nutsCounterText; // Referência para o texto UI
    
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
}