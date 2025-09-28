using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    [Header(" └─ Dash Settings")]
    public float dashSpeed = 15f;       // velocidade do dash
    public float dashDuration = 0.2f;   // quanto tempo dura
    public float dashCooldown = 1f;     // tempo até poder usar de novo

    private CharacterController controller; // Referência ao CharacterController
    private bool isDashing = false; // Indica se o player está usando o dash
    private bool canDash = true; // Indica que o player pode usar o dash
    
    // Variáveis de controle de tempo
    private float dashTimer = 0f; // Timer para controlar a duração do dash
    private float cooldownTimer = 0f; // Timer para controlar o cooldown

    void Start()
    {
        // Atribui o componente CharacterController que está no mesmo GameObject do player
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleDash();
        HandleCooldown();
    }

    // Método para ser chamado pelo Input System
    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            StartDash();
        }
    }

    // Inicia o dash
    public void StartDash()
    {
        if (!canDash || isDashing) return;

        canDash = false; // Faz o dash entrar em cooldown
        isDashing = true; // Marca que o dash está sendo utilizado
        dashTimer = dashDuration; // Define o tempo de duração do dash
    }

    // Gerencia o comportamento durante o dash
    private void HandleDash()
    {
        if (!isDashing) return;

        // Decrementa o timer do dash
        dashTimer -= Time.deltaTime;

        // Move o player para frente durante o dash
        if (controller != null)
        {
            Vector3 dashMove = transform.forward * dashSpeed;
            controller.Move(dashMove * Time.deltaTime);
        }

        // Se o timer chegou a zero, finaliza o dash
        if (dashTimer <= 0f)
        {
            isDashing = false; // Finaliza o dash
            cooldownTimer = dashCooldown; // Inicia o cooldown
            Debug.Log("[Dash] Dash finalizado! Cooldown iniciado.");
        }
    }

    // Gerencia o cooldown do dash
    private void HandleCooldown()
    {
        if (canDash || cooldownTimer <= 0f) return;

        // Decrementa o timer do cooldown
        cooldownTimer -= Time.deltaTime;

        // Se o cooldown acabou, permite usar o dash novamente
        if (cooldownTimer <= 0f)
        {
            canDash = true; // Libera o próximo dash
            Debug.Log("[Dash] Dash disponível novamente!");
        }
    }

    // Método público para verificar se pode fazer dash
    public bool CanDash()
    {
        return canDash && !isDashing;
    }

    // Método público para verificar se está fazendo dash
    public bool IsDashing()
    {
        return isDashing;
    }

    // Método público para obter o tempo restante do cooldown
    public float GetCooldownTimeRemaining()
    {
        return Mathf.Max(0f, cooldownTimer);
    }
}
