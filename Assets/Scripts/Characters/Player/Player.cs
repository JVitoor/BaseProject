using UnityEngine;

public partial class Player : MonoBehaviour
{
    #region Unity Methods

    private CameraController cameraController;
    public GameObject mainCamera;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraController = mainCamera.GetComponent<CameraController>();
    }

    private void Update()
    {
        HandleInputs();
        HandleMove();
    }

    #endregion Unity Methods

    #region Player Movement

    private void HandleInputs()
    {
        // Captura o input do jogador para os eixos horizontal e vertical
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.z = Input.GetAxis("Vertical");

        // Se houver movimento, calcula a dire��o desejada baseada na orienta��o da c�mera
        if (moveInput.magnitude > 0)
        {
            // Calcula o vetor de movimento relativo � c�mera
            Vector3 desiredMove = (cameraController.camForward * moveInput.z) + (cameraController.camRight * moveInput.x);

            // Calcula a rota��o desejada para o player olhar na dire��o do movimento
            rot = Quaternion.LookRotation(desiredMove);

            // Suaviza a rota��o do player para a dire��o desejada
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
        }

        // Atualiza os vetores de dire��o da c�mera
        cameraController.HandleCamera();
    }

    private void HandleMove()
    {
        // Calcula o vetor de movimento relativo � c�mera
        Vector3 desiredMove = (cameraController.camForward * moveInput.z) + (cameraController.camRight * moveInput.x);

        // Normaliza o vetor de movimento e multiplica pela velocidade m�xima
        move = desiredMove.normalized * moveSpeed;

        // Move o player usando o CharacterController
        controller.SimpleMove(move);
    }

    #endregion Player Movement
}