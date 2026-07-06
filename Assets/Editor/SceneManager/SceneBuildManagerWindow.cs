using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class SceneBuildManagerWindow : EditorWindow
{
    // mi prendo nome, path, se la scena è da mettere in build e se è già attivata ls cena correntemente controllata
    private class SceneData
    {
        public string name;
        public string path;
        public bool inBuild;
        public bool enabled;
    }

    private readonly List<SceneData> scenes = new List<SceneData>();

    private Vector2 scroll;

    [MenuItem("Tools/Scene Build Manager")]
    public static void Open()
    {
        GetWindow<SceneBuildManagerWindow>("Scene Build Manager");
    }

    private void OnEnable()
    {
        // refresh la lista di scene presenti nel progetto e verifico quali sono incluse nei build settings
        RefreshScenes();
    }

    void RefreshScenes()
    {
        // svuoto la lista corrente
        scenes.Clear();

        // array di identificatori univoci di asset di tipo Scene
        string[] guids = AssetDatabase.FindAssets("t:Scene");

        foreach (string guid in guids)
        {
            // converto il guid nel percorso dell'asset
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SceneData data = new SceneData
            {
                // prendo il nome dle file senza estensione
                name = System.IO.Path.GetFileNameWithoutExtension(path),

                // prendo il path appena trovato e lo assegna alla variabile path del SceneData
                path = path,
            };

            // cerco se questa scena è presente nei buildSettings
            EditorBuildSettingsScene buildScene = EditorBuildSettings.scenes.FirstOrDefault(s => s.path == path);

            // se buildScene esiste la scena è inclusa nella build
            if (buildScene != null)
            {
                // memorizzio che la scena è presente nei buildSettings
                data.inBuild = true;

                // memorizzio se la scena è abilitata o meno nei build settings
                data.enabled = buildScene.enabled;
            }

            // aggiungo la scena alla lista visualizzata dalla finestra
            scenes.Add(data);
        }

        // ordino alfabeticamente le scene
        scenes.Sort((a,b) => a.name.CompareTo(b.name));
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Refresh"))
            RefreshScenes();

        if (GUILayout.Button("Enable All"))
            SetAllEnabled(true);

        if (GUILayout.Button("Disable All"))
            SetAllEnabled(false);

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // flag per capire se durante il draw della lista è stata richiesta una modifica di quella lista
        bool refreshNeeded = false;

        // scrollview
        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach(SceneData scene in scenes)
        {
            // se è a true ho modificato i buildSettings
            if (DrawSceneRow(scene))
            {
                refreshNeeded = true;
                break;
            }
        }

        EditorGUILayout.EndScrollView();

        if (refreshNeeded)
        {
            // ricostruzione della lista
            RefreshScenes();

            // costringo Unity a ridisegnare la window con i nuovi dati
            Repaint();
        }
    }

    private bool DrawSceneRow(SceneData scene)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();

        // nome della scena
        GUILayout.Label(scene.name, GUILayout.Width(180));

        // path della scena
        GUILayout.Label(scene.path);

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();

        // diabilito il click sul toggle se la scena non è presente nei buildSettings
        GUI.enabled = scene.inBuild;

        // faccio il toggle relativo all'abilitazione della scena
        bool newEnabled = GUILayout.Toggle(scene.enabled, "Enabled", GUILayout.Width(80));

        // se ho modificato l'enabled dalla window
        if(newEnabled != scene.enabled)
        {
            // aggiorno lo stato di abilitazione
            scene.enabled = newEnabled;

            // sincronizzo i buildSettings
            ApplyBuildSettings();

        }

        GUI.enabled = true;


        GUILayout.FlexibleSpace();

        if (!scene.inBuild)
        {
            if(GUILayout.Button("Add", GUILayout.Width(80)))
            {
                // aggiungo la scena ai buildSettings
                AddScene(scene);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                // comunico all'OnGui che la lista deve essere aggiornata
                return true;
            }
        }
        else
        {
            if(GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                // rimuovo la scena dai buildSettings
                RemoveScene(scene);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                // comunico all'OnGui che la lista deve essere aggiornata
                return true;
            }
        }

        // se clicco questo pulsante, mi vado a prendere il path, vado nella cartella a quel path e evidenzio la scena
        if(GUILayout.Button("Ping", GUILayout.Width(80)))
        {
            SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
            EditorGUIUtility.PingObject(asset);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        return false;
    }

    void AddScene(SceneData scene)
    {
        // prendo l'array di scene in build e ne faccio una lista mia
        List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList<EditorBuildSettingsScene>();

        // aggiungo la scena selezionata e la abilito automaticamente
        buildScenes.Add(new EditorBuildSettingsScene(scene.path, true));

        // aggiorno le scene nei buildSettings e le ritrasformo in array
        EditorBuildSettings.scenes = buildScenes.ToArray();
    }

    void RemoveScene(SceneData scene)
    {
        // prendo l'array di scene in build e ne faccio una lista mia
        List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList<EditorBuildSettingsScene>();

        // rimuovo tutte le scene che hanno quel percorso esatto
        buildScenes.RemoveAll(s => s.path == scene.path);

        // aggiorno le scene nei buildSettings (senza quelle "incriminate") e le ritrasformo in array
        EditorBuildSettings.scenes = buildScenes.ToArray();
    }

    void ApplyBuildSettings()
    {
        // prendo la lista di scene in build e ne creo una nostra
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

        // per ogni scena in lista
        foreach(SceneData scene in scenes)
        {
            // se non è da mettere in build salto l'operazione successiva
            if (!scene.inBuild)
                continue;

            // altrimenti la aggiunogo ai buildSettings
            buildScenes.Add(new EditorBuildSettingsScene(scene.path, scene.enabled));
        }

        // aggiorno le scene nei buildSettings
        EditorBuildSettings.scenes = buildScenes.ToArray();
    }

    void SetAllEnabled(bool value)
    {
        // per ogni scena in lista, se si tratta di una scena da mettere in build,
        // setto la booleana enabled in base al pulsante cliccato (enable all / disable all)
        foreach (SceneData scene in scenes)
        {
            if (scene.inBuild)
                scene.enabled = value;
        }

        // aggorno i buildSettings
        ApplyBuildSettings();
    }
}
