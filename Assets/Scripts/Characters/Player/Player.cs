using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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

    [Header(" └─ Glide Settings")]
    [Tooltip("Tempo máximo de planeio em segundos")]
    public float maxGlideTime = 3f;

    [Tooltip("Tempo atual de planeio")]
    private float currentGlideTime = 0f;

    [Header(" └─ Slope Settings")]
    [Tooltip("Ângulo máximo antes do player começar a escorregar")]
    public float slopeLimit = 15f;

    [Tooltip("Força aplicada quando o player está escorregando")]
    public float slideForce = 200f;

    private Vector3 velocity = Vector3.zero;

    [Header(" └─ Log Movement")]
    private Transform currentLog; // Referência ao tronco atual
    private Vector3 lastLogPosition; // Última posição do tronco
    private Quaternion lastLogRotation; // Última rotação do tronco
    private bool isOnMovingPlatform = false; // Flag para saber se está em plataforma móvel

    #endregion Movement Properties

    #region Season Abilities Control

    [Header(" └─ Season Abilities")]
    [Tooltip("Habilidades habilitadas conforme a estação")]
    private bool canJump = true;
    private bool canDoubleJump = false;
    private bool canGlide = false;

    #endregion Season Abilities Control

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

    [Header(" └─ Hot Floor VFX")]
    [Tooltip("Prefab com Visual Effect de fumaça para chão quente")]
    public GameObject smokeVFXPrefab;

    private VisualEffect currentSmokeVFX;

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

        // Atualiza habilidades baseado na estação atual
        UpdateSeasonAbilities();
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

    #region Season Methods


    public void UpdateSeasonAbilities()
    {
        if (SeasonManager.Instance == null)
        {
            Debug.LogWarning("[Player] SeasonManager não encontrado! Usando configurações padrão.");
            canJump = true;
            canDoubleJump = true;
            canGlide = true;
            return;
        }

        canJump = SeasonManager.Instance.IsJumpEnabled();
        canDoubleJump = SeasonManager.Instance.IsDoubleJumpEnabled();
        canGlide = SeasonManager.Instance.IsGlideEnabled();

        Season currentSeason = SeasonManager.Instance.GetCurrentSeason();
        Debug.Log($"[Player] Habilidades atualizadas para {currentSeason}: Jump={canJump}, DoubleJump={canDoubleJump}, Glide={canGlide}");
    }

    #endregion Season Methods

    #region Helper Methods

    private bool IsWinterSeason()
    {
        if (SeasonManager.Instance == null)
        {
            return false; // Se não há SeasonManager, não aplica mecânicas de Inverno
        }

        return SeasonManager.Instance.GetCurrentSeason() == Season.Inverno;
    }

    #endregion Helper Methods

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
        // Ativa o glide se estiver no ar, já usou o double jump e a tecla de pulo estão pressionadas
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
        // Verifica se está em uma lombada íngreme NA FASE DE INVERNO - se sim, não pode pular
        if (IsWinterSeason() && OnSteepSlope(out Vector3 _))
        {
            Debug.Log("[Player] Não é possível pular em uma lombada íngreme!");
            return;
        }

        // Verifica se pode pular (sempre pode em todas as estação)
        if (context.performed && canJump && jumpCount < maxJumps)
        {
            // Se está no primeiro pulo OU se tem duplo pulo habilitado
            if (jumpCount == 0 || (jumpCount > 0 && canDoubleJump))
            {
                // Desanexa do tronco ao pular
                DetachFromLog();

                // Remove efeito de fumaça ao pular (sai do chão quente)
                RemoveSmokeEffect();

                verticalVelocity = jumpForce;
                jumpCount++;
                PlayJumpSound();
            }
            else if (jumpCount > 0 && !canDoubleJump)
            {
                Debug.Log("[Player] Duplo pulo não está disponível nesta estação!");
            }
        }
        // Ativa o glide se estiver no ar, já usou o double jump e a tecla de pulo está pressionada
        else if (context.performed && !controller.isGrounded && jumpCount >= maxJumps && !isGliding && canGlide)
        {
            // Verifica se ainda tem tempo de planeio disponível
            if (currentGlideTime < maxGlideTime)
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
            else
            {
                Debug.Log("[Player] Tempo de planeio esgotado!");
            }
        }
        // Se tentou planar mas não pode
        else if (context.performed && !controller.isGrounded && jumpCount >= maxJumps && !isGliding && !canGlide)
        {
            Debug.Log("[Player] Planeio não está disponível nesta estação!");
        }
        // Desativa o glide AO SOLTAR a tecla de pulo
        else if (context.canceled)
        {
            if (isGliding) // Só executa se estava planando
            {
                StopGliding();
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
            // Se o player está anexado a um tronco
            if (currentLog != null)
            {
                // Calcula o movimento do tronco desde o último frame
                Vector3 logMovement = currentLog.position - lastLogPosition;
                Quaternion logRotation = currentLog.rotation * Quaternion.Inverse(lastLogRotation);

                // Aplica o movimento do tronco ao player APENAS se houver movimento significativo
                // Isso evita micromovimentos que causam travadas
                if (logMovement.magnitude > 0.0001f)
                {
                    controller.Move(logMovement);
                }

                // Rotaciona o player junto com o tronco APENAS se houver rotação significativa
                if (Quaternion.Angle(Quaternion.identity, logRotation) > 0.01f)
                {
                    Vector3 playerPosRelativeToLog = transform.position - currentLog.position;
                    Vector3 newPlayerPos = currentLog.position + (logRotation * playerPosRelativeToLog);
                    Vector3 rotationMovement = newPlayerPos - transform.position;

                    if (rotationMovement.magnitude > 0.0001f)
                    {
                        controller.Move(rotationMovement);
                    }
                }

                // Aplica o movimento do próprio player (input)
                if (move.magnitude > 0.0001f)
                {
                    controller.Move(move * Time.deltaTime);
                }

                // ATUALIZA a posição do tronco AQUI, no final do movimento
                lastLogPosition = currentLog.position;
                lastLogRotation = currentLog.rotation;
            }
            else
            {
                // Movimento normal quando não está anexado a nada
                controller.Move(move * Time.deltaTime);

                // Aplica o slope slide APENAS se estiver na fase de Inverno
                if (IsWinterSeason() && OnSteepSlope(out Vector3 slopeDirection))
                {
                    velocity += slopeDirection * slideForce * Time.deltaTime;
                    controller.Move(velocity * Time.deltaTime);
                }
                else
                {
                    // Reseta a velocidade de slide quando não está em slope ou não é Inverno
                    velocity = Vector3.zero;
                }
            }
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

            // Reseta o tempo de planeio quando tocar o chão
            currentGlideTime = 0f;

            // *** ADICIONE ESTA VERIFICAÇÃO ***
            // Se o player estava planando quando tocou o chão, pare o planeio
            if (isGliding)
            {
                StopGliding();
            }
            // *** FIM DA ADIÇÃO ***

        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;

            // Se não está no chão e não está em um tronco, desanexa
            // Isso garante que o player se desanexa ao cair do tronco
            if (currentLog != null)
            {
                // Verifica se ainda está colidindo com o tronco
                // Se não estiver, desanexa
                // Implementação simples: se não está no chão, eventualmente cai
            }
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
            // Incrementa o tempo de planeio
            currentGlideTime += Time.deltaTime;

            // Verifica se atingiu o tempo máximo de planeio
            if (currentGlideTime >= maxGlideTime)
            {
                Debug.Log($"[Player] Tempo máximo de planeio atingido ({maxGlideTime}s)");
                StopGliding();
                return;
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

    private void StopGliding()
    {
        isGliding = false;
        StopGlideSound(); // PARA O SOM
        if (planador != null)
        {
            planador.SetActive(false); // Esconde o planador
        }
    }

    private bool OnSteepSlope(out Vector3 slopeDirection)
    {
        slopeDirection = Vector3.zero;

        // Só verifica slope se estiver no chão
        if (!controller.isGrounded) return false;

        // Lança um raycast para baixo para detectar a superfície
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f))
        {
            // Calcula o ângulo entre a normal da superfície e o vetor "para cima"
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            // Se o ângulo for maior que o limite, está em uma lombada íngreme
            if (angle > slopeLimit)
            {
                // Projeta o vetor "para baixo" no plano da superfície para obter a direção de escorregamento
                slopeDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal);
                return true;
            }
        }

        return false;
    }

    public void Respawn(Vector3 respawnPosition)
    {
        Debug.Log($"[Player] Respawnando em {respawnPosition}");

        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        // Desanexa do tronco antes de respawnar
        DetachFromLog();

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
        currentGlideTime = 0f;
        // Garante que o planador seja desativado se o player morrer planando
        if (isGliding)
        {
            StopGliding();
        }

        // Reseta a rotação para evitar que o player respawne inclinado
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }
    #endregion Movement Methods

    #region Collider Methods

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Detecta colisão com o centro da flor (FlowerController)
        FlowerController flowerController = hit.gameObject.GetComponent<FlowerController>();
        if (flowerController != null)
        {
            flowerController.OnPlayerEnterCenter();
        }

        // Detecta colisão com as pétalas
        PetalController petalController = hit.gameObject.GetComponent<PetalController>();
        if (petalController != null)
        {
            petalController.OnPlayerEnter();
        }

        FallingLeaf fallingLeaf = hit.gameObject.GetComponent<FallingLeaf>();
        if (fallingLeaf != null)
        {
            fallingLeaf.AtivarQueda();
        }

        LogMovement logMovement = hit.gameObject.GetComponent<LogMovement>();
        if (logMovement != null && controller.isGrounded)
        {
            if (currentLog == null || currentLog.gameObject != hit.gameObject)
            {
                AttachToLog(hit.gameObject);
            }
        }

        if (hit.gameObject.CompareTag("VitoriaRegia") && controller.isGrounded)
        {
            if (currentLog == null || currentLog.gameObject != hit.gameObject)
            {
                AttachToLog(hit.gameObject);
            }
        }
    }

    private void AttachToLog(GameObject log)
    {

        currentLog = log.transform;
        lastLogPosition = currentLog.position;
        lastLogRotation = currentLog.rotation;
        isOnMovingPlatform = true;

        Debug.Log($"[Player] Anexado ao tronco: {log.name}");
    }

    public void DetachFromLog()
    {
        // Limpa a referência ao tronco
        if (currentLog != null)
        {
            Debug.Log($"[Player] Desanexado de: {currentLog.name}");
            currentLog = null;
        }

        isOnMovingPlatform = false;

        // Remove parent se existir (segurança)
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
    }

    #endregion Collider Methods

    #region Hot Floor VFX Methods

    public void SpawnSmokeEffect()
    {
        if (smokeVFXPrefab == null)
        {
            Debug.LogWarning("[Player] Smoke VFX Prefab não está configurado!");
            return;
        }

        // Remove efeito anterior se existir (após 1 segundo)
        if (currentSmokeVFX != null)
        {
            currentSmokeVFX.Stop();
            Destroy(currentSmokeVFX.gameObject, 0.75f);
            currentSmokeVFX = null;
        }

        // Instancia o GameObject com Visual Effect como filho do player
        GameObject vfxObject = Instantiate(smokeVFXPrefab, transform);

        // Define a posição LOCAL do efeito (relativa ao player)
        vfxObject.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        vfxObject.transform.localRotation = Quaternion.identity;
        vfxObject.transform.localScale = Vector3.one;

        // Obtém o componente Visual Effect
        currentSmokeVFX = vfxObject.GetComponent<VisualEffect>();

        if (currentSmokeVFX != null)
        {
            // Inicia o Visual Effect
            currentSmokeVFX.Play();
            Debug.Log("[Player] Visual Effect de fumaça iniciado!");
        }
        else
        {
            Debug.LogError("[Player] O prefab não contém um componente VisualEffect!");
            Destroy(vfxObject);
        }
    }

    public void RemoveSmokeEffect()
    {
        if (currentSmokeVFX != null)
        {
            // Para o Visual Effect
            currentSmokeVFX.Stop();
            Debug.Log("[Player] Visual Effect de fumaça parado!");

            // Destrói o efeito após as partículas existentes desaparecerem
            Destroy(currentSmokeVFX.gameObject);
            currentSmokeVFX = null;

            Debug.Log("[Player] Efeito de fumaça removido!");
        }
    }

    #endregion Hot Floor VFX Methods

    #endregion Methods
}