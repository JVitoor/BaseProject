using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

public class FlowerShooterPuzzle : MonoBehaviour
{
    #region Serialized Fields
    [Header("Flores - Referências na Cena")]
    [SerializeField] private GameObject[] flowers = new GameObject[5];
    
    [Header("Colliders")]
    [SerializeField] private BoxCollider playerEnterCollider;
    
    [Header("Configurações de Animação")]
    [SerializeField] private string loopAnimation = "FlowerLoop";
    [SerializeField] private string closeAnimation = "FlowerClose";
    [SerializeField] private string loopParameter = "isLooping";

    [Header("Configurações")]
    [SerializeField] private bool autoStartLoop = true;
    [SerializeField] private string projectileTag = "Projectile";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool requireAllFlowers = true;
    
    [Header("Camera")]
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraControllerPuzzle cameraController;
    
    [Header("UI - Pontuação")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string scoreFormat = "{0}/5";
    [SerializeField] private bool showScoreUI = true;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onFlowerHit;
    [SerializeField] private UnityEvent onPuzzleComplete;
    [SerializeField] private UnityEvent onPlayerEnter;
    [SerializeField] private UnityEvent onPlayerExit;
    #endregion

    #region Private Fields
    private List<FlowerInstance> flowerInstances = new List<FlowerInstance>();
    private Dictionary<Collider, FlowerInstance> flowerColliderMap = new Dictionary<Collider, FlowerInstance>();
    private int loopParameterHash;
    private int loopAnimationHash;
    private int closeAnimationHash;
    private bool puzzleCompleted = false;
    private bool playerInPuzzleArea = false;
    private int flowersHit = 0;
    private int projectileTagHash;
    private int playerTagHash;
    private string cachedScoreText;
    #endregion

    #region Helper Class
    [System.Serializable]
    private class FlowerInstance
    {
        public GameObject gameObject;
        public Animator animator;
        public BoxCollider hitCollider;
        public bool isHit = false;
        public int index;

        public FlowerInstance(GameObject go, Animator anim, BoxCollider collider, int idx)
        {
            gameObject = go;
            animator = anim;
            hitCollider = collider;
            index = idx;
            isHit = false;
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        CacheAnimationHashes();
        CacheTagHashes();
        InitializeFlowers();
    }

    private void Start()
    {
        ValidateSetup();
        SetupColliders();
        SetupCameras();
        InitializeScoreUI();

        if (autoStartLoop)
        {
            StartAllFlowersLoop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check projectile hit using dictionary lookup for O(1) performance
        if (other.CompareTag(projectileTag))
        {
            CheckFlowerHitOptimized(other);
            return;
        }

        // Check player enter
        if (other.CompareTag(playerTag) && playerEnterCollider != null)
        {
            OnPlayerEnterPuzzle();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && playerEnterCollider != null && playerInPuzzleArea)
        {
            OnPlayerExitPuzzle();
        }
    }
    #endregion

    #region Initialization
    private void InitializeFlowers()
    {
        flowerInstances.Clear();
        flowerColliderMap.Clear();

        for (int i = 0; i < flowers.Length; i++)
        {
            if (flowers[i] != null)
            {
                SetupFlower(flowers[i], i);
            }
        }

        LogDebug($"[FlowerShooterPuzzle] {flowerInstances.Count} flores inicializadas!");
    }

    private void SetupFlower(GameObject flowerGO, int index)
    {
        if (flowerGO == null)
        {
            Debug.LogWarning($"[FlowerShooterPuzzle] Flor {index} é nula!", this);
            return;
        }

        Animator animator = flowerGO.GetComponentInChildren<Animator>();
        BoxCollider hitCollider = flowerGO.GetComponent<BoxCollider>();

        if (hitCollider == null)
        {
            hitCollider = flowerGO.AddComponent<BoxCollider>();
            hitCollider.isTrigger = true;
            LogDebug($"[FlowerShooterPuzzle] BoxCollider adicionado automaticamente à flor {index}");
        }
        else
        {
            hitCollider.isTrigger = true;
        }

        if (animator == null)
        {
            Debug.LogWarning($"[FlowerShooterPuzzle] Flor {index} ({flowerGO.name}) não possui Animator!", this);
        }

        FlowerInstance flower = new FlowerInstance(flowerGO, animator, hitCollider, index);
        flowerInstances.Add(flower);
        
        // Map collider to flower for fast lookup
        if (hitCollider != null)
        {
            flowerColliderMap[hitCollider] = flower;
        }
    }

    private void SetupColliders()
    {
        if (playerEnterCollider != null)
        {
            playerEnterCollider.isTrigger = true;
        }
    }

    private void SetupCameras()
    {
        if (puzzleCamera != null)
        {
            puzzleCamera.enabled = false;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void CacheAnimationHashes()
    {
        loopParameterHash = Animator.StringToHash(loopParameter);
        loopAnimationHash = Animator.StringToHash(loopAnimation);
        closeAnimationHash = Animator.StringToHash(closeAnimation);
    }

    private void CacheTagHashes()
    {
        // Cache tag comparisons (Unity internally uses hashes but we can reduce string comparisons)
        projectileTagHash = projectileTag.GetHashCode();
        playerTagHash = playerTag.GetHashCode();
    }

    private void ValidateSetup()
    {
        int validFlowers = 0;
        for (int i = 0; i < flowers.Length; i++)
        {
            if (flowers[i] != null)
            {
                validFlowers++;
            }
        }

        if (validFlowers == 0)
        {
            Debug.LogError($"[FlowerShooterPuzzle] Nenhuma flor atribuída em {gameObject.name}!", this);
        }
        else
        {
            LogDebug($"[FlowerShooterPuzzle] {validFlowers} flores configuradas.");
        }

        if (playerEnterCollider == null)
        {
            Debug.LogWarning($"[FlowerShooterPuzzle] Player Enter Collider não configurado em {gameObject.name}!", this);
        }

        if (showScoreUI && scoreText == null)
        {
            Debug.LogWarning($"[FlowerShooterPuzzle] Score Text não configurado em {gameObject.name}. UI de pontuação desabilitada.", this);
            showScoreUI = false;
        }
    }
    #endregion

    #region UI Management
    private void InitializeScoreUI()
    {
        if (showScoreUI && scoreText != null)
        {
            UpdateScoreUI();
            if (scoreText.gameObject.activeSelf && !playerInPuzzleArea)
            {
                scoreText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (showScoreUI && scoreText != null)
        {
            // Cache the formatted string to avoid repeated allocations
            cachedScoreText = string.Format(scoreFormat, flowersHit);
            scoreText.text = cachedScoreText;
            LogDebug($"[FlowerShooterPuzzle] UI atualizada: {flowersHit}/5");
        }
    }

    private void ShowScoreUI()
    {
        if (showScoreUI && scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            UpdateScoreUI();
        }
    }

    private void HideScoreUI()
    {
        if (showScoreUI && scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }
    }

    public void SetScoreText(TextMeshProUGUI newScoreText)
    {
        scoreText = newScoreText;
        UpdateScoreUI();
    }

    public string GetFormattedScore()
    {
        return cachedScoreText ?? string.Format(scoreFormat, flowersHit);
    }
    #endregion

    #region Puzzle Logic - Hit Detection
    // Optimized version using dictionary lookup
    private void CheckFlowerHitOptimized(Collider projectile)
    {
        // Use Physics.OverlapSphere at projectile position for more accurate detection
        Collider[] hitColliders = Physics.OverlapSphere(projectile.transform.position, 0.5f);
        
        foreach (Collider hitCollider in hitColliders)
        {
            if (flowerColliderMap.TryGetValue(hitCollider, out FlowerInstance flower))
            {
                if (!flower.isHit)
                {
                    OnFlowerHit(flower, projectile);
                    return;
                }
            }
        }
    }

    private void CheckFlowerHit(Collider projectile)
    {
        // Legacy method - kept for compatibility, but optimized version is preferred
        CheckFlowerHitOptimized(projectile);
    }

    private void OnFlowerHit(FlowerInstance flower, Collider projectile)
    {
        flower.isHit = true;
        flowersHit++;

        UpdateScoreUI();
        PlayCloseAnimation(flower);
        onFlowerHit?.Invoke();

        LogDebug($"[FlowerShooterPuzzle] Flor {flower.index} ({flower.gameObject.name}) acertada por {projectile.name}! ({flowersHit}/{flowerInstances.Count})");

        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        if (puzzleCompleted) return;

        bool completed = requireAllFlowers ? 
            flowersHit >= flowerInstances.Count : 
            flowersHit > 0;

        if (completed)
        {
            puzzleCompleted = true;
            onPuzzleComplete?.Invoke();
            LogDebug("[FlowerShooterPuzzle] ? Puzzle completado!");
        }
    }

    public bool IsPuzzleCompleted()
    {
        return puzzleCompleted;
    }

    public int GetFlowersHitCount()
    {
        return flowersHit;
    }

    public int GetTotalFlowersCount()
    {
        return flowerInstances.Count;
    }
    #endregion

    #region Puzzle Logic - Player Interaction
    private void OnPlayerEnterPuzzle()
    {
        if (playerInPuzzleArea) return;

        playerInPuzzleArea = true;
        SwitchToPuzzleCamera();
        ShowScoreUI();
        
        if (cameraController != null)
        {
            cameraController.SetActive(true);
        }
        
        onPlayerEnter?.Invoke();
        
        LogDebug($"[FlowerShooterPuzzle] Player entrou na área do puzzle!");
    }

    private void OnPlayerExitPuzzle()
    {
        if (!playerInPuzzleArea) return;

        playerInPuzzleArea = false;
        SwitchToMainCamera();
        HideScoreUI();
        
        if (cameraController != null)
        {
            cameraController.SetActive(false);
        }
        
        onPlayerExit?.Invoke();
        
        LogDebug($"[FlowerShooterPuzzle] Player saiu da área do puzzle!");
    }

    public bool IsPlayerInPuzzleArea()
    {
        return playerInPuzzleArea;
    }
    #endregion

    #region Camera Management
    private void SwitchToPuzzleCamera()
    {
        if (puzzleCamera == null)
        {
            Debug.LogWarning("[FlowerShooterPuzzle] Câmera do puzzle não configurada!", this);
            return;
        }

        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }

        puzzleCamera.enabled = true;
        
        LogDebug("[FlowerShooterPuzzle] Câmera mudada para puzzle camera");
    }

    private void SwitchToMainCamera()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("[FlowerShooterPuzzle] Câmera principal não encontrada!", this);
            return;
        }

        if (puzzleCamera != null)
        {
            puzzleCamera.enabled = false;
        }

        mainCamera.enabled = true;
        
        LogDebug("[FlowerShooterPuzzle] Câmera mudada para main camera");
    }

    public void ForceSwitchCamera(bool usePuzzleCamera)
    {
        if (usePuzzleCamera)
        {
            SwitchToPuzzleCamera();
        }
        else
        {
            SwitchToMainCamera();
        }
    }
    #endregion

    #region Animation Control
    public void StartAllFlowersLoop()
    {
        foreach (var flower in flowerInstances)
        {
            if (!flower.isHit && flower.animator != null)
            {
                StartLoop(flower);
            }
        }

        LogDebug($"[FlowerShooterPuzzle] Loop iniciado em {flowerInstances.Count} flores!");
    }

    private void StartLoop(FlowerInstance flower)
    {
        if (flower.animator == null) return;

        flower.animator.SetBool(loopParameterHash, true);
        flower.animator.Play(loopAnimationHash);
    }

    private void PlayCloseAnimation(FlowerInstance flower)
    {
        if (flower.animator == null) return;

        flower.animator.SetBool(loopParameterHash, false);
        flower.animator.Play(closeAnimationHash);
    }
    #endregion

    #region Reset
    public void ResetPuzzle()
    {
        puzzleCompleted = false;
        flowersHit = 0;

        UpdateScoreUI();

        foreach (var flower in flowerInstances)
        {
            flower.isHit = false;
            
            if (autoStartLoop && flower.animator != null)
            {
                StartLoop(flower);
            }
        }

        if (!playerInPuzzleArea)
        {
            SwitchToMainCamera();
        }
        
        LogDebug("[FlowerShooterPuzzle] Puzzle resetado!");
    }

    public void ReinitializeFlowers()
    {
        InitializeFlowers();
        ResetPuzzle();
        
        LogDebug("[FlowerShooterPuzzle] Flores reinicializadas!");
    }
    #endregion

    #region Debug Helpers
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log(message);
        }
    }
    #endregion

    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (flowers != null && !Application.isPlaying)
        {
            for (int i = 0; i < flowers.Length; i++)
            {
                if (flowers[i] != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(flowers[i].transform.position, 0.3f);
                    
                    UnityEditor.Handles.Label(
                        flowers[i].transform.position + Vector3.up * 0.5f, 
                        $"Flower {i}"
                    );
                }
            }
        }

        if (playerEnterCollider != null)
        {
            Gizmos.color = playerInPuzzleArea ? Color.yellow : Color.cyan;
            Gizmos.matrix = playerEnterCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(playerEnterCollider.center, playerEnterCollider.size);
        }

        if (Application.isPlaying && flowerInstances != null)
        {
            foreach (var flower in flowerInstances)
            {
                if (flower.gameObject != null)
                {
                    Gizmos.color = flower.isHit ? Color.red : Color.green;
                    Gizmos.DrawWireSphere(flower.gameObject.transform.position, 0.5f);
                }
            }
        }
    }
#endif
    #endregion
}
