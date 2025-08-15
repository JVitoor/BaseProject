using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
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
        // Permite pular se pressionou o botão e não excedeu o número máximo de pulos
        if (context.performed && jumpCount < maxJumps)
        {
            verticalVelocity = jumpForce;
            jumpCount++;
        }
        // Ativa o glide se estiver no ar, já usou o double jump e a tecla de pulo está pressionada
        else if (context.performed && !controller.isGrounded && jumpCount >= maxJumps)
        {
            isGliding = true;
        }
        // Desativa o glide ao soltar a tecla de pulo
        else if (context.canceled)
        {
            isGliding = false;
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

        // Aplica movimento vertical (pulo, gravidade e glide)
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
        // Aplica gravidade e reseta o contador de pulos ao tocar o chão
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = 0f;
            jumpCount = 0;
            isGliding = false;
        }
        else
        {
            // Se estiver planando, aplica gravidade reduzida
            if (isGliding)
            {
                verticalVelocity += glideGravity * Time.deltaTime;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }
    }

    #endregion Player Movement
}