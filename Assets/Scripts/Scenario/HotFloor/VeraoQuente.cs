using UnityEngine;
using UnityEngine.UI;

public class VeraoQuente : MonoBehaviour
{
    [Header("Configurações da Temperatura")]
    public float temperaturaMaxima = 100f;
    public float taxaDeDano =15f;
    public float taxaDeRecuperacao = 8f;

    // --- MUDANÇAS AQUI ---
    // A referência ao slider agora é privada.
    // Ela será obtida do InterfaceManager.
    private Slider sliderTemperatura;
    // --------------------

    private float temperaturaAtual;
    private bool playerEstaNoChaoQuente = false;

    void Start()
    {
        // --- LÓGICA ATUALIZADA ---
        // Pega a referência do slider diretamente do Singleton InterfaceManager
        if (InterfaceManager.Instance != null)
        {
            sliderTemperatura = InterfaceManager.Instance.sliderTemperatura;
        }
        else
        {
            Debug.LogError("[VeraoQuente] Instância do InterfaceManager não encontrada na cena!");
            // Desativa o componente se o manager não existir para evitar erros no Update.
            this.enabled = false; 
            return;
        }
        // -------------------------

        // Inicializa a temperatura no valor máximo
        temperaturaAtual = temperaturaMaxima;

        // Configura o slider da UI (agora com verificação de segurança)
        if (sliderTemperatura != null)
        {
            sliderTemperatura.maxValue = temperaturaMaxima;
            sliderTemperatura.value = temperaturaAtual;
        }
        else
        {
            Debug.LogError("[VeraoQuente] O Slider de Temperatura não foi definido no InterfaceManager!");
            // Desativa o componente se o slider não estiver configurado.
            this.enabled = false;
        }
    }

    void Update()
    {
        if (GameManager.Instance.veraoQuente == this)
        {
            if (playerEstaNoChaoQuente)
            {
                temperaturaAtual -= taxaDeDano * Time.deltaTime;
            }
            else
            {
                temperaturaAtual += taxaDeRecuperacao * Time.deltaTime;
            }

            temperaturaAtual = Mathf.Clamp(temperaturaAtual, 0f, temperaturaMaxima);

            // A atualização do slider continua funcionando normalmente
            sliderTemperatura.value = temperaturaAtual;

            if (temperaturaAtual <= 0)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RespawnPlayer();
                }
            }
        }
    }

    // Os métodos OnTriggerEnter e OnTriggerExit permanecem exatamente iguais.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEstaNoChaoQuente = true;
            GameManager.Instance.veraoQuente = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEstaNoChaoQuente = false;
        }
    }

    public void ResetarTemperatura()
    {
        temperaturaAtual = temperaturaMaxima;
        Debug.Log("[VeraoQuente] Temperatura resetada para o máximo.");
    }
}