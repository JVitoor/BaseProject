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
        // Permite pular se pressionou o botão e não excedeu o número máximo de pulos
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
        else if (context.canceled) // <<< REMOVIDO O '|| controller.isGrounded' DAQUI
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
        // TO-DO
        // Se espaço estiver pressionado, o modo planagem continua ativo mesmo tocando o chão

        // Se estiver planando, aplica gravidade reduzida e inclina o player
        if (isGliding && controller != null && !controller.isGrounded)
        {
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
            StopGlideSound();
            if (planador != null)
            {
                planador.SetActive(false);
            }
        }

        // Reseta a rotação para evitar que o player respawne inclinado
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }


    #endregion Movement Methods

    #endregion Methods
}