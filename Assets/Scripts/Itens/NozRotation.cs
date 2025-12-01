using UnityEngine;

public class NozRotation : MonoBehaviour
{
    public float velocidade = 90f; // graus por segundo

    [Header("Rotação")]
    public float velocidadeRotacao = 90f;

    [Header("Flutuação")]
    public float amplitude = 0.25f;      // o quanto sobe e desce
    public float velocidadeFlutuacao = 2f;

    private float posicaoInicialY;

    void Start()
    {
        posicaoInicialY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, velocidade * Time.deltaTime);


        // Flutuação
        float novoY = posicaoInicialY + Mathf.Sin(Time.time * velocidadeFlutuacao) * amplitude;
        transform.position = new Vector3(transform.position.x, novoY, transform.position.z);
    }
}
