using UnityEngine;

public class PetalController : MonoBehaviour
{
    [Header("Configuração")]
    public int petalIndex;
    public FlowerController flowerController;
    public Renderer petalRenderer;

    [Header("Cores")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color activeColor = Color.black; // Cor quando player está em cima

    private bool playerOnPetal = false;
    private Color currentColor;

    private void Start()
    {
        currentColor = normalColor;
        SetColor(normalColor);
    }

    public void SetColor(Color color)
    {
        if (petalRenderer != null)
        {
            currentColor = color;
            petalRenderer.material.color = color;
        }
    }

    public void ResetColor()
    {
        SetColor(playerOnPetal ? activeColor : normalColor);
    }

    public void Highlight()
    {
        SetColor(highlightColor);
    }

    public void OnPlayerEnter()
    {
        if (!playerOnPetal)
        {
            playerOnPetal = true;
            SetColor(activeColor);

            // Notifica o FlowerController que o player pisou nesta pétala
            if (flowerController != null)
            {
                flowerController.OnPetalStepped(petalIndex);
            }
        }
    }

    private void Update()
    {
        // Verifica se o player ainda está sobre a pétala
        if (playerOnPetal)
        {
            // Procura pelo player na cena
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);

                // Se o player saiu da pétala (distância maior que um threshold)
                if (distance > 2f) // Ajuste este valor conforme necessário
                {
                    playerOnPetal = false;
                    ResetColor();
                }
            }
        }
    }
}