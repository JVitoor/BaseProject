using UnityEngine;
using TMPro;

public class ItemScreen : MonoBehaviour
{
    #region Properties
    [SerializeField]
    public static ItemScreen Instance;
    int collectiblesCount = 0;
    public TextMeshProUGUI collectiblesText;

    #endregion

    #region Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCollectibles(int value)
    {
        collectiblesCount += value;
        UpdateUI();
    }

    void UpdateUI()
    {
        collectiblesText.text = "Coletáveis = " + collectiblesCount;
    }
    #endregion
}