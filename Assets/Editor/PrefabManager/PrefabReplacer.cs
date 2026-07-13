using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.LowLevel;

public class PrefabReplacer : EditorWindow
{
    [MenuItem("Tools/Prefab Replacer")]
    public static void Open()
    {
        GetWindow<PrefabReplacer>("Prefab Replacer");
    }

    private GameObject oldPrefab;
    private GameObject newPrefab;

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Replace Prefabs", EditorStyles.boldLabel);
        GUILayout.Space(10);

        oldPrefab = (GameObject)EditorGUILayout.ObjectField("Old Prefab", oldPrefab, typeof(GameObject), false);
        newPrefab = (GameObject)EditorGUILayout.ObjectField("New Prefab", newPrefab, typeof(GameObject), false);
        GUILayout.Space(20);

        // abilitare/disabilitare il click di interazione
        GUI.enabled = oldPrefab != null && newPrefab != null;

        if(GUILayout.Button("replace All", GUILayout.Height(40)))
        {
            ReplacePrefabs();
        }

        GUI.enabled = true;
    }

    private void ReplacePrefabs()
    {
        GameObject[] sceneObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in sceneObjects)
        {
            // recupero il prefab da cui deriva l'oggetto
            GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

            // se non deriva da un prefab passo oltre
            if (sourcePrefab == null) continue;

            // se non è il prefab che vogliamo sostituire, skippa
            if (sourcePrefab != oldPrefab) continue;

            // prendiamo le info del vecchio prefab
            Vector3 position = obj.transform.position;
            Quaternion rotation = obj.transform.rotation;
            Vector3 scale = obj.transform.localScale;
            Transform parent = obj.transform.parent;

            // instanziamo quello nuovo
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);

            // gli incolliamo le info
            newObject.transform.position = position;
            newObject.transform.rotation = rotation;
            newObject.transform.localScale = scale;
            newObject.transform.parent = parent;

            // salviamo l'undo
            Undo.RegisterCreatedObjectUndo(newObject, "Replace Prefab");

            // distruggiamo quello vecchio
            Undo.DestroyObjectImmediate(obj);
        }
    }
}
