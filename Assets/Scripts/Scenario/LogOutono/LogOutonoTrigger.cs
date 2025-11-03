using UnityEngine;

/// <summary>
/// Trigger que ativa a sequência do tronco quando o player entra na área
/// Adicione este script a um GameObject com Collider configurado como Trigger
/// Funciona com a versão Cinemachine do LogOutono
/// </summary>
[RequireComponent(typeof(Collider))]
public class LogOutonoTrigger : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private LogOutono logOutono; // Referência ao script LogOutono
    
    [Header("Configurações")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool triggerOnce = true; // Ativa apenas uma vez?
    [SerializeField] private bool requireInteraction = false; // Requer apertar uma tecla?
    [SerializeField] private KeyCode interactionKey = KeyCode.E; // Tecla de interação
    
    [Header("UI (Opcional)")]
    [SerializeField] private GameObject interactionPrompt; // UI para mostrar "Pressione E"
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private bool hasTriggered = false;
    private bool playerInRange = false;
    private Collider triggerCollider;
    
    void Start()
    {
        // Validação
        if (logOutono == null)
        {
            Debug.LogError("[LogOutonoTrigger] Referência ao LogOutono não está atribuída!", this);
        }
        
        // Garante que o collider é trigger
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
        
        // Esconde o prompt de interação inicialmente
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void Update()
    {
        // Se requer interação e player está na área
        if (requireInteraction && playerInRange && !hasTriggered)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                ActivateLogSequence();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o player
        if (!other.CompareTag(playerTag)) return;
        
        playerInRange = true;
        
        // Se já foi ativado e só pode ativar uma vez, ignora
        if (hasTriggered && triggerOnce) return;
        
        if (requireInteraction)
        {
            // Mostra prompt de interação
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"[LogOutonoTrigger] Pressione {interactionKey} para ativar o tronco!");
            }
        }
        else
        {
            // Ativa automaticamente
            ActivateLogSequence();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        playerInRange = false;
        
        // Esconde prompt de interação
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    /// <summary>
    /// Ativa a sequência do tronco com Cinemachine
    /// </summary>
    private void ActivateLogSequence()
    {
        if (logOutono != null && !logOutono.IsSequencePlaying())
        {
            logOutono.ActivateLogSequence();
            hasTriggered = true;
            
            // Esconde o prompt após ativar
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            if (showDebugLogs)
            {
                Debug.Log("[LogOutonoTrigger] Sequência do tronco ativada! Cinemachine fará o blend automaticamente.");
            }
        }
    }
    
    /// <summary>
    /// Reseta o trigger para poder ser ativado novamente
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        if (showDebugLogs)
        {
            Debug.Log("[LogOutonoTrigger] Trigger resetado!");
        }
    }
    
    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Desenha a área do trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = hasTriggered ? Color.gray : Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (col is BoxCollider boxCollider)
            {
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            }
            else if (col is SphereCollider sphereCollider)
            {
                Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
            }
            
            // Label
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 2f,
                hasTriggered ? "TRIGGER USADO" : "TRIGGER ATIVO\n(Cinemachine)"
            );
        }
    }
#endif
    #endregion
}
