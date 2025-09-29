using UnityEngine;

public class Pullable : MonoBehaviour
{
    private Rigidbody rb; // O rigidbody do objeto

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtém o componente Rigidbody
    }

    public void SetKinematic(bool value) // Método para definir se o objeto é kinematic
    {
        if (rb != null) // Verifica se o Rigidbody existe
        {
            rb.isKinematic = value; // Define o valor de isKinematic
        }
    }
}