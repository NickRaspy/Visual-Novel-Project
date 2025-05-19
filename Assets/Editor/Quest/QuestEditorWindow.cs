using UnityEditor;
using UnityEngine;
using VNP.Scriptables;
using VNP.Data;
using System.Collections.Generic;

public class QuestEditorWindow : EditorWindow
{
    private const string QuestListResourcePath = "QuestList";
    private const string QuestListAssetPath = "Assets/Resources/QuestList.asset";

    private QuestList questList;
    private Vector2 scrollPos;

    private List<bool> questFoldouts = new();
    private List<List<bool>> stepFoldouts = new();

    private static readonly Vector2 MinWindowSize = new(400, 300);

    [MenuItem("VNP/Quest Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<QuestEditorWindow>("Quest Editor");
    }

    private void OnEnable()
    {
        minSize = MinWindowSize;
        LoadQuestList();
        SyncFoldouts();
    }

    private void LoadQuestList()
    {
        questList = Resources.Load<QuestList>(QuestListResourcePath);
        SyncFoldouts();
    }

    private void SyncFoldouts()
    {
        if (questList == null || questList.Quests == null)
        {
            questFoldouts = new();
            stepFoldouts = new();
            return;
        }
        int questCount = questList.Quests.Count;
        while (questFoldouts.Count < questCount)
            questFoldouts.Add(false);
        while (questFoldouts.Count > questCount)
            questFoldouts.RemoveAt(questFoldouts.Count - 1);

        while (stepFoldouts.Count < questCount)
            stepFoldouts.Add(new());
        while (stepFoldouts.Count > questCount)
            stepFoldouts.RemoveAt(stepFoldouts.Count - 1);

        for (int i = 0; i < questCount; i++)
        {
            var steps = questList.Quests[i].progress ?? new();
            if (stepFoldouts[i] == null)
                stepFoldouts[i] = new();
            while (stepFoldouts[i].Count < steps.Count)
                stepFoldouts[i].Add(false);
            while (stepFoldouts[i].Count > steps.Count)
                stepFoldouts[i].RemoveAt(stepFoldouts[i].Count - 1);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        if (questList == null)
        {
            EditorGUILayout.HelpBox("QuestList ScriptableObject not found in Resources.", MessageType.Warning);
            if (GUILayout.Button("Create QuestList"))
            {
                CreateQuestListAsset();
                LoadQuestList();
            }
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
            DrawQuestListEditor();
        }

        GUI.enabled = true;
    }

    private void CreateQuestListAsset()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        var asset = ScriptableObject.CreateInstance<QuestList>();
        AssetDatabase.CreateAsset(asset, QuestListAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private void DrawQuestListEditor()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Quest List", EditorStyles.boldLabel);
        if (GUILayout.Button("Ping Asset", GUILayout.Width(90)))
        {
            var asset = AssetDatabase.LoadAssetAtPath<QuestList>(QuestListAssetPath);
            if (asset != null)
                EditorGUIUtility.PingObject(asset);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        var quests = questList.Quests;
        int removeQuestIdx = -1;

        for (int i = 0; i < quests.Count; i++)
        {
            var quest = quests[i];
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            questFoldouts[i] = EditorGUILayout.Foldout(questFoldouts[i], $"[{quest.id}] {quest.title}", true, EditorStyles.foldoutHeader);

            GUILayout.FlexibleSpace();
            DrawRemoveButton(() =>
            {
                if (EditorUtility.DisplayDialog("Remove quest?", $"Are you sure you want to remove quest '{quest.title}'?", "Remove", "Cancel"))
                    removeQuestIdx = i;
            });
            EditorGUILayout.EndHorizontal();

            if (questFoldouts[i])
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Quest Info", EditorStyles.miniBoldLabel);

                quest.title = EditorGUILayout.TextField(new GUIContent("Title", "Quest title"), quest.title);
                quest.id = EditorGUILayout.TextField(new GUIContent("ID", "Unique identifier"), quest.id);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Steps", EditorStyles.boldLabel);

                if (quest.progress == null)
                    quest.progress = new();

                int removeStepIdx = -1;
                for (int s = 0; s < quest.progress.Count; s++)
                {
                    var step = quest.progress[s];
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();
                    stepFoldouts[i][s] = EditorGUILayout.Foldout(stepFoldouts[i][s], $"Step {s + 1}", true);

                    GUILayout.FlexibleSpace();
                    DrawRemoveButton(() =>
                    {
                        if (EditorUtility.DisplayDialog("Remove step?", $"Remove step {s + 1}?", "Remove", "Cancel"))
                            removeStepIdx = s;
                    });
                    EditorGUILayout.EndHorizontal();

                    if (stepFoldouts[i][s])
                    {
                        EditorGUILayout.LabelField("Tasks", EditorStyles.miniBoldLabel);

                        if (step.tasks == null)
                            step.tasks = new();

                        int removeTaskIdx = -1;
                        for (int t = 0; t < step.tasks.Count; t++)
                        {
                            var task = step.tasks[t];
                            task.title = EditorGUILayout.TextField(new GUIContent("Title", "Task title"), task.title);
                            task.id = EditorGUILayout.TextField(new GUIContent("ID", "Unique task identifier"), task.id);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            DrawRemoveButton(() =>
                            {
                                if (EditorUtility.DisplayDialog("Remove task?", $"Remove task '{task.title}'?", "Remove", "Cancel"))
                                    removeTaskIdx = t;
                            });
                            EditorGUILayout.EndHorizontal();
                        }
                        if (removeTaskIdx >= 0)
                            step.tasks.RemoveAt(removeTaskIdx);

                        if (GUILayout.Button("Add Task"))
                            step.tasks.Add(new());
                    }
                    EditorGUILayout.EndVertical();
                }
                if (removeStepIdx >= 0)
                    quest.progress.RemoveAt(removeStepIdx);

                if (GUILayout.Button("Add Step"))
                    quest.progress.Add(new() { tasks = new() { new Task() } });
            }
            EditorGUILayout.EndVertical();
        }
        if (removeQuestIdx >= 0)
            quests.RemoveAt(removeQuestIdx);

        EditorGUILayout.Space();
        if (GUILayout.Button("Add Quest", GUILayout.Height(30)))
            quests.Add(new() { id = "", title = "New Quest", progress = new() });

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            SyncFoldouts();
            EditorUtility.SetDirty(questList);
        }
    }

    private void DrawRemoveButton(System.Action onClick)
    {
        Color prev = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("X", GUILayout.Width(24), GUILayout.Height(18)))
        {
            onClick?.Invoke();
        }
        GUI.backgroundColor = prev;
    }
}