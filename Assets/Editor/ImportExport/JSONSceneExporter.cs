using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public string name;
    public Vector3 position;
}

[System.Serializable]
public class SceneData
{
    public List<ObjectData> objects = new List<ObjectData>();
}

public class JSONSceneExporter : EditorWindow
{
    [MenuItem("Tools/JSON Scene Exporter")]
    public static void Open()
    {
        GetWindow<JSONSceneExporter>("JSON Scene Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("JSON Scene Exporter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Export JSON", GUILayout.Height(40)))
        {
            ExportJSON();
        }
    }

    private void ExportJSON()
    {
        string path = EditorUtility.SaveFilePanel("Export JSON", "", "SceneObjects", "json");

        if (string.IsNullOrEmpty(path)) return;

        // creo il contenitore di dati dell'intera scena
        SceneData sceneData = new SceneData();

        GameObject[] sceneObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in sceneObjects)
        {
            // per ogni oggetto in scena mi creo il contenitore di dati dell'oggetto singolo
            ObjectData data = new ObjectData();

            data.name = obj.name;
            data.position = obj.transform.position;

            // aggiungo l'oggetto alla lista degli oggetti in scena
            sceneData.objects.Add(data);
        }

        // converto la struct in JSON
        string json = JsonUtility.ToJson(sceneData, true);

        File.WriteAllText(path, json);

        EditorUtility.DisplayDialog("Export Completed", "JSON exported successfully!", "OK");
    }
}
