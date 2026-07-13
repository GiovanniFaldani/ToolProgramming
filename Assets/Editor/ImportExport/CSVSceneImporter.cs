using System.IO;
using UnityEngine;
using UnityEditor;

public class CSVSceneImporter : EditorWindow
{
    [MenuItem("Tools/CSV Scene Importer")]
    public static void Open()
    {
        GetWindow<CSVSceneImporter>("CSV Scene Importer");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("CSV Scene Importer", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Import CSV", GUILayout.Height(40)))
        {
            ImportCSV();
        }
    }

    private void ImportCSV()
    {
        string path = EditorUtility.OpenFilePanel("Import CSV", "", "csv");

        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        
        // skip header line
        for (int i = 1; i < lines.Length; i++)
        {
            // separatore ,
            string[] values = lines[i].Split(',');

            if (values.Length != 4) continue;

            // prendo la colonna dei nome e assegno il nome
            string objectName = values[0];

            // converto le stringhe contenute nelle celle in numeri da aseegnare alla posizione
            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            GameObject obj = new GameObject(objectName);

            obj.transform.position = new Vector3(x, y, z);

            Undo.RegisterCreatedObjectUndo(obj, "Import CSV");

        }

        EditorUtility.DisplayDialog("Import Completed", "CSV imported succesfully!", "OK");
    }
}
