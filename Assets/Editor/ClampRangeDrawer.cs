using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ClampRangeAttribute))]

public class ClampRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ClampRangeAttribute att = this.attribute as ClampRangeAttribute;
        property.floatValue = EditorGUI.Slider(position, label, property.floatValue, att.min, att.max);
    }
}
