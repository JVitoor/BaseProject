using UnityEngine;

public class JumpBoostController : MonoBehaviour
{
    public float jumpBoost = 30f; // Altura extra do trampolim

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que encostou é o player
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.verticalVelocity = jumpBoost;
            player.jumpCount++; // Conta como um pulo
        }
    }
}