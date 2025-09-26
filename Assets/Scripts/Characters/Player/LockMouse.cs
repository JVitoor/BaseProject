using UnityEngine;

public class LockMouse : MonoBehaviour
{
    #region Properties

    public KeyCode toggleKey;

    public bool lockStartOn = true;
    public bool cursorLock { get; private set; }
    #endregion

    #region UnityMethods
    void Start()
    {
        if (lockStartOn) LockCursor();
        else UnlockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleCursor();
    }

    #endregion

    #region MouseMethods
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLock = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        cursorLock = false;
    }

    public void ToggleCursor()
    {
        if (cursorLock) UnlockCursor();
        else LockCursor();
    }
    #endregion
}
