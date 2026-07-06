using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class SceneValidatorWindow : EditorWindow
{
    private class SceneValidation
    {
        public string sceneName;
        public string scenePath;
        public bool hasMainCamera;
        public bool hasNavMeshSurface;
    }

    private List<SceneValidation> scenes = new List<SceneValidation>();
    private Vector2 scroll;

    [MenuItem("Tools/Scene Validator")]
    public static void Open()
    {
        GetWindow<SceneValidatorWindow>("Scene Validator");
    }

    private void OnEnable()
    {
        SearchScenes();
    }

    void SearchScenes()
    {
        // refresh della lista
        scenes.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Scene");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // filtro solo quelli che sono nella cartella assets
            if (!path.StartsWith("Assets/"))
                continue;

            scenes.Add(new SceneValidation()
            {
                sceneName = System.IO.Path.GetFileName(path),

                scenePath = path
            });
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Refresh"))
            SearchScenes();

        // button per validare
        if(GUILayout.Button("Validate"))
            ValidateScenes();

        // se manca la main camera in delle scene, la aggiunge
        if (GUILayout.Button("Auto Fix"))
            AutoFixScenes();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach(SceneValidation scene in scenes)
        {
            EditorGUILayout.BeginVertical("box");

            // nome scena
            EditorGUILayout.LabelField("Scene", scene.sceneName);

            //path scena
            EditorGUILayout.LabelField("Path", scene.scenePath);

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = scene.hasMainCamera ? Color.green : Color.red;

            GUIStyle style2 = new GUIStyle(EditorStyles.boldLabel);
            style2.normal.textColor = scene.hasNavMeshSurface ? Color.green : Color.red;

            EditorGUILayout.LabelField(scene.hasMainCamera ? "OK - Main Camera trovata" : "X - Main Camera mancante", style);

            EditorGUILayout.LabelField(scene.hasNavMeshSurface ? "OK - Nav Mesh Surface trovata" : "X - Nav Mesh Surface mancante", style2);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    private void ValidateScenes()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        foreach(SceneValidation scene in scenes)
        {
            // carico la scena corrente
            EditorSceneManager.OpenScene(scene.scenePath);

            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            // validazione: se main camera diversa da null allora hasMainCamera = true, altrimenti = false
            scene.hasMainCamera = mainCamera != null;

            // controllo che esista una NavMeshSurface
            NavMeshSurface navMeshSurface = FindAnyObjectByType<NavMeshSurface>();

            scene.hasNavMeshSurface = navMeshSurface != null;
        }

        if (!string.IsNullOrEmpty(currentScene))
            EditorSceneManager.OpenScene(currentScene);

        Repaint();
    }

    void AutoFixScenes()
    {
        // ci prendiamo il path
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        // controlliamo tutte le scene
        foreach(SceneValidation scene in scenes)
        {
            // le apriamo
            EditorSceneManager.OpenScene(scene.scenePath);

            // controlliamo che non esista la camera
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            // controllo anche l'esistenza della NavmeshSurface
            NavMeshSurface navMeshSurface = FindAnyObjectByType<NavMeshSurface>();

            // se non esiste Main Camera
            if (mainCamera == null)
            {
                // la creiamo
                GameObject camera = new GameObject("Main Camera");

                // le assegnamo il tag
                camera.tag = "MainCamera";

                // le aggiungiamo la componente
                camera.AddComponent<Camera>();

                // le diamo la posizione di default
                camera.transform.position = new Vector3(0, 1, -10);

                // salviamo
                EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }

            // se non esiste NavmeshSurface
            if(navMeshSurface == null)
            {
                GameObject newNavMeshSurface = new GameObject("NavMesh Surface");

                newNavMeshSurface.AddComponent<NavMeshSurface>();

                newNavMeshSurface.transform.position = Vector3.zero;

                EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        if (!string.IsNullOrEmpty(currentScene))
            EditorSceneManager.OpenScene(currentScene);

        // aggiorniamo il report che abbiamo nella window
        ValidateScenes();
    }
}
