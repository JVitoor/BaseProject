using System.Collections;
using UnityEngine;

public class FallingLeaf : MonoBehaviour
{
    [Header("Configurações de Queda")]
    [SerializeField] private float delayAntesDeCair = 0.5f; // Meio segundo de delay
    [SerializeField] private float gravidade = 20f;
    [SerializeField] private float velocidadeRotacao = 100f; // Rotação enquanto cai
    
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
        
        // Configura o Rigidbody para não cair automaticamente
        rb.isKinematic = true;
        rb.useGravity = false;
    
        // Garante que existe um collider (NÃO trigger) para o player poder pisar
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning("[FallingLeaf] Nenhum collider encontrado na folha. Adicionando BoxCollider.");
            gameObject.AddComponent<BoxCollider>();
        }
    }

    void Update()
    {
        // Se está caindo, aplica rotação para efeito visual
        if (estaCaindo)
        {
            transform.Rotate(Vector3.forward * velocidadeRotacao * Time.deltaTime);
        }
    }

    // Método público que pode ser chamado pelo Player quando detectar colisão
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
        // Aguarda o tempo configurado (meio segundo por padrão)
        yield return new WaitForSeconds(delayAntesDeCair);
     
        // Inicia a queda
        estaCaindo = true;
        rb.isKinematic = false;
        rb.useGravity = true;
    
        Debug.Log("[FallingLeaf] Folha começou a cair!");
    }
}
