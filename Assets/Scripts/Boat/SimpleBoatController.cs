using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SimpleBoatController : MonoBehaviour
{
    [Header("Movimento (sem física)")]
    [Tooltip("Velocidade de translação do barco")]
    public float moveSpeed = 5f;
    [Tooltip("Velocidade de rotação do barco (graus/s)")]
    public float turnSpeed = 90f;

    [Header("Área delimitada")]
    [Tooltip("Centro da área onde o barco pode se mover")]
    public Vector3 areaCenter = Vector3.zero;
    [Tooltip("Tamanho da área (X = largura, Z = profundidade). Y é ignorado.")]
    public Vector3 areaSize = new Vector3(20f, 0f, 20f);

    [Header("Interação do jogador")]
    [Tooltip("Transform do assento onde o jogador ficará ao entrar no barco")]
    public Transform seatTransform;
    [Tooltip("Distância do trigger de interação. Se null, um SphereCollider será criado")]
    public SphereCollider interactionTrigger;
    [Tooltip("Tecla para entrar/sair do barco")]
    public KeyCode interactKey = KeyCode.E;
    [Tooltip("Painel UI que aparece quando o jogador se aproxima")]
    public GameObject interactionPanel;
    [Tooltip("Painel UI mostrado quando não há nozes suficientes")]
    public GameObject insufficientNutsPanel;

    [Header("Requisitos")]
    [Tooltip("Quantidade mínima de nozes necessárias para entrar no barco")]
    public int requiredNuts = 0;

    // Estado
    public bool isOccupied = false;
    private bool playerInRange = false;
    public GameObject currentPlayer; 

    // Guarda a posição/rota original do jogador para retornar ao sair
    private Vector3 savedPlayerPosition;
    private Quaternion savedPlayerRotation;

    // Componentes do jogador que podem ser desativados
    private CharacterController playerCC;
    private MonoBehaviour playerMovementScript;

    // Posição inicial do barco para reset
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        // Armazena posição/rot inicial
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Garante um trigger de interação
        if (interactionTrigger == null)
        {
            interactionTrigger = gameObject.AddComponent<SphereCollider>();
            interactionTrigger.isTrigger = true;
            interactionTrigger.radius = 2f;
            interactionTrigger.center = Vector3.zero;
        }
        else
        {
            interactionTrigger.isTrigger = true;
        }

        // Garante que os painéis de interação começam desativados
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
        if (insufficientNutsPanel != null)
        {
            insufficientNutsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInteractionInput();

        if (isOccupied)
        {
            // Movimento simples por transform, sem física
            float mv = Input.GetAxis("Vertical");
            float tr = Input.GetAxis("Horizontal");

            if (Mathf.Abs(mv) > 0.001f)
            {
                transform.Translate(transform.forward * mv * moveSpeed * Time.deltaTime, Space.World);
            }

            if (Mathf.Abs(tr) > 0.001f)
            {
                transform.Rotate(Vector3.up, tr * turnSpeed * Time.deltaTime, Space.World);
            }

            // Restringe posição dentro da área (mantém Y do centro)
            Vector3 half = new Vector3(areaSize.x * 0.5f, 0f, areaSize.z * 0.5f);
            Vector3 local = transform.position - areaCenter;
            local.x = Mathf.Clamp(local.x, -half.x, half.x);
            local.z = Mathf.Clamp(local.z, -half.z, half.z);
            Vector3 constrainedPos = areaCenter + new Vector3(local.x, transform.position.y, local.z);
            transform.position = constrainedPos;

            // Mantém Y do barco igual ao do centro (opcional)
            Vector3 pos = transform.position;
            pos.y = areaCenter.y;
            transform.position = pos;

            // Se um jogador estiver montado, mantém sua posição no assento
            if (currentPlayer != null && seatTransform != null)
            {
                currentPlayer.transform.position = seatTransform.position;
                currentPlayer.transform.rotation = seatTransform.rotation;
            }
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (playerInRange && !isOccupied && currentPlayer != null)
            {
                // Verifica requisito de nozes antes de permitir entrar
                if (CanEnterBoat())
                {
                    EnterBoat();
                }
                else
                {
                    Debug.Log($"[SimpleBoatController] Jogador precisa de {requiredNuts} nozes para entrar no barco. Atual: {GetCurrentNuts()}");
                    ShowInsufficientNutsUI();
                }
            }
            else if (isOccupied && currentPlayer != null)
            {
                ExitBoat();
            }
        }
    }

    private void ShowInsufficientNutsUI()
    {
        // Mostra painel de insuficiência e esconde o painel padrão
        if (insufficientNutsPanel != null)
        {
            insufficientNutsPanel.SetActive(true);
        }
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
    }

    private void HideAllInteractionUI()
    {
        if (interactionPanel != null) interactionPanel.SetActive(false);
        if (insufficientNutsPanel != null) insufficientNutsPanel.SetActive(false);
    }

    private void ShowAppropriatePanel()
    {
        // Decide qual painel mostrar quando o jogador entra no trigger
        bool canEnter = CanEnterBoat();
        if (!isOccupied)
        {
            if (canEnter)
            {
                if (interactionPanel != null) interactionPanel.SetActive(true);
                if (insufficientNutsPanel != null) insufficientNutsPanel.SetActive(false);
            }
            else
            {
                if (insufficientNutsPanel != null) insufficientNutsPanel.SetActive(true);
                if (interactionPanel != null) interactionPanel.SetActive(false);
            }
        }
    }

    private bool CanEnterBoat()
    {
        // Se não houver restrição, permite entrar
        if (requiredNuts <= 0) return true;

        // Usa GameManager para verificar quantidade de nozes coletadas
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[SimpleBoatController] GameManager.Instance é null. Permitindo entrada por segurança.");
            return true;
        }

        return GameManager.Instance.nutsCollected >= requiredNuts;
    }

    private int GetCurrentNuts()
    {
        return GameManager.Instance != null ? GameManager.Instance.nutsCollected : 0;
    }

    private void EnterBoat()
    {
        if (currentPlayer == null || seatTransform == null) return;

        // Salva posição/rota original
        savedPlayerPosition = currentPlayer.transform.position;
        savedPlayerRotation = currentPlayer.transform.rotation;

        // Desativa componentes de movimento do jogador, se existirem
        playerCC = currentPlayer.GetComponent<CharacterController>();
        playerMovementScript = currentPlayer.GetComponent<MonoBehaviour>(); // fallback genérico

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerCC != null) playerCC.enabled = false;

        // Move jogador para o assento e parentear
        currentPlayer.transform.position = seatTransform.position;
        currentPlayer.transform.rotation = seatTransform.rotation;
        currentPlayer.transform.SetParent(seatTransform, true);

        isOccupied = true;

        // Desativa todos os painéis quando entrar no barco
        HideAllInteractionUI();
    }

    public void ExitBoat()
    {
        if (currentPlayer == null) return;

        // Desparenta e mantém a posição/rotação atual do jogador (permanece onde está no assento)
        currentPlayer.transform.SetParent(null, true);

        // Reativa componentes do jogador
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerCC != null) playerCC.enabled = true;

        isOccupied = false;

        // Mostra o painel apropriado se o jogador ainda estiver próximo
        if (playerInRange)
        {
            ShowAppropriatePanel();
        }
        else
        {
            HideAllInteractionUI();
        }

        // limpa referência ao jogador
        currentPlayer = null;
    }

    // Força o jogador a sair do barco (usado por sistemas externos como DeadZone)
    public void ForceExitPlayer()
    {
        if (currentPlayer != null)
        {
            // Reativa componentes do jogador
            if (playerMovementScript != null) playerMovementScript.enabled = true;
            if (playerCC != null) playerCC.enabled = true;

            // Remove parent
            currentPlayer.transform.SetParent(null, true);

            currentPlayer = null;
            isOccupied = false;

            HideAllInteractionUI();
        }
    }

    // Desapega o jogador se necessário.
    public void ResetToInitialPosition()
    {
        // Se houver jogador no barco, força saída
        if (currentPlayer != null)
        {
            ForceExitPlayer();
        }

        // Move o barco para a posição inicial
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Reinicializa estado
        isOccupied = false;
        currentPlayer = null;

        HideAllInteractionUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o jogador através da tag
        if (other == null) return;
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        currentPlayer = other.gameObject;

        // Mostra o painel correto dependendo da quantidade de nozes
        ShowAppropriatePanel();
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica se é o jogador através da tag
        if (other == null) return;
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        // Limpa referência ao jogador se não estiver no barco
        if (!isOccupied && currentPlayer == other.gameObject)
        {
            currentPlayer = null;
        }

        // Desativa todos os painéis quando o jogador sair do alcance do trigger
        HideAllInteractionUI();
    }

    // Visualização da área no editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.2f);
        Gizmos.DrawCube(areaCenter + new Vector3(0f, 0f, 0f), new Vector3(areaSize.x, 0.1f, areaSize.z));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(areaCenter, new Vector3(areaSize.x, 0.1f, areaSize.z));

        if (seatTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(seatTransform.position, 0.2f);
        }
    }
#endif
}
