using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProgressBarAttribute))]
public class ProgressBarDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ProgressBarAttribute att = this.attribute as ProgressBarAttribute;

        float value = property.floatValue;
        float percentage = value / att.maxValue;

        GUILayout.Space(10);

        EditorGUI.ProgressBar(position, percentage, label.text + " : " + value);

        GUILayout.Space(10);
    }
}
