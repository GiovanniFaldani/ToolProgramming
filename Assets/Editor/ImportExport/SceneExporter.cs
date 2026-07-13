using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneExporter : EditorWindow
{
    [MenuItem("Tools/Scene Exporter")]
    public static void Open()
    {
        GetWindow<SceneExporter>("Scene Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Scene Exporter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if(GUILayout.Button("Export Scene", GUILayout.Height(40)))
        {
            ExportScene();
        }
    }

    private void ExportScene()
    {
        // prendiamo i lpath in cui salvare il file e la sua estensione
        string path = EditorUtility.SaveFilePanel("Export Scene", "", "SceneObjects", "txt");

        // se annulliamo skippo tutto
        if (string.IsNullOrEmpty(path)) return;

        // prendiamo tutti gli oggetti in scena
        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        // creiamo un file di testo in quel path
        StreamWriter writer = new StreamWriter(path);

        // creiamo una riga per ogni oggetto e scriviamo il suo nome
        foreach (GameObject obj in objects)
        {
            writer.WriteLine(obj.name);
        }

        // chiudiamo il file e salvaimo i sati
        writer.Close();

        // popup con messaaggio di conferma
        EditorUtility.DisplayDialog("Export Completed", "Ecene exported successfully!", "Ok");
    }
}
