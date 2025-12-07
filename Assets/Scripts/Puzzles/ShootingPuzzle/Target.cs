using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Tag do projétil para detectar colisão")]
    public string projectileTag = "Projectile";

    [Header("Visual Feedback")]
    [Tooltip("Partículas ao acertar (opcional)")]
    public GameObject hitParticles;

    [Header("Destruction Settings")]
    [Tooltip("Tempo de delay antes de destruir (para permitir partículas)")]
    public float destroyDelay = 0.1f;

    // Estado
    private bool hasBeenHit = false;
    private ShootingPuzzleManager puzzleManager;

    // Chamado pelo PuzzleManager para inicializar
    public void Initialize(ShootingPuzzleManager manager)
    {
        puzzleManager = manager;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se foi atingido por um projétil
        if (collision.gameObject.CompareTag(projectileTag) && !hasBeenHit)
        {
            OnHit(collision.contacts[0].point);
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

        // Destrói o alvo após um pequeno delay
        Destroy(gameObject, destroyDelay);
    }
}
