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
    [SerializeField] private Animator logAnimator; // animator opcional do tronco
    [Tooltip("Parâmetro do Animator (trigger) a ser acionado quando a sequência iniciar. Se vazio, será ignorado.")]
    [SerializeField] private string animatorTriggerName = "";

    [Header("Câmeras Unity Padrão")]
    [SerializeField] private Camera logCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator cameraAnimator; // Animator da câmera (se a animação for da câmera)
    [Tooltip("Parâmetro do Animator (trigger) da câmera. Se vazio, será ignorado.")]
    [SerializeField] private string cameraAnimatorTriggerName = "";

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

        // Caso haja um Animator configurado mas não exista referência para o GameObject, tenta obter do log
        if (logAnimator == null && log != null)
        {
            logAnimator = log.GetComponent<Animator>();
        }

        // Caso a animação seja da câmera, tenta obter Animator da camera se não foi atribuído
        if (cameraAnimator == null && logCamera != null)
        {
            cameraAnimator = logCamera.GetComponent<Animator>();
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

        // Pequena espera para garantir que o GameObject e Animator inicializem
        yield return new WaitForSeconds(0.05f);

        // Troca para câmera do tronco
        if (logCamera != null && playerCamera != null)
        {
            playerCamera.enabled = false;
            logCamera.enabled = true;
            Debug.Log("[LogOutono] Câmera trocada para Log Camera");
        }
        else if (logCamera != null)
        {
            // Se não há playerCamera, apenas habilita logCamera
            logCamera.enabled = true;
            Debug.LogWarning("[LogOutono] PlayerCamera ausente — apenas LogCamera foi habilitada.");
        }

        // Dispara animação da câmera (se for animação da câmera)
        if (cameraAnimator != null && !string.IsNullOrEmpty(cameraAnimatorTriggerName))
        {
            try { cameraAnimator.ResetTrigger(cameraAnimatorTriggerName); } catch { }
            cameraAnimator.SetTrigger(cameraAnimatorTriggerName);
            Debug.Log($"[LogOutono] Trigger do Animator da câmera '{cameraAnimatorTriggerName}' acionado.");
        }
        else if (cameraAnimator != null)
        {
            Debug.Log("[LogOutono] Animator da câmera encontrado, mas nenhum trigger definido — não foi possível iniciar animação por trigger.");
        }

        // Dispara animação do tronco (se houver) - opcional
        if (logAnimator != null && !string.IsNullOrEmpty(animatorTriggerName))
        {
            try
            {
                logAnimator.ResetTrigger(animatorTriggerName);
            }
            catch { }
            logAnimator.SetTrigger(animatorTriggerName);
            Debug.Log($"[LogOutono] Trigger do Animator do tronco '{animatorTriggerName}' acionado.");
        }
        else if (logAnimator != null)
        {
            Debug.Log("[LogOutono] Animator do tronco encontrado, mas nenhum trigger definido — não foi possível iniciar animação por trigger.");
        }

        // Aguarda pelo tempo configurado
        Debug.Log($"[LogOutono] Aguardando {displayDuration}s...");
        yield return new WaitForSeconds(displayDuration);

        // Volta para câmera do player
        if (logCamera != null && playerCamera != null)
        {
            logCamera.enabled = false;
            playerCamera.enabled = true;
            Debug.Log("[LogOutono] Voltou para Player Camera");
        }
        else if (logCamera != null)
        {
            // se não houver playerCamera, mantém logCamera desativada
            logCamera.enabled = false;
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

            // Opcional: reset do Animator do tronco
            if (logAnimator != null && !string.IsNullOrEmpty(animatorTriggerName))
            {
                try
                {
                    logAnimator.ResetTrigger(animatorTriggerName);
                }
                catch { }
            }

            // Opcional: reset do Animator da câmera
            if (cameraAnimator != null && !string.IsNullOrEmpty(cameraAnimatorTriggerName))
            {
                try
                {
                    cameraAnimator.ResetTrigger(cameraAnimatorTriggerName);
                }
                catch { }
            }

            Debug.Log("[LogOutono] Sequência interrompida!");
        }
    }

    public bool IsSequencePlaying()
    {
        return isPlayingSequence;
    }
}
