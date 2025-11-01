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

    private float nextFireTime = 0f;

    private Camera mainCamera;

    void Start()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[ShooterController] Prefab do projétil não atribuído!");
        }

        if (firePoint == null)
        {
            Debug.LogWarning("[ShooterController] FirePoint não atribuído, usando posição do próprio objeto.");
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[ShooterController] Câmera principal não encontrada!");
        }
    }

    void Update()
    {
        if (autoFire)
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }

        else if (Input.GetKeyDown(fireKey))
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private Vector3 GetMouseDirection()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("[ShooterController] Câmera não disponível, usando direção padrão.");
            return fireDirection.normalized;
        }

        Vector3 shootFrom = firePoint != null ? firePoint.position : transform.position;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;

        if (mouseRaycastLayer != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseRaycastLayer))
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
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(defaultMouseDistance);
            }
        }

        Vector3 direction = (targetPoint - shootFrom).normalized;

        return direction;
    }

    public void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[ShooterController] Não é possível disparar: prefab não atribuído!");
            return;
        }

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        Vector3 shootDirection = shootTowardsMouse ? GetMouseDirection() : fireDirection.normalized;

        Quaternion spawnRotation = Quaternion.LookRotation(shootDirection);

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript == null)
        {
            projectileScript = projectile.AddComponent<Projectile>();
        }

        projectileScript.Initialize(shootDirection, projectileSpeed, projectileLifetime);

        Debug.Log("[ShooterController] Projétil disparado na direção: " + shootDirection);
    }

    public void TriggerFire()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }
}
