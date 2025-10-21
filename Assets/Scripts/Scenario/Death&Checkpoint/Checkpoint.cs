using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    // Opcional: Efeitos para feedback
    // public GameObject activationEffect; 

    private void Awake()
    {
        // Garante que o collider seja um trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o player e se ainda não foi ativado
        if (!isActivated && other.CompareTag("Player"))
        {
            // Marca como ativado
            isActivated = true;
            
            // Informa ao GameManager a nova posição de checkpoint
            GameManager.Instance.SetCheckpoint(transform.position);

            // Feedback visual/sonoro (opcional)
            // if (activationEffect != null) Instantiate(activationEffect, transform.position, Quaternion.identity);
            // AudioManager.Instance.PlayCheckpointSound();
            
            Debug.Log($"[Checkpoint] Ativado em {transform.position}");

            // Opcional: Desativa o collider para não ser ativado de novo
            // GetComponent<Collider>().enabled = false;
        }
    }
}