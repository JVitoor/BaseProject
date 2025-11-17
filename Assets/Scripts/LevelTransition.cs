using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{

    [Header("Configurações do Trigger")]

    public int nextLevel = 1;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            LoadNextLevel(nextLevel);
        }
    }

    public void LoadNextLevel(int nextLevel)
    {
        if (isTransitioning) return;

        isTransitioning = true;

        SceneManager.LoadScene(nextLevel);

    }
}
