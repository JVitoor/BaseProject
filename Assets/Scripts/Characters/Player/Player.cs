using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    #region Properties

    private float verticalVelocity;
    public float jumpForce = 7f;
    public float gravity = -20f;

    #endregion Properties

    #region Unity Methods

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraController = mainCamera.GetComponent<CameraController>();
    }

    private void FixedUpdate()
    {
        HandlePlayerMovement();
        HandlePlayerJump();
    }

    #endregion Unity Methods

    #region Input Methods

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // Lê o valor do input do sistema de entrada
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // Se pressionou o botão de pulo e está no chão, aplica o pulo
        if (context.performed && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    #endregion Input Methods

    #region Player Movement

    private void HandlePlayerMovement()
    {
        // Calcula o vetor de movimento relativo à câmera
        Vector3 desiredMove = (cameraController.camForward * moveInput.y) + (cameraController.camRight * moveInput.x);

        // Normaliza o vetor de movimento e multiplica pela velocidade máxima
        move = desiredMove.normalized * moveSpeed;

        // Aplica movimento vertical (pulo e gravidade)
        move.y = verticalVelocity;

        // Move o player usando o CharacterController
        controller.Move(move * Time.deltaTime);

        // Rotaciona o player para a direção do movimento, se houver input
        if (moveInput.magnitude > 0)
        {
            rotation = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }

        cameraController.HandleCamera();
    }

    private void HandlePlayerJump()
    {
        // Aplica gravidade
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    #endregion Player Movement
}