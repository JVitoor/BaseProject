using UnityEngine;

public partial class Player : MonoBehaviour
{
    #region Unity Methods

    private void Start()
    {
        controller = GetComponent<CharacterController>();
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

        if (moveInput.magnitude > 0)
        {
            rot = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
        }
    }

    private void HandleMove()
    {
        move = moveInput.normalized * maxSpeed;
        controller.SimpleMove(move);
    }

    #endregion Player Movement
}