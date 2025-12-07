using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CheckpointTeleportButton : MonoBehaviour
{
    [Header("Checkpoint Configuration")]
    [Tooltip("ID do checkpoint para onde este botão teleportará o jogador")]
    public string checkpointID = "Checkpoint1";

    [Header("UI Configuration")]
    [Tooltip("Painel que será fechado ao teleportar (deixe vazio para detectar automaticamente)")]
    public GameObject panelToClose;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError($"[CheckpointTeleportButton] Button não encontrado em '{gameObject.name}'!");
        }
    }

    private void OnButtonClick()
    {
        TeleportToCheckpoint();
    }

    public void TeleportToCheckpoint()
    {
        Debug.Log($"[CheckpointTeleportButton] Teleportando para checkpoint: {checkpointID}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TeleportToCheckpoint(checkpointID);

            // Fecha o painel
            ClosePanelAndUnpause();
        }
        else
        {
            Debug.LogError("[CheckpointTeleportButton] GameManager não encontrado!");
        }
    }

    private void ClosePanelAndUnpause()
    {
        // Tenta fechar o painel especificado ou busca o painel pai
        GameObject panel = panelToClose;

        if (panel == null)
        {
            // Busca o painel pai que contém este botão
            Transform current = transform;
            while (current != null)
            {
                // Procura por um GameObject que tenha "Panel" no nome
                if (current.name.Contains("Panel"))
                {
                    panel = current.gameObject;
                    break;
                }
                current = current.parent;
            }
        }

        // Desativa o painel se encontrado
        if (panel != null)
        {
            panel.SetActive(false);
            Debug.Log($"[CheckpointTeleportButton] Painel '{panel.name}' fechado");
        }
        else
        {
            Debug.LogWarning("[CheckpointTeleportButton] Nenhum painel encontrado para fechar");
        }

        // Despause o jogo
        Time.timeScale = 1f;
        Debug.Log("[CheckpointTeleportButton] Jogo despausado");

        // Trava e esconde o cursor usando o InputManagers
        InputManagers inputManager = FindObjectOfType<InputManagers>();
        if (inputManager != null)
        {
            inputManager.LockCursor();
            Debug.Log("[CheckpointTeleportButton] Cursor travado via InputManagers");
        }
        else
        {
            // Fallback caso InputManagers não esteja disponível
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.LogWarning("[CheckpointTeleportButton] InputManagers não encontrado, usando fallback para travar cursor");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
