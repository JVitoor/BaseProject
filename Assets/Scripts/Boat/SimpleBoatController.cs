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

    // Estado
    private bool isOccupied = false;
    private bool playerInRange = false;
    private GameObject currentPlayer;

    // Guarda a posição/rota original do jogador para retornar ao sair
    private Vector3 savedPlayerPosition;
    private Quaternion savedPlayerRotation;

    // Componentes do jogador que podem ser desativados
    private CharacterController playerCC;
    private MonoBehaviour playerMovementScript;

    private void Awake()
    {
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

        // Se não houver collider (não-trigger), adiciona um para que OnTrigger funcione
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sc = gameObject.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            interactionTrigger = sc;
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
                EnterBoat();
            }
            else if (isOccupied && currentPlayer != null)
            {
                ExitBoat();
            }
        }
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
    }

    private void ExitBoat()
    {
        if (currentPlayer == null) return;

        // Desparenta e mantém a posição/rotação atual do jogador (permanece onde está no assento)
        currentPlayer.transform.SetParent(null, true);

        // Não restaura a posição salva: o jogador sai no mesmo local onde estava montado

        // Reativa componentes do jogador
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerCC != null) playerCC.enabled = true;

        isOccupied = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        currentPlayer = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        if (!other.CompareTag("Player")) return;

        // Se o jogador sair do trigger enquanto não estiver no barco, limpa referência
        if (!isOccupied && currentPlayer == other.gameObject)
        {
            currentPlayer = null;
        }

        playerInRange = false;
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
