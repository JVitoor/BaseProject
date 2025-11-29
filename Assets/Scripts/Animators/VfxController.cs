using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class VfxController : MonoBehaviour
{
    [Header("Lista de Animators")]
    [SerializeField] private List<Animator> animators = new List<Animator>();

    [Header("Lista de Parâmetros (mesmo índice que Animator)")]
    [Tooltip("Triggers e Bools NÃO possuem valor. Int e Float usam seus respectivos campos.")]
    [SerializeField] private List<AnimationParameter> parameters = new List<AnimationParameter>();

    [Header("Configuração Geral")]
    [SerializeField] private bool logDebug = false;

    [Header("Auto Reset")]
    [Tooltip("Ativa reset automático após disparar animações.")]
    [SerializeField] private bool autoResetEnabled = false;
    [Tooltip("Tempo em segundos antes de resetar os parâmetros disparados.")]
    [SerializeField] private float autoResetDelay = 2f;
    [Tooltip("Também reverte bools para false após o delay.")]
    [SerializeField] private bool resetBoolsToFalse = true;
    [Tooltip("Também redefine ints e floats para 0 após o delay.")]
    [SerializeField] private bool resetNumbersToZero = false;

    private Coroutine autoResetRoutine;

    #region Estruturas
    public enum ParameterType { Trigger, BoolTrue, BoolFalse, Int, Float }

    [System.Serializable]
    public class AnimationParameter
    {
        public string name;
        public ParameterType type = ParameterType.Trigger;
        // Usados SOMENTE quando type == Int ou Float (drawer oculta caso contrário)
        public int intValue;
        public float floatValue;

        public void Apply(Animator animator)
        {
            if (animator == null || string.IsNullOrEmpty(name)) return;
            switch (type)
            {
                case ParameterType.Trigger:
                    animator.SetTrigger(name);
                    break;
                case ParameterType.BoolTrue:
                    animator.SetBool(name, true);
                    break;
                case ParameterType.BoolFalse:
                    animator.SetBool(name, false);
                    break;
                case ParameterType.Int:
                    animator.SetInteger(name, intValue);
                    break;
                case ParameterType.Float:
                    animator.SetFloat(name, floatValue);
                    break;
            }
        }
    }
    #endregion

    #region Métodos Públicos
    public void PlayAllIndexed()
    {
        if (animators.Count == 0 || parameters.Count == 0) return;
        for (int i = 0; i < animators.Count; i++)
        {
            var animator = animators[i];
            var param = i < parameters.Count ? parameters[i] : parameters[parameters.Count - 1];
            param.Apply(animator);
            DebugLog($"[VfxController] Animator {i} - parâmetro '{param.name}' aplicado.");
        }
        ScheduleAutoReset();
    }

    public void PlayTriggerAll(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName)) return;
        foreach (var animator in animators)
        {
            if (animator == null) continue;
            animator.SetTrigger(triggerName);
            DebugLog($"[VfxController] Trigger '{triggerName}' enviado para {animator.name}.");
        }
        ScheduleAutoReset(triggerName);
    }

    public void SetBoolAll(string boolName, bool value)
    {
        if (string.IsNullOrEmpty(boolName)) return;
        foreach (var animator in animators)
        {
            if (animator == null) continue;
            animator.SetBool(boolName, value);
            DebugLog($"[VfxController] Bool '{boolName}'={value} em {animator.name}.");
        }
        ScheduleAutoReset(boolName);
    }

    public void PlayDynamicTriggers(IList<string> triggerList)
    {
        if (triggerList == null) return;
        for (int i = 0; i < animators.Count; i++)
        {
            var animator = animators[i];
            if (animator == null) continue;
            string trig = i < triggerList.Count ? triggerList[i] : triggerList[triggerList.Count - 1];
            if (!string.IsNullOrEmpty(trig))
            {
                animator.SetTrigger(trig);
                DebugLog($"[VfxController] Dynamic trigger '{trig}' aplicado em {animator.name}.");
            }
        }
        ScheduleAutoReset();
    }

    public void PlayDynamicParameters(IList<AnimationParameter> paramList)
    {
        if (paramList == null || paramList.Count == 0) return;
        for (int i = 0; i < animators.Count; i++)
        {
            var animator = animators[i];
            if (animator == null) continue;
            AnimationParameter p = i < paramList.Count ? paramList[i] : paramList[paramList.Count - 1];
            p.Apply(animator);
            DebugLog($"[VfxController] Dynamic parâmetro '{p.name}' aplicado em {animator.name}.");
        }
        ScheduleAutoReset();
    }

    public void ResetAllTriggers()
    {
        foreach (var animator in animators)
        {
            if (animator == null) continue;
            foreach (var p in parameters)
            {
                if (p.type == ParameterType.Trigger && !string.IsNullOrEmpty(p.name))
                {
                    animator.ResetTrigger(p.name);
                }
                if (resetBoolsToFalse && (p.type == ParameterType.BoolTrue || p.type == ParameterType.BoolFalse) && !string.IsNullOrEmpty(p.name))
                {
                    animator.SetBool(p.name, false);
                }
                if (resetNumbersToZero)
                {
                    if (p.type == ParameterType.Int && !string.IsNullOrEmpty(p.name)) animator.SetInteger(p.name, 0);
                    if (p.type == ParameterType.Float && !string.IsNullOrEmpty(p.name)) animator.SetFloat(p.name, 0f);
                }
            }
        }
        DebugLog("[VfxController] Reset automático executado.");
    }

    public void AddAnimator(Animator animator)
    {
        if (animator != null && !animators.Contains(animator))
        {
            animators.Add(animator);
            DebugLog($"[VfxController] Animator '{animator.name}' adicionado.");
        }
    }

    public void RemoveAnimator(Animator animator)
    {
        if (animator != null && animators.Remove(animator))
        {
            DebugLog($"[VfxController] Animator '{animator.name}' removido.");
        }
    }

    public void ForceAutoResetNow()
    {
        if (autoResetRoutine != null)
        {
            StopCoroutine(autoResetRoutine);
            autoResetRoutine = null;
        }
        ResetAllTriggers();
    }
    #endregion

    #region Auto Reset Animations
    private void ScheduleAutoReset(string singleParamName = null)
    {
        if (!autoResetEnabled) return;
        if (autoResetRoutine != null)
        {
            StopCoroutine(autoResetRoutine);
        }
        autoResetRoutine = StartCoroutine(AutoResetCoroutine());
        DebugLog($"[VfxController] Auto reset agendado ({autoResetDelay}s) {(singleParamName != null ? "para '" + singleParamName + "'" : "para parâmetros disparados")}");
    }

    private IEnumerator AutoResetCoroutine()
    {
        yield return new WaitForSeconds(autoResetDelay);
        ResetAllTriggers();
        autoResetRoutine = null;
    }
    #endregion

    #region Util
    private void DebugLog(string msg)
    {
        if (logDebug)
        {
            Debug.Log(msg, this);
        }
    }
    #endregion
}
