using UnityEngine;
using UnityEditor;

public class DeleteEnemyWindow: EditorWindow
{
    [MenuItem("Tools/Delete Enemy Window")]
    public static void ShowWindow()
    {
        GetWindow<DeleteEnemyWindow>("Delete Enemy Tool");
    }

    private EnemyType enemyType;

    private void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.white;
        titleStyle.fontStyle = FontStyle.BoldAndItalic;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 20;

        EditorGUILayout.Space(5);

        GUILayout.Label("Delete enemies", titleStyle);

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");

        GUI.color = Color.red;
        if (GUILayout.Button("Delete All Enemies"))
        {
            DeleteAllEnemies();
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.green;
        if (GUILayout.Button("Delete Enemies of type:"))
        {
           DeleteEnemiesOfType(enemyType);
        }

        EditorGUILayout.Space(5);

        GUI.color = Color.white;

        enemyType = (EnemyType)EditorGUILayout.EnumPopup(enemyType);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

    }

    void DeleteAllEnemies()
    {
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Delete All Enemies");


        Enemy[] enemies = FindObjectsByType<Enemy>(sortMode: FindObjectsSortMode.None);
        foreach (Enemy en in enemies)
        {
            Undo.DestroyObjectImmediate(en.gameObject);
            //DestroyImmediate(en.gameObject);
        }

        Undo.CollapseUndoOperations(group);
    }

    void DeleteEnemiesOfType(EnemyType type)
    {
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName($"Delete Enemies: {type}");

        Enemy[] enemies = FindObjectsByType<Enemy>(sortMode: FindObjectsSortMode.None);
        foreach (Enemy en in enemies)
        {
            if (en.type == type)
            {
                Undo.DestroyObjectImmediate(en.gameObject);
                //DestroyImmediate(en.gameObject);
            }
        }

        Undo.CollapseUndoOperations(group);
    }
}
