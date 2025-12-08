using UnityEngine;
using UnityEngine.UI;

public class VeraoQuente : MonoBehaviour
{
    [Header("Configurações da Temperatura")]
    public float temperaturaMaxima = 100f;
    public float taxaDeDano = 15f;
    public float taxaDeRecuperacao = 8f;

    [Header("Efeitos Sonoros")]
    [Tooltip("Som que toca quando o player entra no chão quente")]
    public AudioClip hotFloorSizzleSound;

    private Slider sliderTemperatura;

    // Referência ao Player para chamar SpawnSmokeEffect/RemoveSmokeEffect
    private Player playerReference;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEstaNoChaoQuente = true;
            GameManager.Instance.veraoQuente = this;

            // Obtém referência ao componente Player
            playerReference = other.GetComponent<Player>();

            if (playerReference != null)
            {
                // Chama o método do Player para spawnar o efeito de fumaça
                playerReference.SpawnSmokeEffect();
            }
            else
            {
                Debug.LogWarning("[VeraoQuente] Componente Player não encontrado no GameObject do player!");
            }

            // Toca o som de chão quente
            PlayHotFloorSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEstaNoChaoQuente = false;

            // Chama o método do Player para remover o efeito de fumaça
            if (playerReference != null)
            {
                playerReference.RemoveSmokeEffect();
                playerReference = null;
            }
        }
    }

    private void PlayHotFloorSound()
    {
        if (hotFloorSizzleSound == null)
        {
            Debug.LogWarning("[VeraoQuente] Hot Floor Sound não está configurado!");
            return;
        }

        AudioManager audioManager = AudioManager.GetInstance();
        if (audioManager != null)
        {
            audioManager.PlaySFX(hotFloorSizzleSound);
            Debug.Log("[VeraoQuente] Som de chão quente reproduzido!");
        }
        else
        {
            Debug.LogWarning("[VeraoQuente] AudioManager não encontrado - som ignorado!");
        }
    }

    public void ResetarTemperatura()
    {
        temperaturaAtual = temperaturaMaxima;
        Debug.Log("[VeraoQuente] Temperatura resetada para o máximo.");
    }
}