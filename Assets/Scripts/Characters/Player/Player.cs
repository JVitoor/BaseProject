using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Properties

    #region Movement Properties

    [Header(" └─ Movement")]
    public Vector2 moveInput;

    protected CharacterController controller;
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

    [Header(" └─ Acceleration")]
    public float acceleration = 15.0f; // Velocidade de aceleração
    public float deceleration = 20.0f; // Velocidade de desaceleração
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

    // TRABALHO DE PDJ - EMPURRAR:
    private Pushable pushableTarget; // Pega o target que pode ser empurrado
    public float pushForce = 10f; // Força aplicada ao empurrar

    // TRABALHO DE PDJ - PUXAR:
    private Pullable pullTarget; // Pega o target que pode ser puxado
    private Transform pullPoint; // Ponto de ancoragem para o objeto puxado

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

        // TRABALHO DE PDJ - PUXAR

        pullPoint = new GameObject("PullPoint").transform; // Cria um novo GameObject vazio para ser o ponto de pull
        pullPoint.SetParent(transform); // Define o player como pai do ponto de pull
        pullPoint.localPosition = new Vector3(0, 1f, 1.5f); // Posiciona o ponto de pull na frente do player
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

    public void OnJumpInput(InputAction.CallbackContext context)
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
    }

    // TRABALHO DE PDJ - EMPURRAR:
    public void OnPushInput(InputAction.CallbackContext context) // Método chamado ao pressionar "E"
    {
        if (context.performed && pushableTarget != null) // Verifica se o botão foi pressionado e se há um objeto para empurrar
        {
            float distance = Vector3.Distance(transform.position, pushableTarget.transform.position); // Calcula a distância entre o player e o objeto

            if (distance <= 2f) // Verifica se está dentro do alcance para empurrar
            {
                Vector3 pushDir = transform.forward; // Direção do empurrão (para frente do player)
                pushableTarget.Push(pushDir, pushForce); // Chama o método Push no objeto pushable
            }
            else // Se estiver fora do alcance
            {
                pushableTarget = null; // Reseta o pushableTarget
            }
        }
    }

    // TRABALHO DE PDJ - PUXAR:
    public void OnPullInput(InputAction.CallbackContext context) // Método chamado pelo Input System ao pressionar o botão de puxar
    {
        if (pullTarget == null) return; // Se não houver objeto para puxar, sai do método

        if (context.performed) // Se o botão foi pressionado
        {
            pullTarget.SetKinematic(true); // Define o objeto como kinematic para evitar física indesejada

            pullTarget.transform.SetParent(pullPoint); // Define o ponto de pull como pai do objeto puxado

            pullTarget.transform.localPosition = Vector3.zero; // Posiciona o objeto puxado no ponto de pull
        }
        else if (context.canceled) // Se o botão foi solto
        {
            pullTarget.SetKinematic(false); // Reseta o objeto para não ser mais kinematic

            pullTarget.transform.SetParent(null); // Remove o pai do objeto puxado

            pullTarget = null; // Reseta o pullTarget
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

        // Sistema de aceleração
        HandleAcceleration();

        // Normaliza o vetor de movimento e multiplica pela velocidade atual (com aceleração)
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
            isGliding = false;
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

    private void HandleAcceleration()
    {
        // Se há input de movimento, acelera até a velocidade máxima
        if (moveInput.magnitude > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
        }
        // Se não há input, desacelera até parar
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }
    }

    // TRABALHO DE PDJ - EMPURRAR/PUXAR:
    private void OnControllerColliderHit(ControllerColliderHit hit) // Detecta colisões com o CharacterController
    {
        // EMPURRAR:

        Pushable pushable = hit.collider.GetComponent<Pushable>(); // Verifica se o objeto colidido tem o script Pushable

        if (pushable != null) // Se for um objeto empurrável
        {
            pushableTarget = pushable; // Define o pushableTarget como o objeto colidido
        }

        // PUXAR:

        Pullable pullable = hit.collider.GetComponent<Pullable>(); // Verifica se o objeto colidido tem o componente Pullable

        if (pullable != null) // Se for um objeto puxável
        {
            pullTarget = pullable; // Define o pullTarget como o objeto colidido
        }
    }

    #endregion Movement Methods

    #endregion Methods
}