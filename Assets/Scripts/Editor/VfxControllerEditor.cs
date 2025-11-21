#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VfxController))]
public class VfxControllerEditor : Editor
{
    private string triggerName = "";
    private string boolName = "";
    private bool boolValue = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var ctrl = (VfxController)target;

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Teste Rápido", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Ações rápidas para testar o VfxController", EditorStyles.miniLabel);
        EditorGUILayout.Space(6);

        // Play All Indexed
        if (GUILayout.Button("Play All Indexed"))
        {
            ctrl.PlayAllIndexed();
        }

        EditorGUILayout.Space(6);

        // Trigger All (campo + botão)
        EditorGUILayout.BeginHorizontal();
        triggerName = EditorGUILayout.TextField("Trigger Name", triggerName);
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(triggerName));
        if (GUILayout.Button("Play Trigger All", GUILayout.Width(140)))
        {
            ctrl.PlayTriggerAll(triggerName);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        // Bool All (campo + toggle + botão)
        EditorGUILayout.BeginHorizontal();
        boolName = EditorGUILayout.TextField("Bool Name", boolName);
        boolValue = EditorGUILayout.ToggleLeft("Value", boolValue, GUILayout.Width(80));
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(boolName));
        if (GUILayout.Button("Set Bool All", GUILayout.Width(120)))
        {
            ctrl.SetBoolAll(boolName, boolValue);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        // Reset Triggers
        if (GUILayout.Button("Reset All Triggers"))
        {
            if (EditorUtility.DisplayDialog("Confirmar", "Resetar todos os triggers?", "OK", "Cancelar"))
            {
                ctrl.ResetAllTriggers();
            }
        }

        EditorGUILayout.EndVertical();
    }
}
#endif
