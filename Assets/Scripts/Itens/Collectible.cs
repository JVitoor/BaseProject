using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                CollectNut();
            }
        }
    }
    
    private void CollectNut()
    {
        // Adiciona a noz ao contador no GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddNut();
        }
        else
        {
            Debug.LogWarning("[Collectible] GameManager não encontrado!");
        }
        
        // Remove a noz da cena
        Destroy(gameObject);
    }
}