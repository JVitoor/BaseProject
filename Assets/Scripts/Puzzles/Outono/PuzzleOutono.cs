using UnityEngine;

public class PuzzleOutono : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [Tooltip("Número de nozes necessárias para completar o puzzle")]
    public int requiredNuts = 5;

    [Tooltip("GameObject que será ativado ao completar o puzzle")]
    public GameObject objectToActivate;

    [Header("Status")]
    [Tooltip("Se o puzzle já foi completado")]
    private bool puzzleCompleted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!puzzleCompleted)
        {
            CheckPuzzleCompletion();
        }
    }

    private void CheckPuzzleCompletion()
    {
        if (GameManager.Instance == null) return;

        // Verifica se o jogador coletou o número necessário de nozes
        if (GameManager.Instance.nutsCollected >= requiredNuts)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        puzzleCompleted = true;

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
}
