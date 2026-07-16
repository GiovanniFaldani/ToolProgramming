//using System;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEditor.Overlays;
//using UnityEngine;

//[Overlay(typeof(SceneView), "Room Placer")]
//public class RoomPlacerGUI : IMGUIOverlay
//{
//    // classe per memorizzare i dati di una cartella di prefab
//    private class CategoryData
//    {
//        public string name;
//        public bool foldout; // stato del foldout: aperto/chiuso
//        public string[] prefabPaths; // path relativi a Unity
//        public string[] prefabNames;
//        public GameObject[] prefabAssets; // riferimenti ai prefab in memoria
//    }

//    private Vector2 scrollPos;

//    private static readonly List<CategoryData> categories = new List<CategoryData>();
//    private const string RootFolder = "Assets/Prefab/Rooms";

//    private int columns = 3;
//    private float thumbsize = 64;

//    private static GameObject selectedPrefab;
//    private static string selectedName;
//    private static Mesh previewMesh;
//    private static Material[] previewMaterials;
//    private static Vector3 previewPosition;
//    private static Vector3 previewScale;

//    public override void OnCreated()
//    {
//        ScanFolders();

//        SceneView.duringSceneGui -= OnScenePreview;
//        SceneView.duringSceneGui += OnScenePreview;

//        displayedChanged -= OnVisibilityChanged;
//        displayedChanged += OnVisibilityChanged;
//    }

//    private void OnVisibilityChanged(bool obj)
//    {
//        if (!obj)
//        {
//            ResetSelection();
//        }
//    }
//    public override void OnWillBeDestroyed()
//    {
//        SceneView.duringSceneGui -= OnScenePreview;
//        displayedChanged -= OnVisibilityChanged;

//        ResetSelection();
//    }

//    private static void ScanFolders()
//    {
//        categories.Clear();

//        if (!AssetDatabase.IsValidFolder(RootFolder)) return;

//        // controlla tutte le sottocartelle del root folder assegnato
//        string fullRootPath = Path.GetFullPath(RootFolder);
//        string[] subDirs = Directory.GetDirectories(fullRootPath); // 1 door rooms, 2 door rooms...

//        foreach (string dir in subDirs)
//        {
//            string folderName = Path.GetFileName(dir);

//            // ricostruire il path relativo di Unity
//            string assetFolderPath = RootFolder + "/" + folderName;

//            // trova tutti i prefab nelle subdirectory
//            string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { assetFolderPath });

//            // cartella vuota skip
//            if (guids.Length == 0) continue;

//            // crea un nuovo categoryData per la cartella corrente
//            var category = new CategoryData()
//            {
//                name = folderName,
//                foldout = false,
//                prefabPaths = new string[guids.Length],
//                prefabNames = new string[guids.Length],
//                prefabAssets = new GameObject[guids.Length],
//            };

//            // per ogni prefab nella cartella
//            for (int i = 0; i < guids.Length; i++)
//            {
//                // conversione GUID --> PATH LEGGIBILE
//                category.prefabPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

//                category.prefabNames[i] = Path.GetFileNameWithoutExtension(category.prefabPaths[i]);

//                // load asset path carica il riferimento in memoria (NON istanzia)
//                category.prefabAssets[i] = AssetDatabase.LoadAssetAtPath<GameObject>(category.prefabPaths[i]);
//            }

//            // aggiunge alla lista dei prefab disponibili da GUI
//            categories.Add(category);
//        }
//    }

//    public override void OnGUI()
//    {
//        if (GUILayout.Button("Refresh")) ScanFolders();

//        if (categories.Count <= 0)
//        {
//            EditorGUILayout.HelpBox("No prefab found in the specified folder", MessageType.Info);
//            return;
//        }

//        // scrollPos tiene traccia della posizione di scroll tra un frame e l'altro
//        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(300));

//        foreach (CategoryData cat in categories)
//        {
//            // crea foldout della categoria di stanza corrispondente
//            cat.foldout = EditorGUILayout.Foldout(cat.foldout, $"{cat.name} ({cat.prefabPaths.Length})", true);

//            if (cat.foldout)
//            {
//                // conta il numero di righe da mettere nel foldout
//                int itemCount = cat.prefabPaths.Length;

//                int rows = Mathf.CeilToInt((float)itemCount / columns);

//                // display della categoria in righe
//                for (int row = 0; row < rows; row++)
//                {
//                    using (new GUILayout.HorizontalScope())
//                    {
//                        for (int col = 0; col < columns; col++)
//                        {
//                            int index = row * columns + col;

//                            if (index >= itemCount)
//                            {
//                                // cella vuota
//                                GUILayout.Label("", GUILayout.Width(thumbsize), GUILayout.Height(thumbsize));
//                                continue;
//                            }

//                            GameObject prefab = cat.prefabAssets[index];

//                            // GetAssetPreview è asincrono
//                            Texture2D preview = AssetPreview.GetAssetPreview(prefab);

//                            // GUI CONTENT combina immagine + tooltip in un unico oggetto
//                            GUIContent content;

//                            if (preview != null)
//                            {
//                                content = new GUIContent(preview, cat.prefabNames[index]);
//                            }
//                            else
//                            {
//                                content = new GUIContent(cat.prefabNames[index]);
//                            }

//                            if (GUILayout.Button(content, GUILayout.Width(thumbsize), GUILayout.Height(thumbsize)))
//                            {
//                                Debug.Log($"Cliccato su: {cat.prefabNames[index]}");
//                                //RoomPlacer.Instance.selectedPrefab = prefab;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        GUILayout.EndScrollView();
//    }

//    private static void OnScenePreview(SceneView sceneView)
//    {

//    }


//    private static void ResetSelection()
//    {
//        // resetta alla prima stanza con una porta
//        // RoomPlacer.Instance.selectedPrefab = categories[0].prefabAssets[0];
//    }

//}
