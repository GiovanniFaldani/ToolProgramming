using System.IO;
using UnityEngine;
using UnityEditor;

public class SceneReportExporter : EditorWindow
{
    [MenuItem("Tools/Scene Report Exporter")]
    public static void Open()
    {
        GetWindow<SceneReportExporter>("Scene Report Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Scene Report Exporter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Export Screenshot + Report", GUILayout.Height(40)))
        {
            ExportScene();
        }
    }

    private void ExportScene()
    {
        string folder = EditorUtility.OpenFolderPanel("Choose Export Folder", "", "");

        if (string.IsNullOrEmpty(folder)) return;

        string screenshotPath = Path.Combine(folder, "SceneScreenshot.png");

        string reportPath = Path.Combine(folder, "SceneReport.txt");

        // salviamo lo screeenshot
        ScreenCapture.CaptureScreenshot(screenshotPath);

        Repaint();

        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);

        MeshRenderer[] meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);

        StreamWriter writer = new StreamWriter(reportPath);

        writer.WriteLine("SCENE REPORT");

        // va a capo con linea vuota
        writer.WriteLine();

        writer.WriteLine("GameObjects: " + objects.Length);
        writer.WriteLine("Cameras: " + cameras.Length);
        writer.WriteLine("Lights: " + lights.Length);
        writer.WriteLine("MeshRenderers: " + meshRenderers.Length);

        writer.WriteLine();

        writer.WriteLine("--------------------------");

        writer.WriteLine("OBJECT LIST");

        writer.WriteLine();

        foreach (GameObject obj in objects)
        {
            writer.WriteLine(obj.name);
        }

        writer.Close();

        EditorUtility.DisplayDialog("Export Completed", "Screenshot e report esportati correttamente!", "OK");
    }
}
