using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(WarningIfAttribute))]
public class WarningIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        WarningIfAttribute warning = (WarningIfAttribute)attribute;
    
        EditorGUI.PropertyField(position, property, label);

        if(property.intValue <= warning.limit)
        {
            GUILayout.Space(50f);

            Rect helpBoxRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 5, position.width, EditorGUIUtility.singleLineHeight * 2);

            string message = string.IsNullOrEmpty(warning.message) ? "La vita non può essere negativa!" : warning.message;

            EditorGUI.HelpBox(helpBoxRect, message, MessageType.Warning);
        }

    }
}
