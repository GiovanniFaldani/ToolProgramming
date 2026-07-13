using UnityEditor;
using UnityEngine;

public class RoomPlacer : EditorWindow
{
    [MenuItem("Tools/Room Placer")]
    public static void Open()
    {
        GetWindow<RoomPlacer>("Room Placer");
    }

    private bool isPlacementMode = false;
    public bool rangeVisibility = false;
    public float snapRange = 20f;
    public GameObject selectedPrefab = null;

    private void OnGUI()
    {
        GUILayout.Space(10f);
        GUILayout.Label("Room Placer", EditorStyles.boldLabel);
        GUILayout.Space(10f);

        // Se la placement mode è disattiva
        if (!isPlacementMode)
        {
            if (GUILayout.Button("Enable Placement Mode"))
            {
                isPlacementMode = true;
                SceneView.duringSceneGui += OnSceneGUI;
            }
        }
        // se la placement mode è attiva
        else
        {
            if (GUILayout.Button("Disable Placement Mode"))
            {
                isPlacementMode = false;
                SceneView.duringSceneGui -= OnSceneGUI;

                // distruggo la preview
                ClearPreview();
            }

            EditorGUILayout.Toggle("View Snap Radius", rangeVisibility);

            GUILayout.Space(10f);

            EditorGUILayout.FloatField("Snap Radius", snapRange);

            GUILayout.Space(10f);
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacementMode)
            return;


        // mi prendo l'evento dell'input
        Event e = Event.current;

        // raggio che parte dal mouse
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        // refresh della scena
        sceneView.Repaint();
    }

    private void OnDisable()
    {
        // se chiudo la finestra chiudo tutto
        SceneView.duringSceneGui -= OnSceneGUI;
        ClearPreview();
    }

    void ClearPreview()
    {
        // distruggiamo la preview se esiste
        if (selectedPrefab != null)
            DestroyImmediate(selectedPrefab);
    }


}
