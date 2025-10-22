using UnityEngine;

public class LogMovement : MonoBehaviour
{
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public Transform destroyPoint;
    public LogSpawner logSpawner;

    public System.Action onDestroyed;

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (destroyPoint != null)
        {
            if (Vector3.Distance(transform.position, destroyPoint.position) < 0.1f)
            {
                onDestroyed?.Invoke();
                Destroy(gameObject);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DestroyPoint"))
        {
            Destroy(other.gameObject);
            logSpawner.SpawnLog();
        }
    }
}