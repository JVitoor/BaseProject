using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    #region Properties
    public GameObject CameraThirdPerson;
    public GameObject CameraTopView;
    public int Manager;

    #endregion Properties

    #region Methods
    public void ManagerCamera()
    {
        if (Manager == 0)
        {
            SwitchToCameraTopView();
            Manager = 1;
        }
        else 
        {
            SwitchToThirdPersonCamera();
            Manager = 0;
        }
    }


    void SwitchToThirdPersonCamera()
    {
        CameraThirdPerson.SetActive(true); 
        CameraTopView.SetActive(false);
    }

    void SwitchToCameraTopView ()
    {
        CameraThirdPerson.SetActive(false);
        CameraTopView.SetActive(true);
    }

    #endregion
}
