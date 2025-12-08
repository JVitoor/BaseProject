using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Nome da cena que será carregada quando o player colidir")]
    public string sceneToLoad = "EndGameScene";

    [Header("Player Tag")]
    [Tooltip("Tag do player para detectar colisão")]
    public string playerTag = "Player";

    [Header("Optional Settings")]
    [Tooltip("Delay em segundos antes de carregar a cena (0 = instantâneo)")]
    public float delayBeforeLoad = 1f;

    private bool sceneLoadTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que colidiu é o player
        if (other.CompareTag(playerTag) && !sceneLoadTriggered)
        {
            LoadEndGameScene();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto que colidiu é o player
        if (collision.gameObject.CompareTag(playerTag) && !sceneLoadTriggered)
        {
            LoadEndGameScene();
        }
    }

    private void LoadEndGameScene()
    {
        if (sceneLoadTriggered) return;

        sceneLoadTriggered = true;
        Debug.Log($"[EndGame] Carregando cena: {sceneToLoad}");

        if (delayBeforeLoad > 0f)
        {
            Invoke(nameof(LoadScene), delayBeforeLoad);
        }
        else
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        // Verifica se a cena existe na build
        if (Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError($"[EndGame] Cena '{sceneToLoad}' não encontrada! Verifique se está adicionada no Build Settings.");
        }
    }
}