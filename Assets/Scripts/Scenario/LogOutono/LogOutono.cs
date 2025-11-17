using UnityEngine;
using System.Collections;

/// <summary>
/// Sistema independente para ativar tronco com troca de câmera
/// Funciona com Camera padrão do Unity (sem dependências)
/// </summary>
public class LogOutono : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject log;
    
    [Header("Câmeras Unity Padrão")]
    [SerializeField] private Camera logCamera;
    [SerializeField] private Camera playerCamera;
    
    [Header("Configurações")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private bool activateOnStart = false;
    [SerializeField] private bool deactivateLogAfterDisplay = false;
    
    private bool isPlayingSequence = false;
    
    void Start()
    {
        ValidateReferences();
        
        if (log != null)
        {
            log.SetActive(false);
        }
        
        SetupCameras();
        
        if (activateOnStart)
        {
            ActivateLogSequence();
        }
    }
    
    private void SetupCameras()
    {
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
        }
        
        if (logCamera != null)
        {
            logCamera.enabled = false;
        }
    }
    
    private void ValidateReferences()
    {
        if (log == null)
        {
            Debug.LogError("[LogOutono] Tronco não atribuído!", this);
        }
        
        if (logCamera == null)
        {
            Debug.LogError("[LogOutono] Log Camera não atribuída!", this);
        }
        
        if (playerCamera == null)
        {
            Debug.LogWarning("[LogOutono] Player Camera não atribuída!", this);
            playerCamera = Camera.main;
        }
    }
    
    public void ActivateLogSequence()
    {
        if (isPlayingSequence)
        {
            Debug.LogWarning("[LogOutono] Sequência já em execução!");
            return;
        }
        
        StartCoroutine(LogActivationSequence());
    }
    
    private IEnumerator LogActivationSequence()
    {
        isPlayingSequence = true;
        
        Debug.Log("[LogOutono] Iniciando sequência...");
        
        // Ativa o tronco
        if (log != null)
        {
            log.SetActive(true);
            Debug.Log("[LogOutono] Tronco ativado!");
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // Troca para câmera do tronco
        if (logCamera != null && playerCamera != null)
        {
            playerCamera.enabled = false;
            logCamera.enabled = true;
            Debug.Log("[LogOutono] Câmera trocada para Log Camera");
        }
        
        // Aguarda
        Debug.Log($"[LogOutono] Aguardando {displayDuration}s...");
        yield return new WaitForSeconds(displayDuration);
        
        // Volta para câmera do player
        if (logCamera != null && playerCamera != null)
        {
            logCamera.enabled = false;
            playerCamera.enabled = true;
            Debug.Log("[LogOutono] Voltou para Player Camera");
        }
        
        // Opcionalmente desativa o tronco
        if (deactivateLogAfterDisplay && log != null)
        {
            log.SetActive(false);
            Debug.Log("[LogOutono] Tronco desativado!");
        }
        
        isPlayingSequence = false;
        Debug.Log("[LogOutono] Sequência finalizada!");
    }
    
    public void ActivateLogOnly()
    {
        if (log != null)
        {
            log.SetActive(true);
        }
    }
    
    public void DeactivateLog()
    {
        if (log != null)
        {
            log.SetActive(false);
        }
    }
    
    public void StopSequence()
    {
        if (isPlayingSequence)
        {
            StopAllCoroutines();
            isPlayingSequence = false;
            
            if (logCamera != null) logCamera.enabled = false;
            if (playerCamera != null) playerCamera.enabled = true;
            
            Debug.Log("[LogOutono] Sequência interrompida!");
        }
    }
    
    public bool IsSequencePlaying()
    {
        return isPlayingSequence;
    }
}
