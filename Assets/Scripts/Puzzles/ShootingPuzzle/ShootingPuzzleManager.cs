using TMPro;
using UnityEngine;

public class ShootingPuzzleManager : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Câmera do puzzle de tiro ao alvo")]
    public Camera shootingCamera;

    [Tooltip("Velocidade de rotação da câmera com o mouse")]
    public float cameraSensitivity = 2f;

    [Tooltip("Limite vertical da rotação da câmera (em graus)")]
    public float verticalRotationLimit = 60f;

    [Header("Shooting Settings")]
    [Tooltip("Prefab do projétil")]
    public GameObject projectilePrefab;

    [Tooltip("Ponto de spawn do projétil (perto da câmera)")]
    public Transform projectileSpawnPoint;

    [Tooltip("Força do disparo")]
    public float shootForce = 30f;

    [Tooltip("Tempo de cooldown entre disparos (em segundos)")]
    public float shootCooldown = 0.5f;

    [Header("Target Settings")]
    [Tooltip("Número total de alvos que precisam ser acertados")]
    public int totalTargets = 5;

    [Tooltip("Array com os alvos do puzzle")]
    public Target[] targets;

    [Header("Audio Settings")]
    [Tooltip("Som ao disparar")]
    public AudioClip shootSound;

    [Tooltip("Som ao acertar um alvo")]
    public AudioClip hitSound;

    [Tooltip("Som de vitória")]
    public AudioClip victorySound;

    [Header("UI Settings")]
    [Tooltip("Texto para mostrar progresso (ex: 3/5 alvos)")]
    public TextMeshProUGUI progressText;

    [Tooltip("Texto de feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Parede de Contenção")]
    [Tooltip("Parede que impede o jogador de sair")]
    public GameObject puzzleWall;

    [Header("Obstacle Settings")]
    [Tooltip("Objeto que bloqueia o caminho e será removido ao completar")]
    public GameObject obstacleToRemove;

    [Header("Debug Settings")]
    [Tooltip("Mostrar linha de debug da mira?")]
    public bool showAimDebug = true;

    // Estado do puzzle
    private bool puzzleActive = false;
    private bool puzzleCompleted = false;
    private int targetsHit = 0;

    // Referências
    private Player player;
    private Camera mainCamera;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    // Controle de input do player
    private bool playerMovementDisabled = false;

    // Controle de cooldown
    private float lastShootTime = 0f;

    private void Start()
    {
        // Garante que a câmera do puzzle começa desativada
        if (shootingCamera != null)
        {
            shootingCamera.gameObject.SetActive(false);
        }

        // Desativa a parede no início
        if (puzzleWall != null)
        {
            puzzleWall.SetActive(false);
        }

        // Inicializa os alvos
        if (targets != null)
        {
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Initialize(this);
                }
            }
        }

        UpdateProgressUI();
    }

    public void StartPuzzle()
    {
        if (puzzleActive || puzzleCompleted)
        {
            Debug.Log("[ShootingPuzzleManager] Puzzle já foi iniciado ou completado!");
            return;
        }

        Debug.Log("[ShootingPuzzleManager] Iniciando puzzle de tiro ao alvo!");

        // Encontra o player
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("[ShootingPuzzleManager] Player não encontrado!");
            return;
        }

        // Salva a câmera principal
        mainCamera = Camera.main;

        // Ativa a parede
        if (puzzleWall != null)
        {
            puzzleWall.SetActive(true);
        }

        // Desabilita movimento do player
        DisablePlayerMovement();

        // Ativa a câmera do puzzle
        if (shootingCamera != null)
        {
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(false);
            }

            shootingCamera.gameObject.SetActive(true);

            // Inicializa as rotações baseado na orientação atual da câmera
            Vector3 currentRotation = shootingCamera.transform.eulerAngles;
            horizontalRotation = currentRotation.y;
            verticalRotation = currentRotation.x;

            // Normaliza o ângulo vertical para o range -180 a 180
            if (verticalRotation > 180f)
                verticalRotation -= 360f;
        }

        // Mantém o cursor bloqueado e invisível (estilo FPS)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        puzzleActive = true;

        UpdateFeedbackText("Use o mouse para mirar e clique esquerdo para atirar!");
        UpdateProgressUI();
    }

    private void Update()
    {
        if (!puzzleActive || puzzleCompleted) return;

        HandleCameraRotation();
        HandleShooting();

        // Debug visual da mira
        if (showAimDebug)
        {
            DrawAimDebug();
        }
    }

    private void HandleCameraRotation()
    {
        if (shootingCamera == null) return;

        // Rotação horizontal (Y) - sem limites
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        horizontalRotation += mouseX;

        // Rotação vertical (X) - com limites
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

        // Aplica a rotação combinada
        shootingCamera.transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }

    private void HandleShooting()
    {
        // Dispara com o botão esquerdo do mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Verifica se o cooldown já passou
            if (Time.time >= lastShootTime + shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[ShootingPuzzleManager] Projectile prefab não está atribuído!");
            return;
        }

        if (shootingCamera == null)
        {
            Debug.LogError("[ShootingPuzzleManager] Shooting camera não está atribuída!");
            return;
        }

        // Direção do tiro = direção que a câmera está olhando
        Vector3 shootDirection = shootingCamera.transform.forward;

        // Posição de spawn do projétil - AFASTADO 1 metro na direção do tiro
        Vector3 spawnPosition;
        if (projectileSpawnPoint != null)
        {
            // Spawna 1 metro à frente do spawn point
            spawnPosition = projectileSpawnPoint.position + shootDirection * 1f;
        }
        else
        {
            // Spawna 2 metros à frente da câmera
            spawnPosition = shootingCamera.transform.position + shootDirection * 2f;
        }

        Debug.Log($"[ShootingPuzzleManager] Disparando da posição: {spawnPosition} na direção: {shootDirection}");

        // Instancia o projétil
        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.LookRotation(shootDirection)
        );

        // Aplica a velocidade ao projétil
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection * shootForce;
            Debug.Log($"[ShootingPuzzleManager] Velocidade aplicada: {rb.linearVelocity}");
        }
        else
        {
            Debug.LogError("[ShootingPuzzleManager] Projétil não possui Rigidbody!");
        }

        // Debug visual
        if (showAimDebug)
        {
            Debug.DrawRay(spawnPosition, shootDirection * 50f, Color.yellow, 3f);
        }

        // Toca som de disparo
        PlaySound(shootSound);
    }

    private void DrawAimDebug()
    {
        if (shootingCamera == null) return;

        // Desenha uma linha vermelha mostrando para onde a câmera está olhando
        Debug.DrawRay(shootingCamera.transform.position, shootingCamera.transform.forward * 100f, Color.red);
    }

    public void OnTargetHit(Target target)
    {
        targetsHit++;

        Debug.Log($"[ShootingPuzzleManager] Alvo acertado! {targetsHit}/{totalTargets}");

        // Toca som de acerto
        PlaySound(hitSound);

        UpdateProgressUI();
        UpdateFeedbackText($"Alvo acertado! {targetsHit}/{totalTargets}");

        // Verifica se completou o puzzle
        if (targetsHit >= totalTargets)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        Debug.Log("[ShootingPuzzleManager] Puzzle completado!");

        puzzleActive = false;
        puzzleCompleted = true;

        UpdateFeedbackText("Puzzle completado! Parabéns!");

        // Toca som de vitória
        PlaySound(victorySound);

        // Remove o obstáculo
        if (obstacleToRemove != null)
        {
            Destroy(obstacleToRemove);
        }

        // Aguarda um pouco e depois restaura o controle do player
        Invoke(nameof(EndPuzzle), 2f);
    }

    private void EndPuzzle()
    {
        // Desativa a câmera do puzzle
        if (shootingCamera != null)
        {
            shootingCamera.gameObject.SetActive(false);
        }

        // Reativa a câmera principal
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }

        // Reabilita movimento do player
        EnablePlayerMovement();

        // Desativa a parede
        if (puzzleWall != null)
        {
            puzzleWall.SetActive(false);
        }

        // Bloqueia o cursor novamente
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("[ShootingPuzzleManager] Controle retornado ao player!");
    }

    private void DisablePlayerMovement()
    {
        if (player == null) return;

        // Desabilita o CharacterController
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // Desabilita o script do Player
        player.enabled = false;

        playerMovementDisabled = true;

        Debug.Log("[ShootingPuzzleManager] Movimento do player desabilitado!");
    }

    private void EnablePlayerMovement()
    {
        if (player == null) return;

        // Reabilita o CharacterController
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = true;
        }

        // Reabilita o script do Player
        player.enabled = true;

        playerMovementDisabled = false;

        Debug.Log("[ShootingPuzzleManager] Movimento do player habilitado!");
    }

    private void UpdateProgressUI()
    {
        if (progressText != null)
        {
            progressText.text = $"Alvos: {targetsHit}/{totalTargets}";
        }
    }

    private void UpdateFeedbackText(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }

        Debug.Log($"[ShootingPuzzleManager] {message}");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        AudioManager audioManager = AudioManager.GetInstance();
        if (audioManager != null)
        {
            audioManager.PlaySFX(clip);
        }
    }
}
