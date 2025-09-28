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
    
    [Header("Dash Input")]
    [Tooltip("Referência para o componente Dash")]
    public Dash dashReference;

    void Start()
    {
        if (lockOnStart)
            LockCursor();
        else
            UnlockCursor();

        // Tenta encontrar o componente Dash automaticamente se não estiver atribuído
        if (dashReference == null)
        {
            dashReference = FindFirstObjectByType<Dash>();
            if (dashReference == null)
            {
                Debug.LogWarning("[InputManager] Componente Dash não encontrado na cena!");
            }
            else
            {
                Debug.Log("[InputManager] Componente Dash encontrado automaticamente!");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleCursor();
    }

    #region Dash Input Methods

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (dashReference != null && context.performed)
        {
            dashReference.StartDash();
        }
    }

    #endregion

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
