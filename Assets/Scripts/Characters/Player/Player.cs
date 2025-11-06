using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Properties

    #region Movement Properties

    [Header(" └─ Movement")]
    public Vector2 moveInput;

    public CharacterController controller;
    protected Vector3 move;
    protected Vector3 desiredMove;
    protected Quaternion rotation;
    public float rotateSpeed = 10.0f;
    public float _moveSpeed = 10.0f;

    public float moveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    public float currentSpeed = 0f; // Velocidade atual do player

    [Header(" └─ Jump")]
    public float jumpForce = 7f;

    public float verticalVelocity;

    public float gravity = -20f;
    public int jumpCount = 0;
    public int maxJumps = 2;
    private bool isGliding = false;
    public float glideGravity = -3f;

    public float maxRollAngle = 30f; // Ângulo máximo de inclinação ao planar

    // Controle de tempo de planagem
    public float maxGlideTime = 5f; // Tempo máximo de planagem em segundos
    private float currentGlideTime = 0f; // Tempo atual de planagem

    #endregion Movement Properties

    #region Data Properties

    [Header(" └─ Data")]
    protected new string name;

    public string _name
    {
        get { return name; }
        set { name = value; }
    }

    #endregion Data Properties

    #region Skills Properties

    [Header(" └─ Skills")]
    protected List<string> skills = new List<string>();

    public List<string> Skills
    {
        get { return skills; }
        set { skills = value; }
    }

    #endregion Skills Properties

    #region Health Properties

    [Header(" └─ Health")]
    protected int life;

    public int _life
    {
        get { return life; }
        set { life = value; }
    }

    protected int maxLife;

    public int MaxLife
    {
        get { return maxLife; }
        set { maxLife = value; }
    }

    [Header("Player Respawn")]
    private Player player; // Referência ao script do player
    private Vector3 initialSpawnPoint;
    public Vector3 lastCheckpointPosition { get; private set; }

    #endregion Health Properties

    #region Unity Tools Properties

    [Header(" └─ Camera")]
    public GameObject mainCamera;

    private CameraController cameraController;

    public GameObject planador;

    #endregion Unity Tools Properties

    #region Platform Movement Properties

    [Header(" └─ Moving Platform")]
    private Transform currentPlatform; // Plataforma atual que o player está pisando
    private Vector3 lastPlatformPosition; // Última posição da plataforma
    private bool isOnPlatform = false; // Se o player está sobre uma plataforma
    private PlataformaMover platformScript; // Referência ao script da plataforma

    #endregion Platform Movement Properties

    #endregion Properties

    #region Methods

    #region Unity Methods


    private void Start()
    {
        Debug.Log("[Player] Start() chamado!");

        // Inicializa CharacterController
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("[Player] CharacterController não encontrado!");
        }

        // Inicializa CameraController com verificação de segurança
        if (mainCamera != null)
        {
            cameraController = mainCamera.GetComponent<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("[Player] CameraController não encontrado no GameObject mainCamera!");
            }
        }
        else
        {
            Debug.LogError("[Player] MainCamera não está atribuído no Inspector!");
        }

        // Verifica se AudioManager está disponível no Start
        CheckAudioManagerAvailability();

    }

    private void CheckAudioManagerAvailability()
    {
        if (AudioManager.IsAvailable())
        {
            Debug.Log("[Player] AudioManager está disponível!");
        }
        else
        {
            Debug.LogWarning("[Player] AudioManager não está disponível no Start!");

            // Tenta novamente após um frame
            Invoke(nameof(DelayedAudioManagerCheck), 0.1f);
        }
    }

    private void DelayedAudioManagerCheck()
    {
        if (AudioManager.IsAvailable())
        {
            Debug.Log("[Player] AudioManager encontrado após delay!");
        }
        else
        {
            Debug.LogError("[Player] AudioManager ainda não está disponível após delay!");
        }
    }

    private void Update()
    {
        HandlePlatformMovement(); // Deve ser chamado ANTES do movimento do player
        HandlePlayerMovement();
        HandlePlayerJump();


    }

    #endregion Unity Methods

    #region Input Methods

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // Lê o valor do input do sistema de entrada
        moveInput = context.ReadValue<Vector2>();
    }

    // TESTE DO DANDAN
    /*public void OnJumpInput(InputAction.CallbackContext context)
    {
        // Permite pular se PRESSINADO o botão e não excedeu o número máximo de pulos
        if (context.performed && jumpCount < maxJumps)
        {
            verticalVelocity = jumpForce;
            jumpCount++;

            // Reproduz o som do pulo usando método mais seguro
            PlayJumpSound();
        }
        // Ativa o glide se estiver no ar, já usou o double jump e a tecla de pulo está pressionada
        else if (context.performed && !controller.isGrounded && jumpCount >= maxJumps && !isGliding)
        {
            isGliding = true;
            if (planador != null)
            {
                PlayGlideSound();
                planador.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[Player] GameObject planador não está atribuído!");
            }
        }
        // Desativa o glide ao soltar a tecla de pulo ou ao tocar o chão
        else if (context.canceled || controller.isGrounded)
        {
            isGliding = false;
            if (planador != null)
            {
                planador.SetActive(false);
            }
        }
    }*/

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // Permite pular se pressionou o botão e não excedeu o número máximo de pulos
        if (context.performed && jumpCount < maxJumps)
        {
            verticalVelocity = jumpForce;
            jumpCount++;
            PlayJumpSound();
        }
        // Ativa o glide se estiver no ar, já usou o double jump e a tecla de pulo está pressionada
        else if (context.performed && !controller.isGrounded && jumpCount >= maxJumps && !isGliding)
        {
            isGliding = true;
            currentGlideTime = 0f; // Reseta o timer ao iniciar o glide
            if (planador != null)
            {
                PlayGlideSound(); // INICIA O SOM
                planador.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[Player] GameObject planador não está atribuído!");
            }
        }
        // Desativa o glide AO SOLTAR a tecla de pulo
        else if (context.canceled)
        {
            if (isGliding) // Só executa se estava planando
            {
                isGliding = false;
                StopGlideSound(); // PARA O SOM
                if (planador != null)
                {
                    planador.SetActive(false);
                }
            }
        }
    }

    #endregion Input Methods

    #region Audio Methods

    private void PlayJumpSound()
    {
        Debug.Log("[Player] Tentando reproduzir som de pulo...");

        // Usa o método estático mais seguro
        AudioManager audioManager = AudioManager.GetInstance();

        if (audioManager != null)
        {
            //Debug.Log("[Player] AudioManager encontrado, reproduzindo som...");
            audioManager.PlayJumpSound();
        }
        else
        {
            //Debug.LogWarning("[Player] AudioManager não disponível - som de pulo ignorado!");
        }
    }

    private void PlayGlideSound()
    {
        // Usa o método estático mais seguro
        AudioManager audioManager = AudioManager.GetInstance();

        if (audioManager != null)
        {
            Debug.Log("[Player] AudioManager encontrado, reproduzindo som...");
            audioManager.PlayGlideSound();
        }
        else
        {
            Debug.LogWarning("[Player] AudioManager não disponível - som de pulo ignorado!");
        }
    }

    private void StopGlideSound()
    {
        AudioManager audioManager = AudioManager.GetInstance();

        if (audioManager != null)
        {
            audioManager.StopGlideSound();
        }
        else
        {
            Debug.LogWarning("[Player] AudioManager não disponível - não foi possível parar o som de glide!");
        }
    }

    #endregion Audio Methods

    #region Movement Methods

    private void HandlePlatformMovement()
    {
        // Se estiver sobre uma plataforma e ela existe
        if (isOnPlatform && currentPlatform != null && platformScript != null && controller != null)
        {
            // Pega o movimento da plataforma diretamente do script
            Vector3 platformMovement = platformScript.GetMovementThisFrame();

            // Debug para ver o movimento da plataforma
            if (platformMovement.magnitude > 0.001f)
            {
                Debug.Log($"[Player] Movimento da plataforma: {platformMovement}");
            }

            // Move o player junto com a plataforma
            controller.Move(platformMovement);
        }
    }

    private void HandlePlayerMovement()
    {
        // Verifica se o cameraController existe antes de usar
        if (cameraController == null)
        {
            // Usa movimento padrão se não houver camera controller
            desiredMove = (Vector3.forward * moveInput.y) + (Vector3.right * moveInput.x);
        }
        else
        {
            switch (cameraController.name)
            {
                case "CameraThirdPerson":
                    desiredMove = (cameraController.camForward * moveInput.y) + (cameraController.camRight * moveInput.x);
                    break;

                case "CameraTopDown":
                    desiredMove = (Vector3.forward * moveInput.y) + (Vector3.right * moveInput.x);
                    break;

                default:
                    desiredMove = (Vector3.forward * moveInput.y) + (Vector3.right * moveInput.x);
                    break;
            }
        }

        // Velocidade constante - se há input, usa velocidade máxima, senão é 0
        if (moveInput.magnitude > 0.1f)
        {
            currentSpeed = moveSpeed;
        }
        else
        {
            currentSpeed = 0f;
        }

        // Normaliza o vetor de movimento e multiplica pela velocidade
        move = desiredMove.normalized * currentSpeed;

        // Aplica movimento vertical (pulo, gravidade e glide)
        move.y = verticalVelocity;

        // Move o player usando o CharacterController
        if (controller != null)
        {
            controller.Move(move * Time.deltaTime);
        }

        // Rotaciona o player para a direção do movimento, se houver input
        if (moveInput.magnitude > 0)
        {
            rotation = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }

        // Chama o método da câmera se existir
        if (cameraController != null)
        {
            cameraController.HandleCamera();
        }
    }

    //TESTE DO DANDAN
    /*private void HandlePlayerJump()
    {
        // Verifica se o controller existe antes de usar
        if (controller == null) return;

        // Aplica gravidade e reseta o contador de pulos ao tocar o chão
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = 0f;
            }
            jumpCount = 0;
            isGliding = false;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        HandlePlayerDoubleJump();
        HandlePlayerGlide();
        
    }*/

    private void HandlePlayerJump()
    {
        // Verifica se o controller existe antes de usar
        if (controller == null) return;

        // Aplica gravidade e reseta o contador de pulos ao tocar o chão
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = 0f;
            }
            jumpCount = 0;

            // *** ADICIONE ESTA VERIFICAÇÃO ***
            // Se o player estava planando quando tocou o chão, pare o planeio
            if (isGliding)
            {
                isGliding = false;
                StopGlideSound(); // PARA O SOM
                if (planador != null)
                {
                    planador.SetActive(false); // Esconde o planador
                }
            }
            // *** FIM DA ADIÇÃO ***
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        HandlePlayerDoubleJump();
        HandlePlayerGlide();
    }

    private void HandlePlayerDoubleJump()
    {
        // TO-DO:
        // Implementar: efeitos visuais, sons, etc
    }

    private void HandlePlayerGlide()
    {
        // Se estiver planando, aplica gravidade reduzida e inclina o player
        if (isGliding && controller != null && !controller.isGrounded)
        {
            // Incrementa o tempo de planagem
            currentGlideTime += Time.deltaTime;

            // Verifica se excedeu o tempo máximo de planagem
            if (currentGlideTime >= maxGlideTime)
            {
                // Desativa o glide automaticamente
                isGliding = false;
                StopGlideSound();
                if (planador != null)
                {
                    planador.SetActive(false);
                }
                Debug.Log("[Player] Tempo máximo de planagem atingido!");
                return; // Sai do método para não aplicar a gravidade de glide
            }

            verticalVelocity += glideGravity * Time.deltaTime;

            // Inclina o player para o lado do movimento
            float roll = maxRollAngle * moveInput.x;
            Quaternion glideRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -roll);
            transform.rotation = Quaternion.Lerp(transform.rotation, glideRotation, Time.deltaTime * 5f);
        }
        else
        {
            // Quando não estiver planando, reseta a inclinação
            Quaternion resetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, resetRotation, Time.deltaTime * 5f);
        }
    }

    public void Respawn(Vector3 respawnPosition)
    {
        Debug.Log($"[Player] Respawnando em {respawnPosition}");

        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        // 1. Desabilita o CharacterController para permitir o teleporte
        controller.enabled = false;

        // 2. Define a nova posição
        transform.position = respawnPosition;

        // 3. Reabilita o CharacterController
        controller.enabled = true;

        // --- Resetar o Estado do Player ---

        // Reseta todas as velocidades e inputs
        verticalVelocity = 0f;
        currentSpeed = 0f;
        moveInput = Vector2.zero;
        jumpCount = 0;

        // Garante que o planador seja desativado se o player morrer planando
        if (isGliding)
        {
            isGliding = false;
            currentGlideTime = 0f; // Reseta o timer de planagem
            StopGlideSound();
            if (planador != null)
            {
                planador.SetActive(false);
            }
        }

        // Reseta a rotação para evitar que o player respawne inclinado
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }

    // Detecta quando o player está sobre um trigger da plataforma
    private void OnTriggerStay(Collider other)
    {
        // Verifica se é uma plataforma móvel E se o player está no chão (em cima da plataforma)
        if (other.CompareTag("MovingPlatform") && controller != null && controller.isGrounded)
        {
     if (!isOnPlatform)
   {
      Debug.Log($"[Player] Entrou na plataforma: {other.gameObject.name}");
        }

            isOnPlatform = true;
     currentPlatform = other.transform;
     platformScript = other.GetComponent<PlataformaMover>();

  if (platformScript == null)
      {
    Debug.LogError($"[Player] Plataforma {other.gameObject.name} não tem o script PlataformaMover!");
  }
    }
    }

    // Detecta quando o player sai do trigger da plataforma
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MovingPlatform") && other.transform == currentPlatform)
   {
      Debug.Log("[Player] Saiu da plataforma");
            isOnPlatform = false;
     currentPlatform = null;
       platformScript = null;
  }
    }

    // MÉTODO REMOVIDO - NÃO É MAIS NECESSÁRIO
    /*
  // Detecta quando o CharacterController colide com algo
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verifica se colidiu com uma plataforma móvel (tag "MovingPlatform")
        if (hit.gameObject.CompareTag("MovingPlatform"))
 {
      Debug.Log($"[Player] Colidiu com plataforma: {hit.gameObject.name}, Normal Y: {hit.normal.y}");

            // Verifica se o player está em cima da plataforma
        // A normal aponta para cima (Y positivo) quando estamos pisando por cima
       // Usamos 0.3f para dar uma margem maior
      if (hit.normal.y > 0.3f)
  {
      if (!isOnPlatform) // Log apenas quando começar a pisar
    {
         Debug.Log($"[Player] Agora está sobre a plataforma!");
     }
        isOnPlatform = true;
currentPlatform = hit.transform;
     lastPlatformPosition = currentPlatform.position;
  }
        }
    }
    */

    // MÉTODO REMOVIDO - NÃO É MAIS NECESSÁRIO
    /*
    // Verifica se o player ainda está na plataforma a cada frame
    // Este método é chamado automaticamente pelo Update via HandlePlatformMovement
    private void CheckIfStillOnPlatform()
 {
        if (isOnPlatform && currentPlatform != null)
        {
   // Faz um raycast curto pra baixo para verificar se ainda está na plataforma
            RaycastHit hit;
       float rayDistance = controller.height / 2 + 0.3f; // Um pouco mais que a altura do controller

       if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
      {
       // Verifica se o que está abaixo ainda é a plataforma
              if (hit.transform != currentPlatform)
        {
         Debug.Log("[Player] Saiu da plataforma (não está mais sobre ela)");
         isOnPlatform = false;
         currentPlatform = null;
      }
     }
        else
    {
                // Nada foi detectado abaixo, player está no ar
         Debug.Log("[Player] Saiu da plataforma (não está mais no chão)");
           isOnPlatform = false;
         currentPlatform = null;
   }
  }
    }
    */


    #endregion Movement Methods

    #endregion Methods
}