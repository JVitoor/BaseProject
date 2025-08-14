using UnityEngine;

public partial class Player : MonoBehaviour
{
    #region Unity Methods

    public Transform cameraTransform;
    private Vector3 camForward;
    private Vector3 camRight;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        camForward = cameraTransform.forward;
        camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();
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
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.z = Input.GetAxis("Vertical");

        camForward = cameraTransform.forward;
        camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        if (moveInput.magnitude > 0)
        {
            Vector3 desiredMove = camForward * moveInput.z + camRight * moveInput.x;

            rot = Quaternion.LookRotation(desiredMove);

            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
        }
    }

    private void HandleMove()
    {
        Vector3 desiredMove = camForward * moveInput.z + camRight * moveInput.x;

        move = desiredMove.normalized * maxSpeed;

        controller.SimpleMove(move);
    }

    #endregion Player Movement
}