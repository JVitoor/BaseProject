using System.Collections;
using UnityEngine;

public class FallingLeaf : MonoBehaviour
{
    [Header("Configura  es de Queda")]
    [SerializeField] private float delayAntesDeCair = 0.5f; // Meio segundo de delay
    private float gravidade = 20f;
    [SerializeField] private float velocidadeRotacao = 100f; // Rota  o enquanto cai

    private bool estaCaindo = false;
    private bool colidiu = false;
    private Rigidbody rb;

    void Start()
    {
        // Garante que a folha tenha um Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configura o Rigidbody para n o cair automaticamente
        rb.isKinematic = true;
        rb.useGravity = false;

        // Garante que existe um collider (N O trigger) para o player poder pisar
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning("[FallingLeaf] Nenhum collider encontrado na folha. Adicionando BoxCollider.");
            gameObject.AddComponent<BoxCollider>();
        }
    }

    void Update()
    {
        // Se est  caindo, aplica rota  o para efeito visual
        if (estaCaindo)
        {
            transform.Rotate(Vector3.forward * velocidadeRotacao * Time.deltaTime);
        }
    }

    // M todo p blico que pode ser chamado pelo Player quando detectar colis o
    public void AtivarQueda()
    {
        if (!colidiu)
        {
            Debug.Log("[FallingLeaf] Folha vai cair em " + delayAntesDeCair + " segundos.");
            colidiu = true;
            StartCoroutine(IniciarQueda());
        }
    }

    private IEnumerator IniciarQueda()
    {
        // Aguarda o tempo configurado (meio segundo por padr o)
        yield return new WaitForSeconds(delayAntesDeCair);

        // Inicia a queda
        estaCaindo = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        Debug.Log("[FallingLeaf] Folha come ou a cair!");
    }
}