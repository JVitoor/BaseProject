using UnityEngine;

public class EndGame : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("Painel que será mostrado quando o player colidir")]
    public GameObject endGamePanel;

    [Header("Player Tag")]
    [Tooltip("Tag do player para detectar colisão")]
    public string playerTag = "Player";

    private void Start()
    {
        // Garante que o painel começa desativado
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[EndGame] Painel não atribuído no Inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que colidiu é o player
        if (other.CompareTag(playerTag))
        {
            ShowEndGamePanel();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto que colidiu é o player
        if (collision.gameObject.CompareTag(playerTag))
        {
            ShowEndGamePanel();
        }
    }

    private void ShowEndGamePanel()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            Debug.Log("[EndGame] Painel de fim de jogo ativado!");

            // Opcional: Pausa o jogo
            // Time.timeScale = 0f;

            // Opcional: Mostra o cursor
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        }
        else
        {
            Debug.LogWarning("[EndGame] Painel não está atribuído!");
        }
    }
}
