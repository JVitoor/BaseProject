using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    #region Properties

    // Lista de câmeras disponíveis para alternar
    public List<GameObject> CameraList;

    // Referência para a câmera de terceira pessoa
    public GameObject CameraThirdPerson;

    // Referência para a câmera de visão superior
    public GameObject CameraTopView;

    // Gerenciador de estado da câmera (0 = terceira pessoa, 1 = visão superior)
    public int Manager;

    #endregion Properties

    #region Methods

    // Inicializa a lista de câmeras ao iniciar o script
    private void Start()
    {
        CameraList.Add(CameraThirdPerson);
        CameraList.Add(CameraTopView);
    }

    // Alterna entre as câmeras disponíveis
    public void ManagerCamera(int valor)
    {
        if (valor == 0)
        {
            SwitchToCameraTopView(); // Ativa a visão superior
            valor = 1;
        }
        else
        {
            SwitchToThirdPersonCamera(); // Ativa a terceira pessoa
            valor = 0;
        }
    }

    // Ativa a câmera de terceira pessoa e desativa as demais
    private void SwitchToThirdPersonCamera()
    {
        CameraThirdPerson.SetActive(true);
        foreach (var camera in CameraList)
        {
            if (camera != CameraThirdPerson)
            {
                camera.SetActive(false);
            }
        }
    }

    // Ativa a câmera de visão superior e desativa as demais
    private void SwitchToCameraTopView()
    {
        CameraTopView.SetActive(true);
        foreach (var camera in CameraList)
        {
            if (camera != CameraTopView)
            {
                camera.SetActive(false);
            }
        }
    }

    #endregion Methods
}