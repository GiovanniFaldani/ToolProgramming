using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabUsageFinder : EditorWindow
{
    [MenuItem("Tools/Prefab Usage Finder")]
    public static void Open()
    {
        GetWindow<PrefabUsageFinder>("Prefab Usage Finder");
    }

    private GameObject prefabToFind;

    private List<GameObject> results = new List<GameObject>();

    private Vector2 scroll;

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Prefab Usage Finder", EditorStyles.boldLabel);
        GUILayout.Space(10);

        prefabToFind = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToFind, typeof(GameObject), true);
        GUILayout.Space(10);

        GUI.enabled = prefabToFind != null;

        if(GUILayout.Button("Search", GUILayout.Height(35)))
        {
            SearchPrefab();
        }

        GUI.enabled = true;
        GUILayout.Space(15);
        GUILayout.Label("Found: " + results.Count + " objects");
        GUILayout.Space(5);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (GameObject obj in results)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(obj.name);

            if(GUILayout.Button("Select", GUILayout.Width(70)))
            {
                Selection.activeGameObject = obj;
            }

            if(GUILayout.Button("Ping", GUILayout.Width(70)))
            {
               EditorGUIUtility.PingObject(obj);
            }

            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void SearchPrefab()
    {
        // refresh
        results.Clear();

        // prendiamo tutti gli oggetti della scena
        GameObject[] sceneObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in sceneObjects)
        {
            // per ciascuno guardiamo qual è il suo prefba d'origine
            GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
            
            if (sourcePrefab == null) continue;

            // aggiungiamo il prefab alla lista
            if(sourcePrefab == prefabToFind)
            {
                results.Add(obj);
            }
        }
    }
}
