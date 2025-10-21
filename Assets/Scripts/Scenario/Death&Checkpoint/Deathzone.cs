using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    private void Awake()
    {
        // Garante que o collider seja um trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se foi o player que entrou
        if (other.CompareTag("Player"))
        {
            Debug.Log("[DeathZone] Player entrou na zona de morte. Respawnando...");

            // Manda o GameManager respawnar o player
            GameManager.Instance.RespawnPlayer();
        }
    }
}