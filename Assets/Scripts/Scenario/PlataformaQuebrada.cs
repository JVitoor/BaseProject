using UnityEngine;
using UnityEngine.VFX;

public class PlataformaQuebrada : MonoBehaviour
{
    private int quantVolumes = 0;

    private bool disparado = false;

    private void Awake()
    {
        quantVolumes = transform.childCount;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Disparando plataforma quebrada via tecla Y");
            Disparar();
        }
    }

    public void Disparar()
    {
        if (!disparado)
        {
            disparado = true;
            transform.GetComponent<BoxCollider>().enabled = false;

            for (int i = 1; i < quantVolumes; i++)
            {
                transform.GetChild(i).GetComponent<MeshCollider>().convex = true;
                transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
                transform.GetChild(i).GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }
}
