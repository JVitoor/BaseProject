using UnityEngine;

public class LogDestroy : MonoBehaviour
{
    public LogSpawner logSpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Log") )
        {
            Destroy(other.gameObject);
            logSpawner.SpawnLog();
        }

        if (other.CompareTag("VitoriaRegia"))
        {
            Destroy(other.gameObject);
            logSpawner.SpawnVitoriaRegia();
        }
    }
}
