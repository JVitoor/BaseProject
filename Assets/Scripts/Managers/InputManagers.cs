using UnityEngine;
using UnityEngine.UI;

public class InputManagers : BaseManager
{
    [Header("Configuração do Mouse")]
    [Tooltip("Tecla para alternar o lock do mouse")]
    public KeyCode toggleCursorKey;
    public KeyCode togglePauseKey;

    [Tooltip("Bloquear o mouse ao iniciar o jogo?")]
    public bool lockOnStart = true;

    public bool IsLocked { get; private set; }

    public GameObject pausePanel; 

    void Start()
    {
        if (lockOnStart)
        {
            LockCursor();
            Debug.Log("Cursor locked on start.");
        }
        else
            UnlockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(togglePauseKey))
            TogglePausePanel();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeScene(1);
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeScene(2);
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeScene(3);
       
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeScene(4);
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

    public void TogglePausePanel()
    {
        Time.timeScale = pausePanel.activeSelf ? 1f : 0f;
        pausePanel.SetActive(!pausePanel.activeSelf);
        ToggleCursor();
    }

    public void ChangeScene(int level)
    {
        GameManager.Instance.LoadLevel(level);
    }

    #endregion

    public void Unpause()
    {
        Time.timeScale = 1;
        ToggleCursor();
    }
}
