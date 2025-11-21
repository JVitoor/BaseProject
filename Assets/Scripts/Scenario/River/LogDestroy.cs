using UnityEngine;

public class LogDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Log") )
        {
            Destroy(other.gameObject);
        }

        if (other.CompareTag("VitoriaRegia"))
        {
            Destroy(other.gameObject);
        }
    }
}
