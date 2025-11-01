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

    private TargetController targetController;
    private bool wasHit = false;
    private Renderer flowerRenderer;
    private Color originalColor;

    void Start()
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
            originalColor = flowerRenderer.material.color;
        }

        SetFlowerState(false);
    }

    public void SetFlowerState(bool open)
    {
        isOpen = open;

        if (flowerAnimator != null)
        {
            if (!string.IsNullOrEmpty(openParameterName))
            {
                flowerAnimator.SetBool(openParameterName, open);
            }

            if (open && !string.IsNullOrEmpty(openTriggerName))
            {
                flowerAnimator.SetTrigger(openTriggerName);
            }
            else if (!open && !string.IsNullOrEmpty(closeTriggerName))
            {
                flowerAnimator.SetTrigger(closeTriggerName);
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

        StartCoroutine(HitFeedback());

        if (targetController != null)
        {
            targetController.OnFlowerHit(this);
        }
    }

    private IEnumerator HitFeedback()
    {
        if (flowerRenderer != null)
        {
            flowerRenderer.material.color = hitColor;
            yield return new WaitForSeconds(hitFeedbackDuration);
            flowerRenderer.material.color = originalColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            OnHit();
        }
    }
}
