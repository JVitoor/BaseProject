using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    [Header("Configurações do Tronco")]
    public GameObject logPrefab;
    public Transform spawnPoint;

    public Vector3 baseMoveDirection = Vector3.right;
    public float moveSpeed = 6f;
    public float spawnInterval = 5f;
    public float initialDelay = 0f;
    public int maxLogs = 5; 

    private float spawnTimer = 4f;
    private int spawnedLogsCount = 0;
    private bool canSpawn = true;
    private bool hasStarted = false;

    private void Update()
    {   
        if (canSpawn && spawnedLogsCount < maxLogs)
        {
            spawnTimer += Time.deltaTime;
    
            if (!hasStarted)
            {
                if (spawnTimer >= initialDelay)
                {
                    hasStarted = true;
                    spawnTimer = 0f;
                }
                return;
            }
            
            if (spawnTimer >= spawnInterval)
            {
                if (logPrefab.tag == "Log")
                {
                    SpawnLog();
                }
                else if (logPrefab.tag == "VitoriaRegia")
                {
                    SpawnVitoriaRegia();
                }
                 
                spawnTimer = 0f;
                spawnedLogsCount++;
                
                if (spawnedLogsCount >= maxLogs)
                {
                    canSpawn = false;
                }
            }
        }
    }

    public void SpawnLog()
    {
        if (logPrefab != null && spawnPoint != null)
        {
            Vector3 randomOffset = new Vector3(0, 0, 0);
            
            Quaternion logRotation = Quaternion.Euler(-90, 0, 0);
            GameObject log = Instantiate(logPrefab, spawnPoint.position + randomOffset, logRotation);

            LogMovement logMovement = log.AddComponent<LogMovement>();
            logMovement.moveDirection = (baseMoveDirection).normalized;
            logMovement.moveSpeed = moveSpeed;
        }
    }

    public void SpawnVitoriaRegia()
    {
        if (logPrefab != null && spawnPoint != null)
        {
            Vector3 randomOffset = new Vector3(0, 0, 0);

            Quaternion logRotation = Quaternion.Euler(0, 0, 0);
            GameObject log = Instantiate(logPrefab, spawnPoint.position + randomOffset, logRotation);

            LogMovement logMovement = log.AddComponent<LogMovement>();
            logMovement.moveDirection = (baseMoveDirection).normalized;
            logMovement.moveSpeed = moveSpeed;
        }
    }
}