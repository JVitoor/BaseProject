using UnityEngine;

public class PuzzleOutono : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [Tooltip("Número de nozes necessárias para completar o puzzle")]
    public int requiredNuts = 20;

    [Tooltip("GameObject que será ativado ao completar o puzzle")]
    public GameObject objectToActivate;

    [Header("UI Feedback")]
    [Tooltip("Painel UI para mostrar quando não tem nozes suficientes")]
    public GameObject panelPuzzle;

    [Tooltip("Tempo que o painel fica na tela (segundos)")]
    public float messageDuration = 1.5f;

    [Header("Status")]
    [Tooltip("Se o puzzle já foi completado")]
    private bool puzzleCompleted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Garante que o painel comece desativado
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o player que entrou no trigger
        if (other.CompareTag("Player") && !puzzleCompleted)
        {
            CheckPuzzleCompletion();
        }
    }

    private void CheckPuzzleCompletion()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[PuzzleOutono] GameManager não encontrado!");
            return;
        }

        // Verifica se o jogador coletou o número necessário de nozes
        if (GameManager.Instance.nutsCollected >= requiredNuts)
        {
            CompletePuzzle();
        }
        else
        {
            // Não tem nozes suficientes - mostra painel
            int nutsNeeded = requiredNuts - GameManager.Instance.nutsCollected;
            ShowFeedbackPanel();
            Debug.Log($"[PuzzleOutono] Nozes insuficientes! Tem: {GameManager.Instance.nutsCollected}, Precisa: {requiredNuts}");
        }
    }

    private void CompletePuzzle()
    {
        puzzleCompleted = true;

        // Esconde o painel se estiver visível
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(false);
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"[PuzzleOutono] Puzzle completado! {requiredNuts} nozes coletadas. Objeto ativado.");
        }
        else
        {
            Debug.LogWarning("[PuzzleOutono] GameObject não está atribuído!");
        }
    }

    private void ShowFeedbackPanel()
    {
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(true);

            // Esconde o painel após um tempo
            CancelInvoke(nameof(HideFeedbackPanel));
            Invoke(nameof(HideFeedbackPanel), messageDuration);
        }
        else
        {
            Debug.LogWarning("[PuzzleOutono] Painel de feedback não está configurado!");
        }
    }

    private void HideFeedbackPanel()
    {
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(false);
        }
    }
}
