using UnityEngine;

public class Pushable : MonoBehaviour
{
    private Rigidbody rb; // O rigidbody do objeto

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtém o componente Rigidbody
    }

    public void Push(Vector3 direction, float force) // Método para aplicar força
    {
        if (rb != null) // Verifica se o Rigidbody existe
        {
            rb.AddForce(direction * force, ForceMode.Impulse); // Aplica a força na direção especificada
        }
    }
}
