using UnityEngine;

public class ResetPOS : MonoBehaviour
{

    [Header("Posição para onde o player vai voltar")]
    public Transform[] respawnPoints;//lista de Spawns

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
        if (hit.gameObject.CompareTag("Bloco"))
        {
            //Pegando a Referencia do Bloco
            RespawnPoint rp = hit.gameObject.GetComponent<RespawnPoint>();
            if (rp != null && rp.pointIndex < respawnPoints.Length)
            {
                RespawnAt(rp.pointIndex);
            }
        }
    }

    void RespawnAt(int index)
    {
        if (respawnPoints == null || respawnPoints.Length == 0) return;

        controller.enabled = false;
        transform.position = respawnPoints[index].position;
        controller.enabled = true;
    }
}



