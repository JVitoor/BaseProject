using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Tag do projétil para detectar colisão")]
    public string projectileTag = "Projectile";

    [Header("Visual Feedback")]
    [Tooltip("Partículas ao acertar (opcional)")]
    public GameObject hitParticles;

    [Header("Animator Settings")]
    [Tooltip("Animator do alvo (flor) para tocar animação de acerto")]
    public Animator animator;
    [Tooltip("Nome do parâmetro trigger no Animator para animação de acerto")]
    public string hitTriggerParam = "Hit";

    [Header("Destruction Settings")]
    [Tooltip("Tempo de delay antes de destruir (para permitir partículas) - não usado quando apenas animação é tocada")]
    public float destroyDelay = 0.1f;

    // Estado
    private bool hasBeenHit = false;
    private ShootingPuzzleManager puzzleManager;

    // Chamado pelo PuzzleManager para inicializar
    public void Initialize(ShootingPuzzleManager manager)
    {
        puzzleManager = manager;

        // Se não foi atribuído, tenta obter automaticamente
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se foi atingido por um projétil
        if (collision.gameObject.CompareTag(projectileTag) && !hasBeenHit)
        {
            OnHit(collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Alternativa usando trigger
        if (other.CompareTag(projectileTag) && !hasBeenHit)
        {
            OnHit(other.transform.position);
        }
    }

    private void OnHit(Vector3 hitPosition)
    {
        if (hasBeenHit) return;

        hasBeenHit = true;

        Debug.Log($"[Target] Alvo {gameObject.name} foi acertado!");

        // Spawna partículas de acerto (se configurado)
        if (hitParticles != null)
        {
            Instantiate(hitParticles, hitPosition, Quaternion.identity);
        }

        // Dispara animação de acerto via trigger
        if (animator != null)
        {
            animator.ResetTrigger(hitTriggerParam); // garante estado limpo
            animator.SetTrigger(hitTriggerParam);
        }
        else
        {
            Debug.LogWarning($"[Target] {gameObject.name} não possui Animator atribuído para tocar animação de acerto.");
        }

        // Notifica o puzzle manager
        if (puzzleManager != null)
        {
            puzzleManager.OnTargetHit(this);
        }
        else
        {
            Debug.LogWarning($"[Target] {gameObject.name} não tem referência ao PuzzleManager!");
        }

        // Desabilita collider imediatamente para evitar múltiplos hits
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Não destruímos a flor/alvo; a animação cuida do feedback visual.
        // Se em algum caso precisar remover depois, pode-se usar Destroy(gameObject, destroyDelay);
    }
}
