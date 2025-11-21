using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class VeraoQuente : MonoBehaviour
{
    [Header("Configurações da Temperatura")]
    public float temperaturaMaxima = 100f;
    public float taxaDeDano = 15f;
    public float taxaDeRecuperacao = 8f;

    [Header("Efeitos Visuais e Sonoros")]
    [Tooltip("Prefab com Visual Effect de fumaça (GameObject contendo componente VisualEffect)")]
    public GameObject smokeVFXPrefab;

    [Tooltip("Som que toca quando o player entra no chão quente")]
    public AudioClip hotFloorSizzleSound;

    private VisualEffect currentVFX;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEstaNoChaoQuente = true;
            GameManager.Instance.veraoQuente = this;

            // Spawna o efeito de fumaça no pé do player
            SpawnSmokeEffect(other.transform);

            // Toca o som de chão quente
            PlayHotFloorSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEstaNoChaoQuente = false;

            // Remove o efeito de fumaça quando o player sai do chão quente
            RemoveSmokeEffect();
        }
    }

    private void SpawnSmokeEffect(Transform playerTransform)
    {

        // Remove efeito anterior se existir
        RemoveSmokeEffect();

        // Instancia o GameObject com Visual Effect
        GameObject vfxObject = Instantiate(smokeVFXPrefab, playerTransform);

        // Define a posição LOCAL do efeito (relativa ao player)
        vfxObject.transform.localPosition = new Vector3(0f, -8f, 0f);
        vfxObject.transform.localRotation = Quaternion.identity;
        vfxObject.transform.localScale = Vector3.one;

        // Obtém o componente Visual Effect
        currentVFX = vfxObject.GetComponent<VisualEffect>();

        if (currentVFX != null)
        {
            // Inicia o Visual Effect
            currentVFX.Play();
            Debug.Log("[VeraoQuente] Visual Effect iniciado!");
        }
        else
        {
            Debug.LogError("[VeraoQuente] O prefab não contém um componente VisualEffect!");
            Destroy(vfxObject);
        }
    }

    private void RemoveSmokeEffect()
    {
        if (currentVFX != null)
        {
            // Para o Visual Effect
            currentVFX.Stop();
            Debug.Log("[VeraoQuente] Visual Effect parado!");

            // Destrói o efeito após as partículas existentes desaparecerem
            Destroy(currentVFX.gameObject, 2f);
            currentVFX = null;

            Debug.Log("[VeraoQuente] Efeito de fumaça removido!");
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