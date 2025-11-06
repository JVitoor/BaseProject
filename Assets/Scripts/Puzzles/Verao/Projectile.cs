using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 moveDirection;
    private float moveSpeed;
    private float lifetime;
    private float spawnTime;

    private bool isInitialized = false;

    void Awake()
    {
        Debug.Log("[Projectile] Awake - Objeto criado: " + gameObject.name);
    }

    void Start()
    {
        Debug.Log("[Projectile] Start - isInitialized: " + isInitialized);

        Collider[] colliders = GetComponents<Collider>();
        Debug.Log("[Projectile] Número de colliders: " + colliders.Length);
        foreach (Collider col in colliders)
        {
            Debug.Log("[Projectile] Collider: " + col.GetType().Name + " | IsTrigger: " + col.isTrigger);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log("[Projectile] Rigidbody encontrado | IsKinematic: " + rb.isKinematic + " | UseGravity: " + rb.useGravity);
        }
        else
        {
            Debug.LogWarning("[Projectile] Nenhum Rigidbody encontrado!");
        }
    }

    public void Initialize(Vector3 direction, float speed, float lifeTime)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        lifetime = lifeTime;
        spawnTime = Time.time;
        isInitialized = true;

        Debug.Log("[Projectile] Initialize - Direção: " + moveDirection + " | Velocidade: " + moveSpeed + " | Lifetime: " + lifetime);
    }

    void Update()
    {
        if (!isInitialized)
            return;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (lifetime > 0 && Time.time >= spawnTime + lifetime)
        {
            Debug.Log("[Projectile] Tempo de vida esgotado, destruindo...");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Projectile] OnTriggerEnter detectado! Objeto: " + other.name + " | Tag: " + other.tag);

        FlowerTarget flower = other.GetComponent<FlowerTarget>();
        if (flower != null)
        {
            Debug.Log("[Projectile] ? Acertou uma FLOR: " + other.name);
            flower.OnHit();
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("FlowerTarget"))
        {
            Debug.Log("[Projectile] ? Acertou objeto com tag FlowerTarget: " + other.name);

            flower = other.GetComponentInParent<FlowerTarget>();
            if (flower != null)
            {
                flower.OnHit();
            }

            Destroy(gameObject);
            return;
        }

        Debug.Log("[Projectile] Colidiu com objeto não-alvo: " + other.name);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[Projectile] OnCollisionEnter detectado! Objeto: " + collision.gameObject.name + " | Tag: " + collision.gameObject.tag);
        Debug.LogWarning("[Projectile] ATENÇÃO: OnCollisionEnter foi chamado ao invés de OnTriggerEnter! Verifique se os colliders estão marcados como Trigger!");
    }
}
