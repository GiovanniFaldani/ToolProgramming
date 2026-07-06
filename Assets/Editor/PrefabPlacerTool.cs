using UnityEngine;
using UnityEditor;

public class PrefabPlacerTool : EditorWindow
{
    GameObject prefab;
    GameObject previewObject;

    GameObject container;

    bool isPlacerActive;

    [MenuItem("Tools/Prefab Placer")]
    public static void OpenWindow()
    {
        GetWindow<PrefabPlacerTool>("Prefab Placer");
    }

    private void OnDisable()
    {
        // se chiudo la finestra chiudo tutto
        SceneView.duringSceneGui -= OnSceneGUI;
        ClearPreview();
    }

    private void OnGUI()
    {
        // slottino prefab nella window
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        // se il placer è disattivato
        if (!isPlacerActive)
        {
            if(GUILayout.Button("Enable Placement Mode"))
            {
                isPlacerActive = true;
                SceneView.duringSceneGui += OnSceneGUI;
            }
        }
        // se il placer è attivo
        else
        {
            if(GUILayout.Button("Disable Placement Mode"))
            {
                isPlacerActive = false;
                SceneView.duringSceneGui -= OnSceneGUI;

                // distruggiamo la preview
                ClearPreview();
            }
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if(!isPlacerActive || prefab == null)
            return;

        // mi prendo l'evento dell'input
        Event e = Event.current;

        // raggio che parte dal mouse
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        // se colpisco qualcosa
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // creare la preview e aggiornarla in base alla posizione e alla normale del cursore
            UpdatePreview(hit.point, hit.normal);

            // col tasto sinistro del mouse
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                // spawniamo il prefab
                PlacePrefab(hit.point, hit.normal);

                // impediamo l'uso del pulsante per altro
                e.Use(); 
            }
        }

        // refresho la scena
        sceneView.Repaint();
    }

    void UpdatePreview(Vector3 position, Vector3 normal)
    {
        // se non esiste già una preview in scena
        if (previewObject == null)
        {
            // la istanzio
            previewObject = (GameObject) PrefabUtility.InstantiatePrefab(prefab);

            // disattivo il collider della preview
            foreach(Collider col in previewObject.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        // ruotiamo l'oggetto in base alla normale
        previewObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

        // offset per non piazzare la preview dentro la superficie, ma sopra
        float offset = GetSurfaceOffset(previewObject);

        // piazzo la preview contando l'offset lungo la normale
        previewObject.transform.position = position + normal * offset;
    }

    void PlacePrefab(Vector3 position, Vector3 normal)
    {
        // controlliamo se l'empty che fa da parent esiste
        ContainerCheck();

        // istanziamo il prefab
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);

        // slavo modifiche nell'undo
        Undo.RegisterCreatedObjectUndo(obj, "Place Prefab");

        // ruotiamo il prefabi n base alla normale
        obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

        // offset per non piazzare l'oggetto dentro superficie
        float offset = GetSurfaceOffset(obj);

        // piazzo prefab sulla superficie contando offset lungo normale
        obj.transform.position = position + normal * offset;
    }

    void ContainerCheck()
    {
        if (container != null)
            container = GameObject.Find("Generated Props");
        else
            container = new GameObject("Generated Props");
    }

    void ClearPreview()
    {
        // distruggiamo la preview se esiste
        if(previewObject!= null)
            DestroyImmediate(previewObject);
    }

    float GetSurfaceOffset(GameObject obj)
    {
        // prendiamo tutti i renderer dell'oggetto
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        // se non ho neanche un renderer non do offset
        if (renderers.Length == 0) return 0f;

        // grandezza visiva del gameobject parent con renderer
        Bounds b = renderers[0].bounds;

        // per ogni figlio vado ad ingrandire i bounds in modo da racchiuderli tutti
        for (int i = 0; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        // restituisco metà grandezza sull'asse Y come offset
        return b.extents.y;
    }
}


