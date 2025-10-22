using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatControlle : MonoBehaviour
{
    [Header("Flutua��o")]
    public Transform[] floatPoints;
    public float floatForce = 10f;
    public float damping = 0.5f;

    [Header("Movimento")]
    public float moveForce = 20f;
    public float turnTorque = 5f;

    [Header("Controle")]
    public bool isOccupied = false; // true quando jogador estiver dentro

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isOccupied) return;

        // Flutua��o simples
        foreach (Transform point in floatPoints)
        {
            float waterLevel = 0f; // altura da �gua
            float distanceToWater = point.position.y - waterLevel;
            if (distanceToWater < 0)
            {
                rb.AddForceAtPosition(Vector3.up * -distanceToWater * floatForce, point.position);
            }
        }

        // Movimento baseado no Input do jogador
        float move = Input.GetAxis("Vertical");   // W/S ou setas
        float turn = Input.GetAxis("Horizontal"); // A/D ou setas

        rb.AddForce(transform.forward * move * moveForce);
        rb.AddTorque(Vector3.up * turn * turnTorque);

        // Damping para n�o balan�ar demais
        rb.linearVelocity *= (1 - damping * Time.fixedDeltaTime);
        rb.angularVelocity *= (1 - damping * Time.fixedDeltaTime);
    }
}
