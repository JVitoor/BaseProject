using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 moveDirection;
    private float moveSpeed;
  private float lifetime;
    private float spawnTime;
    
    private bool isInitialized = false;

    public void Initialize(Vector3 direction, float speed, float lifeTime)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        lifetime = lifeTime;
        spawnTime = Time.time;
        isInitialized = true;
    }
    
    void Update()
    {
        if (!isInitialized)
            return;
      
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
  
        if (lifetime > 0 && Time.time >= spawnTime + lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            return;
     
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
         if (collision.gameObject.CompareTag("Player"))
            return;
        
     Destroy(gameObject);
    }
}
