using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(VfxController.AnimationParameter))]
public class AnimatorParamerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // name + type always; value only for certain types
        int lines = 2;
        var typeProp = property.FindPropertyRelative("type");
        if (typeProp != null)
        {
            if (typeProp.enumValueIndex == (int)VfxController.ParameterType.Int ||
                typeProp.enumValueIndex == (int)VfxController.ParameterType.Float)
            {
                lines = 3;
            }
        }

        float totalHeight = EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects using standard spacing
        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Name field
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent("Name Parameter"));

        // Type field
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("type"), new GUIContent("Type Parameter"));

        // Value field (conditional)
        var typeProp = property.FindPropertyRelative("type");
        if (typeProp != null)
        {
            if (typeProp.enumValueIndex == (int)VfxController.ParameterType.Int)
            {
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("intValue"), new GUIContent("Int Value"));
            }
            else if (typeProp.enumValueIndex == (int)VfxController.ParameterType.Float)
            {
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("floatValue"), new GUIContent("Float Value"));
            }
        }

        EditorGUI.EndProperty();
    }
}
