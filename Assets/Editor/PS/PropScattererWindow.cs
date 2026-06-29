using UnityEditor;
using UnityEngine;

public class PropScattererWindow : EditorWindow
{
    [MenuItem("Tools/Create/Prop Scatterer")]
    public static void ShowWindow()
    {
        GetWindow<PropScattererWindow>("Prop Scatterer");
    }

    GameObject prefab;
    GameObject container;
    int amount = 20;
    float areaSize = 10;
    float brushSpacing = 1f;

    Vector2 scaleRandomRange = new Vector2(0.5f, 1.2f);
    Vector3 scatterPosition;
    Vector3 lastPaintPosition;

    bool paintMode = false;

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Space(15f);

        GUILayout.Label("Prop Scatterer", EditorStyles.boldLabel);

        GUILayout.Space(15f);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        amount = EditorGUILayout.IntSlider("Amount", amount, 1, 50);
        areaSize = EditorGUILayout.FloatField("Area Size", areaSize);
        brushSpacing = EditorGUILayout.FloatField("Brush Spacing", brushSpacing);
        scaleRandomRange = EditorGUILayout.Vector2Field("Scale Random", scaleRandomRange);

        paintMode = GUILayout.Toggle(paintMode, "Paint Mode");

        if (GUILayout.Button("Scatter"))
        {
            Scatter();
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!paintMode) return;

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Disegna un cerchio dove il cursore sta paintando
            Handles.color = Color.black;

            Handles.DrawWireDisc(hit.point, hit.normal, 3f);

            SceneView.RepaintAll();

            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
            {
                Paint(e.mousePosition);
                e.Use();
            }
        }
    }

    void Paint(Vector2 mousePos)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Distance(hit.point, lastPaintPosition) < brushSpacing) return;
            lastPaintPosition = hit.point;

            SpawnProp(hit.point);
        }
    }

    void SpawnProp(Vector3 pos)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Assign a prefab!");
            return;
        }

        ContainerCheck();

        for (int i = 0; i < amount; i++)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);

            Undo.RegisterCreatedObjectUndo(obj, "Paint prop");

            pos = new Vector3(Random.Range(pos.x - areaSize, pos.x + areaSize), 0, Random.Range(pos.z - areaSize, pos.z + areaSize));

            obj.transform.position = pos;

            SetRotationAndScale(obj);
        }
    }

    void Scatter()
    {
        if (prefab == null)
        {
            Debug.LogWarning("Assign a prefab!");
            return;
        }

        ContainerCheck();

        for (int i = 0; i < amount; i++)
        {
            //Vector3 position = new Vector3(Random.Range(-areaSize, areaSize), 0, Random.Range(-areaSize, areaSize));

            Vector3 rayOrigin = new Vector3(Random.Range(-areaSize, areaSize), 50, Random.Range(-areaSize, areaSize));

            Ray ray = new Ray(rayOrigin, Vector3.down);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                scatterPosition = hit.point;
            }
            else
            {
                continue;
            }

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);

            Undo.RegisterCreatedObjectUndo(obj, "Scatter prop");

            obj.transform.position = scatterPosition;

            SetRotationAndScale(obj);
        }
    }

    void ContainerCheck()
    {
        if (container == null)
            container = new GameObject("Generated Props");
        else
            container = GameObject.Find("Generated Props");
    }

    void SetRotationAndScale(GameObject obj)
    {
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        float scale = Random.Range(scaleRandomRange.x, scaleRandomRange.y);

        obj.transform.localScale = scale * Vector3.one;

    }
}
