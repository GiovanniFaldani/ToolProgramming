using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

[Overlay(typeof(SceneView), "Room Placer")]
public class RoomPlacerGUI : IMGUIOverlay
{
    private const string RootFolder = "Assets/Prefab";
    // TODO trova come spacchio pescare una reference alla window, o fare drawing su sceneGUI dall'altro script
    // private static readonly RoomPlacer roomPlacerWindow = EditorWindow.GetWindow<RoomPlacer>();

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
            ResetSelection();
        }
    }
    public override void OnWillBeDestroyed()
    {
        SceneView.duringSceneGui -= OnScenePreview;
        displayedChanged -= OnVisibilityChanged;

        ResetSelection();
    }

    private static void ScanFolders()
    {

    }

    public override void OnGUI()
    {
        throw new NotImplementedException();
    }

    private static void OnScenePreview(SceneView sceneView)
    {

    }

    private static void ResetSelection()
    {
        // roomPlacerWindow.selectedPrefab = null;
    }

}
