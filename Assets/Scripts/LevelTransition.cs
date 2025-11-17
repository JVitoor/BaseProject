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

        // Para o timer do Inverno quando o nível é completado
        if (SeasonManager.Instance != null)
        {
            Season currentSeason = SeasonManager.Instance.GetCurrentSeason();
            if (currentSeason == Season.Inverno)
            {
                SeasonManager.Instance.StopWinterTimer();
                Debug.Log("[LevelTransition] Timer do Inverno parado - Nível completado!");
            }
        }

        SceneManager.LoadScene(nextLevel);

    }
}
