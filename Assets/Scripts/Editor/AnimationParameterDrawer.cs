#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VfxController.AnimationParameter))]
public class AnimationParameterDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Always single line height + optional second line for value (int / float)
        var typeProp = property.FindPropertyRelative("type");
        var type = (VfxController.ParameterType)typeProp.enumValueIndex;
        bool hasValue = type == VfxController.ParameterType.Int || type == VfxController.ParameterType.Float;
        int lines = hasValue ? 2 : 1;
        return EditorGUIUtility.singleLineHeight * lines + (lines - 1) * 2 + 4; // padding
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var nameProp = property.FindPropertyRelative("name");
        var typeProp = property.FindPropertyRelative("type");
        var intProp = property.FindPropertyRelative("intValue");
        var floatProp = property.FindPropertyRelative("floatValue");

        var type = (VfxController.ParameterType)typeProp.enumValueIndex;
        bool hasValue = type == VfxController.ParameterType.Int || type == VfxController.ParameterType.Float;

        // Background box
        GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
        var contentRect = new Rect(position.x + 4, position.y + 4, position.width - 8, position.height - 8);

        // First line: Name + Type
        var lineHeight = EditorGUIUtility.singleLineHeight;
        var nameRect = new Rect(contentRect.x, contentRect.y, contentRect.width * 0.55f, lineHeight);
        var typeRect = new Rect(contentRect.x + nameRect.width + 6, contentRect.y, contentRect.width * 0.45f - 6, lineHeight);

        EditorGUI.PropertyField(nameRect, nameProp, new GUIContent("Name"));
        EditorGUI.PropertyField(typeRect, typeProp, new GUIContent("Type"));

        // Second line only if has numeric value
        if (hasValue)
        {
            var valueRect = new Rect(contentRect.x, contentRect.y + lineHeight + 2, contentRect.width, lineHeight);
            if (type == VfxController.ParameterType.Int)
            {
                EditorGUI.PropertyField(valueRect, intProp, new GUIContent("Int Value"));
            }
            else // Float
            {
                EditorGUI.PropertyField(valueRect, floatProp, new GUIContent("Float Value"));
            }
        }
        else
        {
            // Show hint text for non-valued types
            var hintRect = new Rect(contentRect.x, contentRect.y + lineHeight + 2, contentRect.width, lineHeight);
            string hint = type switch
            {
                VfxController.ParameterType.Trigger => "Trigger: será disparado (sem valor).",
                VfxController.ParameterType.BoolTrue => "Bool: será definido para TRUE.",
                VfxController.ParameterType.BoolFalse => "Bool: será definido para FALSE.",
                _ => string.Empty
            };
            if (!string.IsNullOrEmpty(hint))
            {
                EditorGUI.LabelField(hintRect, hint, EditorStyles.miniLabel);
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
