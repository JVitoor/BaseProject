using UnityEngine;

public class InputManagers : BaseManager
{
    [Header("Configuração do Mouse")]
    [Tooltip("Tecla para alternar o lock do mouse")]
    public KeyCode toggleKey;

    [Tooltip("Bloquear o mouse ao iniciar o jogo?")]
    public bool lockOnStart = true;

    public bool IsLocked { get; private set; }

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
