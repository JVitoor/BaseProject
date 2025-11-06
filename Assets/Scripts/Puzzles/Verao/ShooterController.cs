using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Configurações de Disparo")]
    public GameObject projectilePrefab;

    public Transform firePoint;

    [Header("Comportamento do Projétil")]
    public float projectileSpeed = 10f;

    public float projectileLifetime = 5f;

    public Vector3 fireDirection = Vector3.forward;

    [Header("Controles de Disparo")]
    public KeyCode fireKey = KeyCode.Space;

    public float fireRate = 0.5f;

    public bool autoFire = false;

    [Header("Configurações do Mouse")]
    public bool shootTowardsMouse = true;

    public LayerMask mouseRaycastLayer;

    public float defaultMouseDistance = 100f;

    [Header("Performance")]
    [SerializeField] private bool cacheProjectileComponent = true;

    private float nextFireTime = 0f;

    private Camera mainCamera;
    private bool hasCachedCamera = false;
    private Transform cachedTransform;
    private Vector3 cachedFirePointPosition;
    private bool hasFirePoint;

    void Awake()
    {
        // Cache transform reference
        cachedTransform = transform;
    }

    void Start()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[ShooterController] Prefab do projétil não atribuído!");
        }

        hasFirePoint = firePoint != null;
        
        if (!hasFirePoint)
        {
            Debug.LogWarning("[ShooterController] FirePoint não atribuído, usando posição do próprio objeto.");
        }

        CacheMainCamera();
    }

    private void CacheMainCamera()
    {
        mainCamera = Camera.main;
        hasCachedCamera = mainCamera != null;
        
        if (!hasCachedCamera)
        {
            Debug.LogError("[ShooterController] Câmera principal não encontrada!");
        }
    }

    void Update()
    {
        // Check if we need to re-cache camera (in case it changes)
        if (!hasCachedCamera || mainCamera == null)
        {
            CacheMainCamera();
        }

        bool shouldFire = false;

        if (autoFire)
        {
            shouldFire = Time.time >= nextFireTime;
        }
        else if (Input.GetKeyDown(fireKey))
        {
            shouldFire = Time.time >= nextFireTime;
        }

        if (shouldFire)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private Vector3 GetMouseDirection()
    {
        if (!hasCachedCamera || mainCamera == null)
        {
            Debug.LogWarning("[ShooterController] Câmera não disponível, usando direção padrão.");
            return fireDirection.normalized;
        }

        Vector3 shootFrom = hasFirePoint ? firePoint.position : cachedTransform.position;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;

        // Optimize raycast by checking layer mask first
        if (mouseRaycastLayer != 0)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, defaultMouseDistance, mouseRaycastLayer))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(defaultMouseDistance);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hit, defaultMouseDistance))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(defaultMouseDistance);
            }
        }

        return (targetPoint - shootFrom).normalized;
    }

    public void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[ShooterController] Não é possível disparar: prefab não atribuído!");
            return;
        }

        Vector3 spawnPosition = hasFirePoint ? firePoint.position : cachedTransform.position;

        Vector3 shootDirection = shootTowardsMouse ? GetMouseDirection() : fireDirection.normalized;

        Quaternion spawnRotation = Quaternion.LookRotation(shootDirection);

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        // Optimize: Try to get cached component from prefab or add one
        Projectile projectileScript;
        
        if (cacheProjectileComponent)
        {
            projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript == null)
            {
                projectileScript = projectile.AddComponent<Projectile>();
            }
        }
        else
        {
            projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript == null)
            {
                projectileScript = projectile.AddComponent<Projectile>();
            }
        }

        projectileScript.Initialize(shootDirection, projectileSpeed, projectileLifetime);

        #if UNITY_EDITOR
        Debug.Log("[ShooterController] Projétil disparado na direção: " + shootDirection);
        #endif
    }

    public void TriggerFire()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    // Public method to update camera reference (useful if camera changes)
    public void UpdateCameraReference(Camera newCamera)
    {
        mainCamera = newCamera;
        hasCachedCamera = mainCamera != null;
    }
}
