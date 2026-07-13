using UnityEngine;
using UnityEditor;

public class ReplaceSelection : EditorWindow
{
    [MenuItem("Tools/Replace Selection")]
    public static void Open()
    {
        GetWindow<ReplaceSelection>("Replace Selection");
    }

    private GameObject replacementPrefab;

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Replace Selected Objects", EditorStyles.boldLabel);
        GUILayout.Space(10);

        replacementPrefab = (GameObject)EditorGUILayout.ObjectField("Replacement Prefab", replacementPrefab, typeof(GameObject), false);
        
        GUILayout.Space(20);
        GUI.enabled = replacementPrefab != null;

        if(GUILayout.Button("Replace Selection", GUILayout.Height(40)))
        {
            ReplaceObjects();
        }

        GUI.enabled = true;

        GUILayout.Space(10);

        GUILayout.Label("Selected Objects: " + Selection.gameObjects.Length);
    }

    private void ReplaceObjects()
    {
        foreach(GameObject selectedObject in Selection.gameObjects)
        {
            Vector3 position = selectedObject.transform.position;
            Quaternion rotation = selectedObject.transform.rotation;
            Vector3 scale = selectedObject.transform.localScale;
            Transform parent = selectedObject.transform.parent;

            int siblingIndex = selectedObject.transform.GetSiblingIndex();

            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);

            newObject.transform.position = position;
            newObject.transform.rotation = rotation;
            newObject.transform.localScale = scale;
            newObject.transform.parent = parent;

            newObject.transform.SetSiblingIndex(siblingIndex);
            newObject.name = selectedObject.name;
            newObject.layer = selectedObject.layer;
            newObject.tag = selectedObject.tag;

            Undo.RegisterCreatedObjectUndo(newObject, "Replace Selection");

            Undo.DestroyObjectImmediate(selectedObject);

            Selection.activeGameObject = newObject;
        }
    }
}