using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class AnimPlayer : MonoBehaviour
{
    #region Components
    [Header("Components")]
    public Animator animator;
    public CharacterController controller;
    private Player player;
    #endregion

    #region Animation Parameters
    [Header("Animation Parameters")]
    public string movementParam = "Movement";
    public string idleBlendParam = "IdleBlend";
    public string jumpTrigger = "Jump";
    public string doubleJumpTrigger = "DoubleJump";
    public string isGroundedParam = "IsGrounded";
    public string velocityYParam = "VelocityY";
    #endregion

    #region Idle Settings
    [Header("Idle Settings")]
    public float idleSwapTime = 5f;
    private float idleTimer;
    private int currentIdleState = 0;
    #endregion

    #region Jump Settings
    [Header("Jump Settings")]
    private bool wasGrounded = true;
    private int lastJumpCount = 0;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        ValidateComponents();
    }

    private void Update()
    {
        if (!ValidateReferences()) return;

        UpdateMovementAnimation();
        UpdateJumpAnimation();
        UpdateIdleBlend();
        UpdateAnimatorParameters();
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        if (!controller) 
            controller = GetComponent<CharacterController>();
        
        if (!animator) 
            animator = GetComponentInChildren<Animator>();
        
        if (!player) 
            player = GetComponent<Player>();
    }

    private void ValidateComponents()
    {
        if (!controller)
        {
            Debug.LogError($"[AnimPlayer] CharacterController não encontrado em {gameObject.name}!");
        }

        if (!animator)
        {
            Debug.LogError($"[AnimPlayer] Animator não encontrado em {gameObject.name}!");
        }

        if (!player)
        {
            Debug.LogWarning($"[AnimPlayer] Player script não encontrado em {gameObject.name}. Algumas funcionalidades podem não funcionar.");
        }
    }

    private bool ValidateReferences()
    {
        return controller != null && animator != null;
    }

    #endregion

    #region Movement Animation

    private void UpdateMovementAnimation()
    {
        if (player == null) return;

        float speed = player.currentSpeed;
        
        if (speed > 0.1f)
        {
            animator.SetInteger(movementParam, 1);
        }
        else
        {
            animator.SetInteger(movementParam, 0);
        }
    }

    #endregion

    #region Jump Animation

    private void UpdateJumpAnimation()
    {
        if (player == null) return;

        bool isGrounded = controller.isGrounded;
        int currentJumpCount = player.jumpCount;

        if (!isGrounded && wasGrounded && currentJumpCount == 1)
        {
            animator.SetTrigger(jumpTrigger);
            lastJumpCount = 1;
        }
        else if (!isGrounded && currentJumpCount == 2 && lastJumpCount < 2)
        {
            animator.SetTrigger(doubleJumpTrigger);
            lastJumpCount = 2;
        }

        if (isGrounded && !wasGrounded)
        {
            animator.ResetTrigger(jumpTrigger);
            animator.ResetTrigger(doubleJumpTrigger);
            lastJumpCount = 0;
        }

        wasGrounded = isGrounded;
    }

    #endregion

    #region Idle Animation

    private void UpdateIdleBlend()
    {
        bool isIdle = controller.velocity.magnitude <= 0.1f && controller.isGrounded;

        if (isIdle)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleSwapTime)
            {
                currentIdleState = currentIdleState == 0 ? 1 : 0;
                animator.SetFloat(idleBlendParam, currentIdleState);
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
            if (currentIdleState != 0)
            {
                currentIdleState = 0;
                animator.SetFloat(idleBlendParam, 0f);
            }
        }
    }

    #endregion

    #region Animator Parameters

    private void UpdateAnimatorParameters()
    {
        if (player == null) return;

        animator.SetBool(isGroundedParam, controller.isGrounded);
        animator.SetFloat(velocityYParam, player.verticalVelocity);
    }

    #endregion

    #region Public Methods

    public void ForceJumpAnimation()
    {
        animator.SetTrigger(jumpTrigger);
    }

    public void ForceDoubleJumpAnimation()
    {
        animator.SetTrigger(doubleJumpTrigger);
    }

    public void ResetIdleBlend()
    {
        currentIdleState = 0;
        idleTimer = 0f;
        animator.SetFloat(idleBlendParam, 0f);
    }

    #endregion
}