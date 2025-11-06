using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;

public class FlowerTarget : MonoBehaviour
{
    [Header("Estado da Flor")]
    public bool isOpen = false;

    [Header("Animação")]
    public Animator flowerAnimator;

    public string openParameterName = "IsOpen";

    public string openTriggerName = "";

    public string closeTriggerName = "";

    [Header("Configurações de Tempo")]
    public float openDuration = 2f;

    public float closedDuration = 3f;

    [Header("Feedback Visual")]
    public Color hitColor = Color.green;
    public float hitFeedbackDuration = 0.3f;

    [Header("Performance")]
    [SerializeField] private bool useMaterialPropertyBlock = true;

    private TargetController targetController;
    private bool wasHit = false;
    private Renderer flowerRenderer;
    private Color originalColor;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorPropertyID = Shader.PropertyToID("_Color");
    private Material cachedMaterial;
    private Coroutine hitFeedbackCoroutine;
    private int openParameterHash;
    private int openTriggerHash;
    private int closeTriggerHash;

    void Start()
    {
        CacheAnimationHashes();
        InitializeComponents();
        SetFlowerState(false);
    }

    private void CacheAnimationHashes()
    {
        if (!string.IsNullOrEmpty(openParameterName))
            openParameterHash = Animator.StringToHash(openParameterName);
        
        if (!string.IsNullOrEmpty(openTriggerName))
            openTriggerHash = Animator.StringToHash(openTriggerName);
        
        if (!string.IsNullOrEmpty(closeTriggerName))
            closeTriggerHash = Animator.StringToHash(closeTriggerName);
    }

    private void InitializeComponents()
    {
        targetController = GetComponentInParent<TargetController>();

        if (targetController == null)
        {
            Debug.LogWarning("[FlowerTarget] TargetController não encontrado no pai!");
        }

        if (flowerAnimator == null)
        {
            flowerAnimator = GetComponent<Animator>();

            if (flowerAnimator == null)
            {
                flowerAnimator = GetComponentInChildren<Animator>();
            }

            if (flowerAnimator == null)
            {
                Debug.LogError("[FlowerTarget] Animator não encontrado em: " + gameObject.name);
            }
        }

        flowerRenderer = GetComponentInChildren<Renderer>();
        if (flowerRenderer != null)
        {
            if (useMaterialPropertyBlock)
            {
                // Use MaterialPropertyBlock to avoid creating material instances
                propertyBlock = new MaterialPropertyBlock();
                flowerRenderer.GetPropertyBlock(propertyBlock);
                
                // Get original color from the property block or material
                if (propertyBlock.isEmpty)
                {
                    originalColor = flowerRenderer.sharedMaterial.GetColor(ColorPropertyID);
                }
                else
                {
                    originalColor = propertyBlock.GetColor(ColorPropertyID);
                }
            }
            else
            {
                // Cache material instance (creates one instance per flower)
                cachedMaterial = flowerRenderer.material;
                originalColor = cachedMaterial.color;
            }
        }
    }

    public void SetFlowerState(bool open)
    {
        isOpen = open;

        if (flowerAnimator != null)
        {
            if (openParameterHash != 0)
            {
                flowerAnimator.SetBool(openParameterHash, open);
            }

            if (open && openTriggerHash != 0)
            {
                flowerAnimator.SetTrigger(openTriggerHash);
            }
            else if (!open && closeTriggerHash != 0)
            {
                flowerAnimator.SetTrigger(closeTriggerHash);
            }
        }

        Debug.Log("[FlowerTarget] " + gameObject.name + " está " + (open ? "ABERTA" : "FECHADA"));
    }

    public IEnumerator OpenTemporarily()
    {
        wasHit = false;
        SetFlowerState(true);

        yield return new WaitForSeconds(openDuration);

        SetFlowerState(false);

        if (targetController != null)
        {
            targetController.OnFlowerClosed(this, wasHit);
        }
    }

    public void OnHit()
    {
        if (!isOpen)
        {
            Debug.Log("[FlowerTarget] " + gameObject.name + " foi acertada mas estava FECHADA!");
            return;
        }

        if (wasHit)
        {
            Debug.Log("[FlowerTarget] " + gameObject.name + " já foi acertada!");
            return;
        }

        wasHit = true;
        Debug.Log("[FlowerTarget] ✓ " + gameObject.name + " foi ACERTADA!");

        // Stop previous feedback coroutine if running
        if (hitFeedbackCoroutine != null)
        {
            StopCoroutine(hitFeedbackCoroutine);
        }
        hitFeedbackCoroutine = StartCoroutine(HitFeedback());

        if (targetController != null)
        {
            targetController.OnFlowerHit(this);
        }
    }

    private IEnumerator HitFeedback()
    {
        if (flowerRenderer != null)
        {
            if (useMaterialPropertyBlock)
            {
                // Use MaterialPropertyBlock - more efficient, no material instances created
                propertyBlock.SetColor(ColorPropertyID, hitColor);
                flowerRenderer.SetPropertyBlock(propertyBlock);
                
                yield return new WaitForSeconds(hitFeedbackDuration);
                
                propertyBlock.SetColor(ColorPropertyID, originalColor);
                flowerRenderer.SetPropertyBlock(propertyBlock);
            }
            else
            {
                // Use cached material instance
                if (cachedMaterial != null)
                {
                    cachedMaterial.color = hitColor;
                    yield return new WaitForSeconds(hitFeedbackDuration);
                    cachedMaterial.color = originalColor;
                }
            }
        }

        hitFeedbackCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cache GetComponent call result
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            OnHit();
        }
    }

    private void OnDestroy()
    {
        // Clean up material instance if we created one
        if (!useMaterialPropertyBlock && cachedMaterial != null)
        {
            Destroy(cachedMaterial);
        }
    }

    // Public method to reset flower state (useful for object pooling)
    public void ResetFlower()
    {
        wasHit = false;
        isOpen = false;
        
        if (hitFeedbackCoroutine != null)
        {
            StopCoroutine(hitFeedbackCoroutine);
            hitFeedbackCoroutine = null;
        }

        if (flowerRenderer != null)
        {
            if (useMaterialPropertyBlock)
            {
                propertyBlock.SetColor(ColorPropertyID, originalColor);
                flowerRenderer.SetPropertyBlock(propertyBlock);
            }
            else if (cachedMaterial != null)
            {
                cachedMaterial.color = originalColor;
            }
        }
    }
}
