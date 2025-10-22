using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    [Header("Configurações do Tronco")]
    public GameObject logPrefab;
    public Transform spawnPoint;
    public Transform destroyPoint;
    public Vector3 baseMoveDirection = Vector3.right;
    public float moveSpeed = 6f;
    public float spawnInterval = 5f;
    public int maxLogs = 5; 

    private float spawnTimer = 0f;
    private int spawnedLogsCount = 0;
    private bool canSpawn = true;

    private void Update()
    {   
        if (canSpawn && spawnedLogsCount < maxLogs)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnLog();
                spawnTimer = 0f;
                spawnedLogsCount++;
                
                if (spawnedLogsCount >= maxLogs)
                {
                    canSpawn = false;
                    Debug.Log("[LogSpawner] Spawnou " + maxLogs + " logs. Parando spawner.");
                }
            }
        }
    }

    public void SpawnLog()
    {
        if (logPrefab != null && spawnPoint != null && destroyPoint != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-8, 8), 0, 0);
            
            Quaternion logRotation = Quaternion.Euler(-90, 0, 0);
            GameObject log = Instantiate(logPrefab, spawnPoint.position + randomOffset, logRotation);

            LogMovement logMovement = log.AddComponent<LogMovement>();
            logMovement.moveDirection = (baseMoveDirection).normalized;
            logMovement.moveSpeed = moveSpeed;
        }
    }
}