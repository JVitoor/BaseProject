using UnityEngine;

public class ShootingPuzzleTrigger : MonoBehaviour
{
    [Header("Puzzle References")]
    [Tooltip("Referência ao gerenciador do puzzle de tiro")]
    public ShootingPuzzleManager puzzleManager;

    [Header("Trigger Settings")]
    [Tooltip("Tag do player para detectar colisão")]
    public string playerTag = "Player";

    private bool puzzleStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o player que entrou no trigger
        if (other.CompareTag(playerTag) && !puzzleStarted)
        {
            Debug.Log("[ShootingPuzzleTrigger] Player entrou no trigger do puzzle!");

            if (puzzleManager != null)
            {
                puzzleManager.StartPuzzle();
                puzzleStarted = true;
            }
            else
            {
                Debug.LogError("[ShootingPuzzleTrigger] PuzzleManager não está atribuído!");
            }
        }
    }

    // Método para resetar o puzzle (caso necessário)
    public void ResetTrigger()
    {
        puzzleStarted = false;
    }
}
