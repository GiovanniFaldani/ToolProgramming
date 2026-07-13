using System.IO;
using UnityEngine;
using UnityEditor;

public class JSONSceneImporter : EditorWindow
{
    [MenuItem("Tools/JSON Scene Importer")]
    public static void Open()
    {
        GetWindow<JSONSceneImporter>("JSON Scene Importer");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("JSON Scene Importer", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Import JSON", GUILayout.Height(40)))
        {
            ImportJSON();
        }
    }

    private void ImportJSON()
    {
        string path = EditorUtility.OpenFilePanel("Import JSON", "", "json");

        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);

        // converto tutto il testo JSON in scene data
        SceneData sceneData = JsonUtility.FromJson<SceneData>(json);

        if(sceneData == null) return;

        foreach(ObjectData data in sceneData.objects)
        {
            GameObject obj = new GameObject(data.name);

            obj.transform.position = data.position;
            Undo.RegisterCreatedObjectUndo(obj, "Import JSON");
        }

        EditorUtility.DisplayDialog("Import Completed", "JSON imported succesfully!", "OK");
    }
}
