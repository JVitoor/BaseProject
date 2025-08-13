using UnityEngine;

public class ManagersController : MonoBehaviour
{
    #region Properties

    [Header(" ?? Managers Array")]
    private BaseManager[] managers;

    #endregion Properties

    #region Unity Methods

    private void Awake()
    {
        managers = Object.FindObjectsByType<BaseManager>(FindObjectsSortMode.None);

        foreach (var manager in managers)
        {
            manager.Initialize();
        }
    }

    #endregion Unity Methods

    #region Game State Methods

    public void StartGame()
    {
        foreach (var manager in managers)
        {
            manager.OnGameStart();
        }
    }

    public void PauseGame()
    {
        foreach (var manager in managers)
        {
            manager.OnGamePause();
        }
    }

    #endregion Game State Methods
}