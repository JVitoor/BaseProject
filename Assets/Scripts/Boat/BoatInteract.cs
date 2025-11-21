using UnityEngine;

public class BoatInteraction : MonoBehaviour
{
    private Player player;
    private BoatController currentBoat;
    private bool isInBoat = false;

    private void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player não encontrado!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInBoat)
            {
                ExitBoat();
            }
            else if (currentBoat != null)
            {
                EnterBoat(currentBoat);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            currentBoat = other.GetComponent<BoatControlle>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            if (!isInBoat)
            {
                currentBoat = null;
            }
        }
    }

    private void EnterBoat(BoatControlle boat)
    {
        isInBoat = true;
        currentBoat = boat;
        boat.isOccupied = true;

        // Desativa controle do player
        player.enabled = false;
        player.controller.enabled = false;

        // Move o player para dentro do barco
        transform.position = boat.transform.position + Vector3.up; // ajuste conforme necessário
        transform.SetParent(boat.transform);
    }

    private void ExitBoat()
    {
        isInBoat = false;
        currentBoat.isOccupied = false;

        // Reativa controle do player
        player.enabled = true;
        player.controller.enabled = true;

        // Remove o player do barco
        transform.SetParent(null);
        transform.position = currentBoat.transform.position + currentBoat.transform.right * 2f; // sai ao lado
        currentBoat = null;
    }
}
