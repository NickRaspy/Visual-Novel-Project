using UnityEngine;
using UnityEditor;
using VNP.Scriptables;
using VNP.MiniGames;
using System.Collections.Generic;
using System.IO;

public class MiniGameEditorWindow : EditorWindow
{
    private const string ResourceAssetName = "MiniGameList";
    private const string ResourceFolder = "Resources";
    private const string ResourceAssetPath = "Assets/Resources/MiniGameList.asset";
    private static readonly Vector2 MinWindowSize = new(400, 300);

    private MiniGameList miniGameList;
    private Vector2 scrollPos;

    private List<bool> miniGameFoldouts = new();

    [MenuItem("VNP/Mini Games Editor")]
    public static void ShowWindow()
    {
        GetWindow<MiniGameEditorWindow>("Mini Games Editor");
    }

    private void OnEnable()
    {
        minSize = MinWindowSize;
        LoadMiniGameListFromResources();
        SyncFoldouts();
    }

    private void LoadMiniGameListFromResources()
    {
        miniGameList = Resources.Load<MiniGameList>(ResourceAssetName);
        SyncFoldouts();
    }

    private void SyncFoldouts()
    {
        if (miniGameList == null || miniGameList.miniGamesList == null)
        {
            miniGameFoldouts = new();
            return;
        }
        int count = miniGameList.miniGamesList.Count;
        while (miniGameFoldouts.Count < count)
            miniGameFoldouts.Add(false);
        while (miniGameFoldouts.Count > count)
            miniGameFoldouts.RemoveAt(miniGameFoldouts.Count - 1);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mini Games Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (miniGameList == null)
        {
            EditorGUILayout.HelpBox($"No MiniGameList asset found in Resources folder.\n" +
                $"Asset must be located at: Assets/{ResourceFolder}/{ResourceAssetName}.asset", MessageType.Warning);

            if (GUILayout.Button("Create MiniGameList in Resources", GUILayout.Height(30)))
            {
                CreateMiniGameListAssetInResources();
                LoadMiniGameListFromResources();
            }
            GUI.enabled = false;
            DrawMiniGameListEditor();
            GUI.enabled = true;
        }
        else
        {
            DrawMiniGameListEditor();
        }
    }

    private void CreateMiniGameListAssetInResources()
    {
        string resourcesPath = Path.Combine("Assets", ResourceFolder);
        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder("Assets", ResourceFolder);
        }

        if (File.Exists(ResourceAssetPath))
        {
            EditorUtility.DisplayDialog("Asset Exists", "MiniGameList.asset already exists in Resources.", "OK");
            return;
        }

        var asset = ScriptableObject.CreateInstance<MiniGameList>();
        asset.miniGamesList = new List<SerializedMiniGame>();
        AssetDatabase.CreateAsset(asset, ResourceAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        miniGameList = asset;
        SyncFoldouts();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private void DrawMiniGameListEditor()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (miniGameList != null && GUILayout.Button("Ping Asset", GUILayout.Width(90)))
        {
            var asset = AssetDatabase.LoadAssetAtPath<MiniGameList>(ResourceAssetPath);
            if (asset != null)
                EditorGUIUtility.PingObject(asset);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (miniGameList == null || miniGameList.miniGamesList == null)
        {
            EditorGUILayout.HelpBox("MiniGameList asset not loaded.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        var games = miniGameList.miniGamesList;
        int removeIdx = -1;

        for (int i = 0; i < games.Count; i++)
        {
            var game = games[i];
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            string label = !string.IsNullOrEmpty(game.id) ? $"[{game.id}]" : $"Mini Game {i + 1}";
            miniGameFoldouts[i] = EditorGUILayout.Foldout(miniGameFoldouts[i], label, true, EditorStyles.foldoutHeader);

            GUILayout.FlexibleSpace();
            DrawRemoveButton(() =>
            {
                if (EditorUtility.DisplayDialog("Remove mini game?", $"Are you sure you want to remove mini game '{game.id}'?", "Remove", "Cancel"))
                    removeIdx = i;
            });
            EditorGUILayout.EndHorizontal();

            if (miniGameFoldouts[i])
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Mini Game Info", EditorStyles.miniBoldLabel);

                game.id = EditorGUILayout.TextField(new GUIContent("ID", "Unique identifier"), game.id);

                // Поле для выбора мини-игры (drag&drop или выбор из Project)
                game.miniGame = (MiniGame)EditorGUILayout.ObjectField(
                    new GUIContent("Mini Game", "Reference to MiniGame prefab or asset"),
                    game.miniGame,
                    typeof(MiniGame),
                    false);

                // Если есть другие поля у SerializedMiniGame, добавьте их здесь:
                // game.difficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty", game.difficulty);
            }
            EditorGUILayout.EndVertical();
        }
        if (removeIdx >= 0)
            games.RemoveAt(removeIdx);

        EditorGUILayout.Space();
        if (GUILayout.Button("Add Mini Game", GUILayout.Height(30)))
            games.Add(new SerializedMiniGame() { id = "" });

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            SyncFoldouts();
            EditorUtility.SetDirty(miniGameList);
            AssetDatabase.SaveAssets();
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