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

        // PlayAllIndexed
        if (GUILayout.Button("Play All Indexed"))
        {
            ctrl.PlayAllIndexed();
        }

        EditorGUILayout.Space(4);

        // Trigger All
        triggerName = EditorGUILayout.TextField("Trigger Name", triggerName);
        if (GUILayout.Button("Play Trigger All"))
        {
            ctrl.PlayTriggerAll(triggerName);
        }

        EditorGUILayout.Space(4);

        // Bool All
        boolName = EditorGUILayout.TextField("Bool Name", boolName);
        boolValue = EditorGUILayout.Toggle("Bool Value", boolValue);
        if (GUILayout.Button("Set Bool All"))
        {
            ctrl.SetBoolAll(boolName, boolValue);
        }

        EditorGUILayout.Space(4);

        // Reset Triggers
        if (GUILayout.Button("Reset All Triggers"))
        {
            ctrl.ResetAllTriggers();
        }
    }
}
#endif
