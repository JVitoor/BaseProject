using UnityEngine;

public class JumpBoostController : MonoBehaviour
{
    public float jumpBoost = 30f; // Altura extra do trampolim

    public AudioClip clip;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que encostou � o player
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            AudioManager audioManager = AudioManager.GetInstance();

            if (audioManager != null)
            {
                audioManager.PlaySFX(clip);
            }

            player.verticalVelocity = jumpBoost;
            player.jumpCount++; // Conta como um pulo
        }
    }
}