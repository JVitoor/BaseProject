using UnityEngine;

public class PlataformaMover : MonoBehaviour
{
    public float amplitude = 0.5f; // quão longe ela vai pra cada lado
    public float velocidade = 2f;  // quão rápido ela se move

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.position;
    }

    void Update()
    {
        // Movimento suave usando seno
        float deslocamento = Mathf.Sin(Time.time * velocidade) * amplitude;
        transform.position = new Vector3(posicaoInicial.x + deslocamento, posicaoInicial.y, posicaoInicial.z);
    }

    // Detecta quando o player encosta
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    // Detecta quando o player sai da plataforma
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}

