using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagers : BaseManager
{
    [Header("Configuração do Mouse")]
    [Tooltip("Tecla para alternar o lock do mouse")]
    public KeyCode toggleKey;

    [Tooltip("Bloquear o mouse ao iniciar o jogo?")]
    public bool lockOnStart = true;

    public bool IsLocked { get; private set; }

    public GameObject pausePanel;
    private bool isPaused = false;

    void Start()
    {
        if (lockOnStart)
            LockCursor();
        else
            UnlockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleCursor();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // congela o jogo
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // volta o tempo ao normal
        isPaused = false;
    }

    #region Methods Cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsLocked = false;
    }

    public void ToggleCursor()
    {
        if (IsLocked) UnlockCursor();
        else LockCursor();
    }
    #endregion
}
