using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Configurações de Disparo")]
    [Tooltip("Prefab do projétil a ser disparado")]
    public GameObject projectilePrefab;

    [Tooltip("Ponto de onde o projétil será disparado")]
    public Transform firePoint;

    [Header("Comportamento do Projétil")]
    [Tooltip("Velocidade do projétil")]
    public float projectileSpeed = 10f;

    [Tooltip("Tempo de vida do projétil (0 = infinito)")]
    public float projectileLifetime = 5f;

    [Tooltip("Direção de disparo (normalizada automaticamente)")]
    public Vector3 fireDirection = Vector3.forward;

    [Header("Controles de Disparo")]
    [Tooltip("Tecla para disparar")]
    public KeyCode fireKey = KeyCode.Space;

    [Tooltip("Tempo mínimo entre disparos")]
    public float fireRate = 0.5f;

    [Tooltip("Se true, dispara automaticamente")]
    public bool autoFire = false;

    [Header("Configurações do Mouse")]
    [Tooltip("Se true, dispara na direção do mouse")]
    public bool shootTowardsMouse = true;

    [Tooltip("Camada do plano onde o mouse será projetado (ex: Ground)")]
    public LayerMask mouseRaycastLayer;

    [Tooltip("Distância do raycast se não houver plano")]
    public float defaultMouseDistance = 100f;

    private float nextFireTime = 0f;
    private Camera mainCamera;

    void Start()
    {
        // Validação inicial
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[ShooterController] Prefab do projétil não atribuído!");
        }

        if (firePoint == null)
        {
            // Se não houver firePoint, usa a própria posição do objeto
            Debug.LogWarning("[ShooterController] FirePoint não atribuído, usando posição do próprio objeto.");
        }

        // Obtém a câmera principal
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[ShooterController] Câmera principal não encontrada!");
        }
    }

    void Update()
    {
        // Disparo automático
        if (autoFire)
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
        // Disparo manual com tecla
        else if (Input.GetKeyDown(fireKey))
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    /// <summary>
    /// Calcula a direção do mouse em relação ao ponto de disparo
    /// </summary>
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

        // Tenta fazer raycast no plano especificado
        if (mouseRaycastLayer != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseRaycastLayer))
            {
                targetPoint = hit.point;
            }
            else
            {
                // Se não acertar nada, usa uma distância padrão
                targetPoint = ray.GetPoint(defaultMouseDistance);
            }
        }
        else
        {
            // Se não houver layer especificado, usa qualquer coisa
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                targetPoint = hit.point;
            }
            else
            {
                // Se não acertar nada, usa uma distância padrão
                targetPoint = ray.GetPoint(defaultMouseDistance);
            }
        }

        // Calcula a direção do ponto de disparo até o ponto alvo
        Vector3 direction = (targetPoint - shootFrom).normalized;

        return direction;
    }

    /// <summary>
    /// Dispara um projétil
    /// </summary>
    public void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[ShooterController] Não é possível disparar: prefab não atribuído!");
            return;
        }

        // Determina a posição de spawn
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        // Determina a direção do disparo
        Vector3 shootDirection = shootTowardsMouse ? GetMouseDirection() : fireDirection.normalized;

        // Determina a rotação baseada na direção de disparo
        Quaternion spawnRotation = Quaternion.LookRotation(shootDirection);

        // Instancia o projétil
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        // Adiciona o componente de movimento se não existir
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript == null)
        {
            projectileScript = projectile.AddComponent<Projectile>();
        }

        // Configura o projétil
        projectileScript.Initialize(shootDirection, projectileSpeed, projectileLifetime);

        Debug.Log("[ShooterController] Projétil disparado na direção: " + shootDirection);
    }

    /// <summary>
    /// Método público para disparar através de eventos ou outros scripts
    /// </summary>
    public void TriggerFire()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }
}
