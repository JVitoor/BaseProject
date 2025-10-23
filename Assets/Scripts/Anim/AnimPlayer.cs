using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class AnimPlayer : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;
    public CharacterController controller;

    [Header("Parâmetros do Animator")]
    public string walkParam = "Walk";
    public string jumpParam = "Jump";          
    public string doubleJumpParam = "DoubleJump"; 
    public string glideParam = "Glide";         
    public string idleBlendParam = "IdleBlend"; // Float (0 = Idle1, 1 = Idle2)

    [Header("Configurações")]
    public float moveSpeedThreshold = 0.1f;   // Velocidade mínima para Walk
    public float idleSwitchInterval = 5f;     // Segundos para alternar idle

    private bool isGrounded;
    private bool canDoubleJump;
    private float idleTimer = 0f;
    private float currentIdleBlend = 0f;
    private bool idleDirection = true; 

    void Update()
    {
        UpdateMovement();
        UpdateJump();
        UpdateGlide();
        UpdateIdleBlend();
    }

    void UpdateMovement()
    {
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        if (animator != null)
            animator.SetBool(walkParam, speed > moveSpeedThreshold);
    }

    void UpdateJump()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded)
            canDoubleJump = true;

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
                animator.SetTrigger(jumpParam);
            else if (canDoubleJump)
            {
                animator.SetTrigger(doubleJumpParam);
                canDoubleJump = false;
            }
        }
    }

    void UpdateGlide()
    {
        bool isGliding = !isGrounded && Input.GetButton("Fire3");
        animator.SetBool(glideParam, isGliding);
    }

    void UpdateIdleBlend()
    {
        if (controller.velocity.magnitude < moveSpeedThreshold && isGrounded)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleSwitchInterval)
            {
                idleDirection = !idleDirection; // alterna entre Idle1 e Idle2
                idleTimer = 0f;
            }

            currentIdleBlend = idleDirection ? 0f : 1f;
            animator.SetFloat(idleBlendParam, currentIdleBlend);
        }
    }
}
