using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 camForward;
    public Vector3 camRight;

    public void HandleCamera()
    {
        switch (this.name)
        {
            case "CameraThirdPerson":
                SetThirdPersonCamera();
                break;

            case "CameraTopDown":
                SetTopDownCamera();
                break;

            default:
                SetThirdPersonCamera();
                break;
        }
    }

    public void SetThirdPersonCamera()
    {
        camForward = transform.forward;
        camRight = transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();
    }

    public void SetTopDownCamera()
    {
        camForward = transform.up;
        camRight = transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();
    }
}