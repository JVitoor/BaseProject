using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Tempo de vida do projétil em segundos")]
    public float lifetime = 5f;

    [Tooltip("Efeito de impacto (opcional)")]
    public GameObject impactEffect;

    [Tooltip("Destruir ao colidir?")]
    public bool destroyOnImpact = true;

    [Header("Movement Settings (Opcional)")]
    [Tooltip("Usar movimento manual ao invés de física?")]
    public bool useManualMovement = false;

    [Tooltip("Velocidade do movimento manual")]
    public float manualSpeed = 30f;

    [Header("Collision Protection")]
    [Tooltip("Tempo de proteção contra colisões após spawn")]
    public float collisionProtectionTime = 0.1f;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private float spawnTime;

    private void Start()
    {
        // Marca o tempo de spawn
        spawnTime = Time.time;

        // Destrói o projétil automaticamente após o tempo de vida
        Destroy(gameObject, lifetime);

        // Se usar movimento manual, desabilita gravidade e salva direção
        if (useManualMovement)
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                moveDirection = transform.forward;
            }
        }

        Debug.Log($"[Projectile] Projétil criado em {transform.position}");
    }

    private void Update()
    {
        // Movimento manual (alternativa à física)
        if (useManualMovement)
        {
            transform.position += moveDirection * manualSpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignora colisões nos primeiros frames após o spawn
        if (Time.time - spawnTime < collisionProtectionTime)
        {
            Debug.Log($"[Projectile] Colisão ignorada (proteção ativa) com {collision.gameObject.name}");
            return;
        }

        // Spawna efeito de impacto
        if (impactEffect != null)
        {
            // Pega o ponto de contato da colisão
            ContactPoint contact = collision.contacts[0];

            // Spawna o efeito no ponto de impacto com a rotação normal à superfície
            Quaternion rotation = Quaternion.LookRotation(contact.normal);
            Instantiate(impactEffect, contact.point, rotation);
        }

        Debug.Log($"[Projectile] Colidiu com {collision.gameObject.name}");

        // Destrói o projétil se configurado
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignora colisões nos primeiros frames após o spawn
        if (Time.time - spawnTime < collisionProtectionTime)
        {
            Debug.Log($"[Projectile] Trigger ignorado (proteção ativa) com {other.gameObject.name}");
            return;
        }

        // Alternativa para triggers
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        Debug.Log($"[Projectile] Colidiu (trigger) com {other.gameObject.name}");

        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}
