using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

[Overlay(typeof(SceneView), "Prefab Spawner")]
public class PrefabSpawner : IMGUIOverlay
{
    private class CategoryData
    {
        public string name;
        public bool foldout; // stato del foldout: aperto/chiuso
        public string[] prefabPaths; // path relativi a Unity
        public string[] prefabNames;
        public GameObject[] prefabAssets; // riferimenti ai prefab in memoria
    }

    private Vector2 scrollPos;

    private static readonly List<CategoryData> categories = new List<CategoryData>();
    private const string RootFolder = "Assets/PrefabSpawner";

    private static GameObject selectedPrefab;
    private static string selectedName;
    private static Mesh previewMesh;
    private static Material[] previewMaterials;
    private static Vector3 previewPosition;
    private static Vector3 previewScale;

    // uguale all'OnEnable
    public override void OnCreated()
    {
        ScanFolders();

        SceneView.duringSceneGui -= OnScenePreview;
        SceneView.duringSceneGui += OnScenePreview;

        displayedChanged -= OnVisibilityChanged;
        displayedChanged += OnVisibilityChanged;
    }

    private void OnVisibilityChanged(bool obj)
    {
        if (!obj)
        {
            ClearSelection();
        }
    }

    // quando chiudo la scena (?)
    public override void OnWillBeDestroyed()
    {
        SceneView.duringSceneGui -= OnScenePreview;
        displayedChanged -= OnVisibilityChanged;

        ClearSelection();
    }

    private static void ScanFolders()
    {
        categories.Clear();

        ClearSelection();

        if (!AssetDatabase.IsValidFolder(RootFolder)) return;

        string fullRootPath = Path.GetFullPath(RootFolder);
        string[] subDirs = Directory.GetDirectories(fullRootPath); // Cubes, Cylinders, Spheres

        foreach (string dir in subDirs)
        {
            string folderName = Path.GetFileName(dir);

            // ricostruire il path relativo di Unity
            string assetFolderPath = RootFolder + "/" + folderName;

            string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { assetFolderPath });

            // cartella vuota skip
            if (guids.Length == 0) continue;

            var category = new CategoryData()
            {
                name = folderName,
                foldout = false,
                prefabPaths = new string[guids.Length],
                prefabNames = new string[guids.Length],
                prefabAssets = new GameObject[guids.Length],
            };

            for (int i = 0; i < guids.Length; i++)
            {
                // conversione GUID --> PATH LEGGIBILE
                category.prefabPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

                category.prefabNames[i] = Path.GetFileNameWithoutExtension(category.prefabPaths[i]);

                // load asset path carica il riferimento in memoria (NON istanzia)
                category.prefabAssets[i] = AssetDatabase.LoadAssetAtPath<GameObject>(category.prefabPaths[i]);
            }

            categories.Add(category);
        }
    }

    public override void OnGUI()
    {
        if (GUILayout.Button("Refresh")) ScanFolders();

        if (categories.Count <= 0)
        {
            EditorGUILayout.HelpBox("No prefab found in the specified folder", MessageType.Info);
            return;
        }
        else
        {
            // scrollPos tiene traccia della posizione di scroll tra un frame e l'altro
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(300));

            foreach (CategoryData cat in categories)
            {
                cat.foldout = EditorGUILayout.Foldout(cat.foldout, $"{cat.name} ({cat.prefabPaths.Length})", true);

                if (cat.foldout)
                {
                    int columns = 3;
                    float thumbsize = 64;
                    int itemCount = cat.prefabPaths.Length;

                    int rows = Mathf.CeilToInt((float)itemCount / columns);

                    for (int row = 0; row < rows; row++)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            for (int col = 0; col < columns; col++)
                            {
                                int index = row * columns + col;
                                if (index >= itemCount)
                                {
                                    // cella vuota
                                    GUILayout.Label("", GUILayout.Width(thumbsize), GUILayout.Height(thumbsize));
                                    continue;
                                }

                                GameObject prefab = cat.prefabAssets[index];

                                // GetAssetPreview è asincrono
                                Texture2D preview = AssetPreview.GetAssetPreview(prefab);

                                // GUI CONTENT combina immagine + tooltip in un unico oggetto
                                GUIContent content;

                                if (preview != null)
                                {
                                    content = new GUIContent(preview, cat.prefabNames[index]);
                                }
                                else
                                {
                                    content = new GUIContent(cat.prefabNames[index]);
                                }

                                if (GUILayout.Button(content, GUILayout.Width(thumbsize), GUILayout.Height(thumbsize)))
                                {
                                    Debug.Log($"Cliccato su: {cat.prefabNames[index]}");
                                    SelectPrefab(prefab, cat.prefabNames[index]);
                                }
                            }
                        } // fine Horizontal Scope
                    }
                }
            }

            GUILayout.EndScrollView();
        }
    }

    private static void OnScenePreview(SceneView sceneView)
    {
        if (selectedPrefab == null || previewMesh == null) return;

        // dice alla sceneview di non gestire i click.
        // Senza questo gli oggetti in scena sarebbero cliccabili (tipo lo "e.Use()")
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            previewPosition = ray.GetPoint(distance);

            Matrix4x4 matrix = Matrix4x4.TRS(previewPosition, Quaternion.identity, previewScale);

            for (int i = 0; i < previewMaterials.Length; i++)
            {
                Graphics.DrawMesh(previewMesh, matrix, previewMaterials[i], 0, sceneView.camera, i);
            }
        }

        // ESC per annullare
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            ClearSelection();
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            GameObject spawned = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;

            spawned.transform.position = previewPosition;

            Undo.RegisterCreatedObjectUndo(spawned, "Spawned Gameobject");

            e.Use();
        }

        sceneView.Repaint();
    }

    private static void SelectPrefab(GameObject prefab, string name)
    {
        selectedPrefab = prefab;
        selectedName = name;

        var meshFilter = prefab.GetComponentInChildren<MeshFilter>();
        var meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();

        if (meshFilter != null && meshRenderer != null)
        {
            previewMesh = meshFilter.sharedMesh;
            previewMaterials = meshRenderer.sharedMaterials;
            previewScale = prefab.transform.localScale;
        }
        else
        {
            Debug.LogWarning($"meshFilter == null || meshRenderer == null");
            ClearSelection();
        }
    }

    private static void ClearSelection()
    {
        if (selectedPrefab == null || previewMesh == null) return;

        selectedPrefab = null;
        selectedName = null;
        previewMesh = null;
        previewMaterials = null;
    }
}