using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CSVSceneExporter : EditorWindow
{
    [MenuItem("Tools/CSV Scene Exporter")]
    public static void Open()
    {
        GetWindow<CSVSceneExporter>("CSV Scene Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("CSV Scene Exporter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if(GUILayout.Button("Export CSV", GUILayout.Height(40)))
        {
            ExportCSV();
        }
    }

    private void ExportCSV()
    {
        string path = EditorUtility.SaveFilePanel("Export CSV", "", "SceneObjects", "csv");

        if (string.IsNullOrEmpty(path)) return;

        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        StreamWriter writer = new StreamWriter(path);

        writer.WriteLine("Name, Position X, Position Y, Position Z"); // colonna A: Name, colonna B: Position X, etc...

        // per ogni altra riga metto le info dell'oggetto specifico
        foreach (GameObject obj in objects)
        {
            Vector3 position = obj.transform.position;

            // Esporta i float con separatore decimale .
            writer.WriteLine(obj.name + "," + 
                position.x.ToString(CultureInfo.InvariantCulture) + "," + 
                position.y.ToString(CultureInfo.InvariantCulture) + "," + 
                position.z.ToString(CultureInfo.InvariantCulture));
        }

        writer.Close();

        EditorUtility.DisplayDialog("Export Completed", "CSV exported successfully!", "OK");
    }
}
