using UnityEngine;

public class PlataformaMover : MonoBehaviour
{
    public float amplitude = 0.5f; // quão longe ela vai pra cada lado
    public float velocidade = 2f;  // quão rápido ela se move

    private Vector3 posicaoInicial;
    private Vector3 previousPosition; // Posição do frame anterior

    void Start()
    {
        posicaoInicial = transform.position;
        previousPosition = transform.position;
        
        // Garante que a plataforma tem a tag correta
        if (!gameObject.CompareTag("MovingPlatform"))
        {
            Debug.LogWarning($"[PlataformaMover] O objeto {gameObject.name} não tem a tag 'MovingPlatform'! Adicione a tag para funcionar corretamente.");
        }

        // Verifica se tem um collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"[PlataformaMover] {gameObject.name} precisa de um Collider!");
        }
        else if (col.isTrigger)
        {
            Debug.LogWarning($"[PlataformaMover] O Collider de {gameObject.name} está marcado como Trigger. Desmarque 'Is Trigger' para funcionar corretamente!");
        }
    }

    void Update()
    {
        // Salva a posição anterior
        previousPosition = transform.position;

        // Movimento suave usando seno
        float deslocamento = Mathf.Sin(Time.time * velocidade) * amplitude;
        transform.position = new Vector3(posicaoInicial.x + deslocamento, posicaoInicial.y, posicaoInicial.z);
    }

    // Retorna o movimento da plataforma neste frame
    public Vector3 GetMovementThisFrame()
    {
        return transform.position - previousPosition;
    }
}

