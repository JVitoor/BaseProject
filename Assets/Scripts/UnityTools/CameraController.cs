using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 camForward;
    public Vector3 camRight;

    public void HandleCamera()
    {
        camForward = transform.forward;
        camRight = transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();
    }
}