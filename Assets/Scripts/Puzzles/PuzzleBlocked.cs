using UnityEngine;
using TMPro;

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

    [Tooltip("Nome do trigger da animação de desaparecer")]
    public string disappearTrigger = "Disappear";

    [Tooltip("Tempo que a animação leva para completar (segundos)")]
    public float animationDuration = 2f;

    [Header("UI Feedback")]
    [Tooltip("Texto UI para mostrar mensagens ao jogador")]
    public TextMeshProUGUI feedbackText;

    [Tooltip("Mensagem quando não tem nozes suficientes")]
    public string insufficientNutsMessage = "Você precisa coletar {0} nozes para desbloquear!";

    [Tooltip("Mensagem quando está desbloqueando")]
    public string unlockingMessage = "Desbloqueando passagem...";

    [Tooltip("Tempo que a mensagem fica na tela (segundos)")]
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
            // Não tem nozes suficientes - mostra mensagem
            int nutsNeeded = requiredNuts - currentNuts;
            string message = string.Format(insufficientNutsMessage, nutsNeeded);
            ShowFeedbackMessage(message);
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

        // Mostra mensagem de desbloqueio
        ShowFeedbackMessage(unlockingMessage);

        // Toca a animação se tiver animator
        if (wallAnimator != null && !string.IsNullOrEmpty(disappearTrigger))
        {
            wallAnimator.SetTrigger(disappearTrigger);
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

    private void ShowFeedbackMessage(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);

            // Esconde a mensagem após um tempo
            CancelInvoke(nameof(HideFeedbackMessage));
            Invoke(nameof(HideFeedbackMessage), messageDuration);
        }
        else
        {
            // Se não tiver UI, mostra no console
            Debug.Log($"[PuzzleBlocked] Mensagem: {message}");
        }
    }

    private void HideFeedbackMessage()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }
}
