using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RoomPlacer : EditorWindow
{
    // classe per memorizzare i dati di una cartella di prefab, contiene una lista di prefab (stanze) al suo interno
    private class CategoryData
    {
        public string name;
        public bool foldout; // stato del foldout: aperto/chiuso
        public string[] prefabPaths; // path relativi a Unity
        public string[] prefabNames;
        public GameObject[] prefabAssets; // riferimenti ai prefab in memoria
    }

    [MenuItem("Tools/Room Placer")]
    public static void Open()
    {
        GetWindow<RoomPlacer>("Room Placer");
        ScanFolders();
    }

    // Attiva la placement mode
    private bool isPlacementMode = false;

    // Toggli visibilità del raggio
    private bool radiusVisibility = false;

    // raggio di snap riscontrato anche nella preview
    private float snapRadius = 20f;

    // categoria di stanze selezionata (= numero di porte)
    private CategoryData selectedCategory = null;

    // Tipo di prefab di stanza selezionato
    private GameObject selectedPrefab = null;

    // istanza in scena del prefab selezionato
    private GameObject previewObject = null;

    // se il giocatore sta correntemente piazzando una stanza
    private bool isPlacing = false;

    // scroll del menu delle stanze
    private Vector2 scrollPos;

    // Lista di categorie di stanze
    private static readonly List<CategoryData> categories = new List<CategoryData>();
    private const string RootFolder = "Assets/Prefab/Rooms";

    // grandezze dell'HUD per selezionare la stanza da piazzare
    private float prefabButtonHeight = 100;
    private float prefabButtonWidth = 200;

    // indice dell'ultima stanza selezionata nella lista della categoria
    private int lastSelectIndex = int.MinValue;

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

                ScanFolders();
            }
        }
        // se la placement mode è attiva
        else
        {
            if (GUILayout.Button("Disable Placement Mode"))
            {
                isPlacementMode = false;
                lastSelectIndex = int.MinValue;
                SceneView.duringSceneGui -= OnSceneGUI;

                // distruggo la preview
                ClearPreview();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Refresh")) ScanFolders();

            GUILayout.Space(10f);

            radiusVisibility = EditorGUILayout.Toggle("View Snap Radius", radiusVisibility);

            GUILayout.Space(10f);

            snapRadius = EditorGUILayout.FloatField("Snap Radius", snapRadius);

            GUILayout.Space(10f);

            // pulsanti di selezione categoria
            foreach (CategoryData cat in categories)
            {
                if (GUILayout.Button(cat.name))
                {
                    selectedCategory = cat;
                    lastSelectIndex = int.MinValue;
                    SceneView.RepaintAll();
                }
            }

            GUILayout.Space(10f);

            if(selectedPrefab != null)
                EditorGUILayout.LabelField($"Currently Selected Prefab: {selectedPrefab.name}");
            else
                EditorGUILayout.LabelField($"Currently Selected Prefab: None");

            GUILayout.Space(10f);
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Debug.Log("OnSceneGUI tick");

        if (!isPlacementMode || selectedCategory == null)
            return;

        if (categories.Count <= 0)
        {
            EditorGUILayout.HelpBox("No prefab found in the specified folder", MessageType.Info);
            return;
        }

        Handles.BeginGUI();

        // Draw dei bottoni in scena per spawnare prefab

        // mi prendo l'evento dell'input
        Event e = Event.current;

        // crea uno scrolling menu di ogni prefab nella categoria corrente
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(1000));

        for(int i = 0; i < selectedCategory.prefabAssets.Length; i++)
        {
            GameObject asset = selectedCategory.prefabAssets[i];

            // GetAssetPreview è asincrono
            Texture2D preview = AssetPreview.GetAssetPreview(asset);

            // GUI CONTENT combina immagine + tooltip in un unico oggetto
            GUIContent content;

            if (preview != null)
            {
                content = new GUIContent(preview, selectedCategory.prefabNames[i]);
            }
            else
            {
                content = new GUIContent(selectedCategory.prefabNames[i]);
            }

            GUILayout.Space(10f);

            if (i == lastSelectIndex) GUI.backgroundColor = Color.lightBlue;
            else GUI.backgroundColor = Color.white;

            // quando clicco sul pulsante del prefab, cambio il selezionato e consumo l'input
            if (GUILayout.Button(content, GUILayout.Width(prefabButtonWidth), GUILayout.Height(prefabButtonHeight)))
            {
                Debug.Log($"Cliccato su: {selectedCategory.prefabNames[i]}");
                selectedPrefab = asset;
                lastSelectIndex = i;
                Repaint();
                e.Use();

                // Istanzia prefab in scena
                previewObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);

                isPlacing = true;

                // TODO register this undo when the prefab is PLACED, not spawned
                Undo.RegisterCreatedObjectUndo(previewObject, "Spawn Room prefab");

            }

            GUI.backgroundColor = Color.white;
        }

        // annulla preview o esegue undo con tasto UI
        if (GUILayout.Button("Undo", GUILayout.Width(prefabButtonWidth), GUILayout.Height(prefabButtonHeight)))
        {
            if(isPlacing)
            {
                ClearPreview();
                isPlacing = false;
            }
            else
            {
                Undo.PerformUndo();
            }
        }

        // annulla il piazzamento con ctrl+z per consistenza con tasto undo
        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z && isPlacing)
        {
            ClearPreview();
            isPlacing = false;
            e.Use();
        }

        GUILayout.EndScrollView();

        Handles.EndGUI();

        // se preview è istanziata, seguire la posizione del mouse, display raggio snap e funzione di snap
        if (previewObject == null) return;

        // raggio che parte dal mouse
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        Plane roomLevel = new Plane(Vector3.up, Vector3.zero);

        if (!roomLevel.Raycast(ray, out float dist)) return;

        Vector3 position = ray.GetPoint(dist);

        previewObject.transform.position = position;

        // display raggio di snap
        if (radiusVisibility) 
            Handles.DrawWireDisc(position, Vector3.up, snapRadius);

        // rotazione della preview
        if (e.type == EventType.KeyDown && e.shift && e.keyCode == KeyCode.Q && isPlacing)
        {
            RotatePreview(90);
        }
        else if (e.type == EventType.KeyDown && e.shift && e.keyCode == KeyCode.E && isPlacing)
        {
            RotatePreview(-90);
        }

        // TODO logica di snap in funzione separata

        // refresh della scena
        sceneView.Repaint();
    }

    private void OnDisable()
    {
        // se chiudo la finestra chiudo tutto
        SceneView.duringSceneGui -= OnSceneGUI;
        ClearPreview();
    }

    private static void ScanFolders()
    {
        categories.Clear();

        if (!AssetDatabase.IsValidFolder(RootFolder)) return;

        // controlla tutte le sottocartelle del root folder assegnato
        string fullRootPath = Path.GetFullPath(RootFolder);
        string[] subDirs = Directory.GetDirectories(fullRootPath); // 1 door rooms, 2 door rooms...

        foreach (string dir in subDirs)
        {
            string folderName = Path.GetFileName(dir);

            // ricostruire il path relativo di Unity
            string assetFolderPath = RootFolder + "/" + folderName;

            // trova tutti i prefab nelle subdirectory
            string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { assetFolderPath });

            // cartella vuota skip
            if (guids.Length == 0) continue;

            // crea un nuovo categoryData per la cartella corrente
            var category = new CategoryData()
            {
                name = folderName,
                foldout = false,
                prefabPaths = new string[guids.Length],
                prefabNames = new string[guids.Length],
                prefabAssets = new GameObject[guids.Length],
            };

            // per ogni prefab nella cartella
            for (int i = 0; i < guids.Length; i++)
            {
                // conversione GUID --> PATH LEGGIBILE
                category.prefabPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

                category.prefabNames[i] = Path.GetFileNameWithoutExtension(category.prefabPaths[i]);

                // load asset path carica il riferimento in memoria (NON istanzia)
                category.prefabAssets[i] = AssetDatabase.LoadAssetAtPath<GameObject>(category.prefabPaths[i]);
            }

            // aggiunge alla lista dei prefab disponibili da GUI
            categories.Add(category);
        }

        Debug.Log($"Categories found: {categories.Count}");
    }

    private void RotatePreview(float angle)
    {
        if (previewObject == null) return;

        previewObject.transform.rotation *= Quaternion.Euler(0, angle, 0);
    }

    private void ClearPreview()
    {
        // distruggiamo la preview se esiste
        if (previewObject != null)
        {
            DestroyImmediate(previewObject);
            previewObject = null;
            lastSelectIndex = int.MinValue;
        }
    }


}
