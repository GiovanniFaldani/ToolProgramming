using UnityEngine;
using UnityEditor;

public class EnemyWindow : EditorWindow
{
    [MenuItem("Tools/Enemy Window")]
    public static void ShowWindow()
    {
        GetWindow<EnemyWindow>("Enemy Tool");
    }

    private string enemyName;
    private EnemyType enemyType;
    private Color enemyColor;
    private GameObject previewPrefab;
    private Mesh enemyMesh;
    private Material enemyMaterial;
    private bool canFly;
    private int flySpeed;
    private bool canSwim;
    private int swimSpeed;
    private bool canDig;
    private int digDepth;

    private void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.yellow;
        titleStyle.fontStyle = FontStyle.BoldAndItalic;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 20;

        GUIStyle centeredStyle = new GUIStyle();
        centeredStyle.normal.textColor = Color.white;
        centeredStyle.fontSize = 12;
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Enemy To Spawn", titleStyle);

        EditorGUILayout.Space(10);

        enemyName = EditorGUILayout.TextField("Name", enemyName);

        EditorGUILayout.Space(10);

        enemyMesh = (Mesh)EditorGUILayout.ObjectField("Shape", enemyMesh, typeof(Mesh), false);

        EditorGUILayout.Space(10);

        enemyMaterial = (Material)EditorGUILayout.ObjectField("Material", enemyMaterial, typeof(Material), false);

        EditorGUILayout.Space(10);

        enemyType = (EnemyType)EditorGUILayout.EnumPopup("Type", enemyType);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Label("Type Specific", titleStyle);

        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.lightCoral;

        enemyColor = EditorGUILayout.ColorField("Color", enemyColor);

        EditorGUILayout.Space(5);

        switch (enemyType)
        {
            case EnemyType.Normal:
                canFly = EditorGUILayout.Toggle("Can Fly", canFly);

                EditorGUILayout.Space(5);

                if (canFly)
                {
                    flySpeed = EditorGUILayout.IntSlider("Fly Speed", flySpeed, 0, 100);
                    EditorGUILayout.HelpBox("Flying enemies need minimum 0 speed", MessageType.Info);
                }
                break;

            case EnemyType.Elite:
                canSwim = EditorGUILayout.Toggle("Can Swim", canSwim);

                EditorGUILayout.Space(5);

                if (canSwim)
                {
                    swimSpeed = EditorGUILayout.IntSlider("Swim Speed", swimSpeed, 0, 100);
                    EditorGUILayout.HelpBox("Swimming enemies need minimum 0 speed", MessageType.Info);
                }
                break;

            case EnemyType.Boss:
                canDig = EditorGUILayout.Toggle("Can Dig", canDig);

                EditorGUILayout.Space(5);

                if (canDig)
                {
                    digDepth = EditorGUILayout.IntSlider("Dig Depth", digDepth, 0, 100);
                    EditorGUILayout.HelpBox("Digging enemies need minimum 0 depth", MessageType.Info);
                }
                break;
        }

        EditorGUILayout.Space(10);

        previewPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", previewPrefab, typeof(GameObject), false);

        EditorGUILayout.Space(10);

        if (previewPrefab != null)
        {
            Texture2D previewTexture= AssetPreview.GetAssetPreview(previewPrefab);

            if (previewPrefab != null)
            {
                GUILayout.Label("Prefab Preview", centeredStyle);

                EditorGUILayout.Space(5);

                Rect rect = GUILayoutUtility.GetRect(128, 128, GUILayout.ExpandWidth(true));

                GUI.DrawTexture(rect, previewTexture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUILayout.Label("Generating Preview...");
            }
        }

        GUI.backgroundColor = Color.white; 

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create"))
        {
            CreateEnemy();
        }

        if(GUILayout.Button("Find Enemies"))
        {
            Enemy[] enemies = FindObjectsByType<Enemy>(sortMode: FindObjectsSortMode.None);

            Debug.Log("Found " + enemies.Length + " enemies");
        }

        EditorGUILayout.EndHorizontal();

    }

    void CreateEnemy()
    {
        GameObject enemy = new GameObject(enemyName);
        Enemy en = enemy.AddComponent<Enemy>();
        MeshFilter filter = enemy.AddComponent<MeshFilter>();
        filter.sharedMesh = enemyMesh;
        MeshRenderer renderer = enemy.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = enemyMaterial; 

        en.enemyName = enemyName;
        en.type = enemyType;
        switch (enemyType)
        {
            case EnemyType.Normal:
                en.canFly = canFly;
                en.flySpeed = flySpeed;
                en.debugColor = enemyColor;
                en.normalPrefab = previewPrefab;
                break;
            case EnemyType.Elite:
                en.canSwim = canSwim;
                en.swimSpeed = swimSpeed;
                en.eliteDebugColor = enemyColor;
                en.elitePrefab = previewPrefab;
                break;
            case EnemyType.Boss:
                en.canDig = canDig;
                en.digDepth = digDepth;
                en.bossDebugColor = enemyColor;
                en.bossPrefab = previewPrefab;
                break;
        }
        enemy.transform.position = Vector3.zero;
        Selection.activeGameObject = enemy;
    }
}

// creare funzioni UI per cambiare colore, tipo (boss, normal, elite) e variabile specifica del tipo
// personalizzazione estetica
