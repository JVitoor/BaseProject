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

        // Se houver movimento, calcula a direção desejada baseada na orientação da câmera
        if (moveInput.magnitude > 0)
        {
            // Calcula o vetor de movimento relativo à câmera
            Vector3 desiredMove = (cameraController.camForward * moveInput.z) + (cameraController.camRight * moveInput.x);

            // Calcula a rotação desejada para o player olhar na direção do movimento
            rot = Quaternion.LookRotation(desiredMove);

            // Suaviza a rotação do player para a direção desejada
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
        }

        // Atualiza os vetores de direção da câmera
        cameraController.HandleCamera();
    }

    private void HandleMove()
    {
        // Calcula o vetor de movimento relativo à câmera
        Vector3 desiredMove = (cameraController.camForward * moveInput.z) + (cameraController.camRight * moveInput.x);

        // Normaliza o vetor de movimento e multiplica pela velocidade máxima
        move = desiredMove.normalized * moveSpeed;

        // Move o player usando o CharacterController
        controller.SimpleMove(move);
    }

    #endregion Player Movement
}