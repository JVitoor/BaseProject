using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    #region Properties

    // Lista de c�meras dispon�veis para alternar
    public List<GameObject> CameraList;

    // Refer�ncia para a c�mera de terceira pessoa
    public GameObject CameraThirdPerson;

    // Refer�ncia para a c�mera de vis�o superior
    public GameObject CameraTopView;

    // Gerenciador de estado da c�mera (0 = terceira pessoa, 1 = vis�o superior)
    public int Manager;

    #endregion Properties

    #region Methods

    // Inicializa a lista de c�meras ao iniciar o script
    private void Start()
    {
        CameraList.Add(CameraThirdPerson);
        CameraList.Add(CameraTopView);
    }

    // Alterna entre as c�meras dispon�veis
    public void ManagerCamera(int valor)
    {
        if (valor == 0)
        {
            SwitchToCameraTopView(); // Ativa a vis�o superior
            valor = 1;
        }
        else
        {
            SwitchToThirdPersonCamera(); // Ativa a terceira pessoa
            valor = 0;
        }
    }

    // Ativa a c�mera de terceira pessoa e desativa as demais
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

    // Ativa a c�mera de vis�o superior e desativa as demais
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