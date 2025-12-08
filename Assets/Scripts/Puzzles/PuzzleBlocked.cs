using UnityEngine;

public class PuzzleBlocked : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Número de nozes necessárias para desbloquear")]
    public int requiredNuts = 10;

    [Header("Referências")]
    [Tooltip("GameObject da parede que será desativada")]
    public GameObject wall;

    [Tooltip("Animator da parede para tocar a animação de desaparecer")]
    public Animator wallAnimator;
    public Animator wallAnimator2;

    [Tooltip("Nome do trigger da animação de desaparecer")]
    public string disappearTrigger = "Disappear";

    [Tooltip("Tempo que a animação leva para completar (segundos)")]
    public float animationDuration = 2f;

    [Header("UI Feedback")]
    [Tooltip("Painel UI para mostrar quando não tem nozes suficientes")]
    public GameObject feedbackPanel;

    [Tooltip("Tempo que o painel fica na tela (segundos)")]
    public float messageDuration = 3f;

    private bool hasBeenUnlocked = false;

    public GameObject invisibleWall;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o player que entrou no trigger
        if (other.CompareTag("Player") && !hasBeenUnlocked)
        {
            CheckAndUnlock();
        }
    }

    private void CheckAndUnlock()
    {
        // Verifica se o GameManager existe
        if (GameManager.Instance == null)
        {
            Debug.LogError("[PuzzleBlocked] GameManager não encontrado!");
            return;
        }

        // Verifica quantas nozes o jogador tem
        int currentNuts = GameManager.Instance.nutsCollected;

        if (currentNuts < requiredNuts)
        {
            // Não tem nozes suficientes - mostra painel
            int nutsNeeded = requiredNuts - currentNuts;
            ShowFeedbackPanel();
            Debug.Log($"[PuzzleBlocked] Nozes insuficientes! Tem: {currentNuts}, Precisa: {requiredNuts}");
        }
        else
        {
            // Tem as 10 nozes - desbloqueia
            UnlockWall();
        }
    }

    private void UnlockWall()
    {
        hasBeenUnlocked = true;

        Debug.Log("[PuzzleBlocked] Desbloqueando parede!");

        // Toca a animação se tiver animator
        if (wallAnimator != null && !string.IsNullOrEmpty(disappearTrigger))
        {
            wallAnimator.SetTrigger(disappearTrigger);
        }
        
        if (wallAnimator2 != null && !string.IsNullOrEmpty(disappearTrigger))
        {
            wallAnimator2.SetTrigger(disappearTrigger);
        }

        // Desativa a parede após a duração da animação
        if (wall != null)
        {
            invisibleWall.SetActive(true);
            Invoke(nameof(DeactivateWall), animationDuration);
        }
        else
        {
            Debug.LogWarning("[PuzzleBlocked] Referência da parede não definida!");
        }
    }

    private void DeactivateWall()
    {
        if (wall != null)
        {
            wall.SetActive(false);
            Debug.Log("[PuzzleBlocked] Parede desativada!");
        }
    }

    private void ShowFeedbackPanel()
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);

            // Esconde o painel após um tempo
            CancelInvoke(nameof(HideFeedbackPanel));
            Invoke(nameof(HideFeedbackPanel), messageDuration);
        }
        else
        {
            Debug.LogWarning("[PuzzleBlocked] Painel de feedback não está configurado!");
        }
    }

    private void HideFeedbackPanel()
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
}
