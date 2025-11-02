using UnityEngine;
using UnityEngine.UI;

public class CameraControllerPuzzle : MonoBehaviour
{
    #region Serialized Fields
    [Header("Camera Settings")]
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float smoothTime = 0.1f;
    
    [Header("Rotation Limits")]
    [SerializeField] private bool limitRotation = true;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 30f;
    [SerializeField] private float minHorizontalAngle = -45f;
    [SerializeField] private float maxHorizontalAngle = 45f;
    
    [Header("UI - Retícula")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite crosshairSprite;
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float crosshairSize = 32f;
    [SerializeField] private bool showCrosshair = true;
    
    [Header("Estado")]
    [SerializeField] private bool isActive = false;
    #endregion

    #region Private Fields
    private Vector2 currentRotation = Vector2.zero;
    private Vector2 targetRotation = Vector2.zero;
    private Vector2 rotationVelocity = Vector2.zero;
    
    private Vector3 initialCameraRotation;
    private bool cursorWasVisible;
    private CursorLockMode previousCursorLockMode;
    
    // Cached values for optimization
    private static readonly Vector3 CenterViewportPoint = new Vector3(0.5f, 0.5f, 0f);
    private Transform cameraTransform;
    private bool hasCameraTransform;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InitializeCamera();
        InitializeCrosshair();
    }

    private void Start()
    {
        if (puzzleCamera != null)
        {
            initialCameraRotation = cameraTransform.localEulerAngles;
            currentRotation = new Vector2(initialCameraRotation.y, initialCameraRotation.x);
            targetRotation = currentRotation;
        }

        if (!isActive)
        {
            SetActive(false);
        }
    }

    private void Update()
    {
        // Early exit if not active or no camera
        if (!isActive || !hasCameraTransform) return;

        HandleMouseInput();
        UpdateCameraRotation();
    }

    private void OnDisable()
    {
        if (isActive)
        {
            RestoreCursorState();
        }
    }
    #endregion

    #region Initialization
    private void InitializeCamera()
    {
        if (puzzleCamera == null)
        {
            puzzleCamera = GetComponent<Camera>();
            
            if (puzzleCamera == null)
            {
                Debug.LogError("[CameraControllerPuzzle] Nenhuma câmera encontrada!", this);
                hasCameraTransform = false;
                return;
            }
        }

        cameraTransform = puzzleCamera.transform;
        hasCameraTransform = cameraTransform != null;
    }

    private void InitializeCrosshair()
    {
        if (crosshairImage == null)
        {
            Debug.LogWarning("[CameraControllerPuzzle] Crosshair Image não atribuído!", this);
            return;
        }

        if (crosshairSprite != null)
        {
            crosshairImage.sprite = crosshairSprite;
        }

        crosshairImage.color = crosshairColor;

        RectTransform rect = crosshairImage.rectTransform;
        Vector2 center = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(crosshairSize, crosshairSize);
        rect.anchorMin = center;
        rect.anchorMax = center;
        rect.pivot = center;
        rect.anchoredPosition = Vector2.zero;

        if (!isActive)
        {
            crosshairImage.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Input Handling
    private void HandleMouseInput()
    {
        // Cache Input.GetAxis calls
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Early exit if no mouse movement
        if (mouseX == 0 && mouseY == 0 && !limitRotation) return;

        targetRotation.x += mouseX * mouseSensitivity;
        targetRotation.y -= mouseY * mouseSensitivity;

        if (limitRotation)
        {
            targetRotation.x = Mathf.Clamp(targetRotation.x, minHorizontalAngle, maxHorizontalAngle);
            targetRotation.y = Mathf.Clamp(targetRotation.y, minVerticalAngle, maxVerticalAngle);
        }
    }

    private void UpdateCameraRotation()
    {
        // Smooth damp both axes
        currentRotation.x = Mathf.SmoothDamp(currentRotation.x, targetRotation.x, ref rotationVelocity.x, smoothTime);
        currentRotation.y = Mathf.SmoothDamp(currentRotation.y, targetRotation.y, ref rotationVelocity.y, smoothTime);

        // Apply rotation using cached transform
        cameraTransform.localEulerAngles = new Vector3(currentRotation.y, currentRotation.x, 0f);
    }
    #endregion

    #region Public Methods - Activation
    public void SetActive(bool active)
    {
        isActive = active;

        if (active)
        {
            ActivateCamera();
        }
        else
        {
            DeactivateCamera();
        }
    }

    private void ActivateCamera()
    {
        cursorWasVisible = Cursor.visible;
        previousCursorLockMode = Cursor.lockState;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (showCrosshair && crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(true);
        }

        if (puzzleCamera != null)
        {
            puzzleCamera.enabled = true;
        }

        Debug.Log("[CameraControllerPuzzle] Câmera ativada!");
    }

    private void DeactivateCamera()
    {
        RestoreCursorState();

        if (crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(false);
        }

        if (puzzleCamera != null)
        {
            puzzleCamera.enabled = false;
        }

        Debug.Log("[CameraControllerPuzzle] Câmera desativada!");
    }

    private void RestoreCursorState()
    {
        Cursor.visible = cursorWasVisible;
        Cursor.lockState = previousCursorLockMode;
    }

    public bool IsActive()
    {
        return isActive;
    }
    #endregion

    #region Public Methods - Configuration
    public void ResetRotation()
    {
        if (hasCameraTransform)
        {
            currentRotation = new Vector2(initialCameraRotation.y, initialCameraRotation.x);
            targetRotation = currentRotation;
            cameraTransform.localEulerAngles = initialCameraRotation;
            rotationVelocity = Vector2.zero;
        }
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 10f);
    }

    public void SetHorizontalLimits(float min, float max)
    {
        minHorizontalAngle = min;
        maxHorizontalAngle = max;
    }

    public void SetVerticalLimits(float min, float max)
    {
        minVerticalAngle = min;
        maxVerticalAngle = max;
    }

    public void SetLimitRotation(bool enable)
    {
        limitRotation = enable;
    }

    public void ShowCrosshair(bool show)
    {
        showCrosshair = show;
        
        if (crosshairImage != null && isActive)
        {
            crosshairImage.gameObject.SetActive(show);
        }
    }

    public void SetCrosshairSprite(Sprite sprite)
    {
        crosshairSprite = sprite;
        
        if (crosshairImage != null && sprite != null)
        {
            crosshairImage.sprite = sprite;
        }
    }

    public void SetCrosshairColor(Color color)
    {
        crosshairColor = color;
        
        if (crosshairImage != null)
        {
            crosshairImage.color = color;
        }
    }

    public void SetCrosshairSize(float size)
    {
        crosshairSize = size;
        
        if (crosshairImage != null)
        {
            crosshairImage.rectTransform.sizeDelta = new Vector2(size, size);
        }
    }
    #endregion

    #region Public Methods - Raycast
    public bool RaycastFromCenter(out RaycastHit hit, float maxDistance = 100f)
    {
        if (!hasCameraTransform)
        {
            hit = default;
            return false;
        }

        // Use cached viewport point
        Ray ray = puzzleCamera.ViewportPointToRay(CenterViewportPoint);
        return Physics.Raycast(ray, out hit, maxDistance);
    }

    public bool RaycastFromCenter(out RaycastHit hit, float maxDistance, LayerMask layerMask)
    {
        if (!hasCameraTransform)
        {
            hit = default;
            return false;
        }

        Ray ray = puzzleCamera.ViewportPointToRay(CenterViewportPoint);
        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }

    public Ray GetCenterRay()
    {
        if (!hasCameraTransform)
        {
            return default;
        }

        return puzzleCamera.ViewportPointToRay(CenterViewportPoint);
    }
    #endregion

    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!isActive || !hasCameraTransform) return;

        Ray ray = puzzleCamera.ViewportPointToRay(CenterViewportPoint);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 10f);

        if (limitRotation && cameraTransform.parent != null)
        {
            Vector3 origin = cameraTransform.position;
            Vector3 parentForward = cameraTransform.parent.forward;
            
            Vector3 leftDir = Quaternion.Euler(0, minHorizontalAngle, 0) * parentForward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, leftDir * 5f);
            
            Vector3 rightDir = Quaternion.Euler(0, maxHorizontalAngle, 0) * parentForward;
            Gizmos.DrawRay(origin, rightDir * 5f);
        }
    }
#endif
    #endregion
}
