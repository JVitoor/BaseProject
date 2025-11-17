using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    
    [Header("Winter Timer Configuration")]
    [Tooltip("Se verdadeiro, reseta o timer do Inverno quando ativado")]
    public bool resetWinterTimer = true;
    
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

            // Reseta o timer do Inverno se configurado
            if (resetWinterTimer && SeasonManager.Instance != null)
            {
                Season currentSeason = SeasonManager.Instance.GetCurrentSeason();
                if (currentSeason == Season.Inverno)
                {
                    SeasonManager.Instance.ResetWinterTimer();
                    Debug.Log("[Checkpoint] Timer do Inverno resetado!");
                }
            }

            // Feedback visual/sonoro (opcional)
            // if (activationEffect != null) Instantiate(activationEffect, transform.position, Quaternion.identity);
            // AudioManager.Instance.PlayCheckpointSound();
            
            Debug.Log($"[Checkpoint] Ativado em {transform.position}");

            // Opcional: Desativa o collider para não ser ativado de novo
            // GetComponent<Collider>().enabled = false;
        }
    }
}