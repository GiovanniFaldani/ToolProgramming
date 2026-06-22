using UnityEditor;
using UnityEngine;

// dice a Unity che questo Drawer viene utilizzato per tutte le property che hanno il ReadOnlyAttribute
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // disabilitiamo temporaneamente la GUI di default
        GUI.enabled = false;

        EditorGUI.PropertyField(position, property, label);

        GUI.enabled = true;
    }
}
