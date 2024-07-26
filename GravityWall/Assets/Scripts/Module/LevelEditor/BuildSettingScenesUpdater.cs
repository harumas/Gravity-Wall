using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Module.LevelEditor
{
    public class BuildSettingScenesUpdater : AssetPostprocessor
    {
        private const string sceneDirLevel = @"Scenes\Level";
        private const string sceneDirTitle = @"Scenes\Title";
        private const string initialLoadScene = @"Scenes\Title\Root.unity";

        public static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!CheckExists())
                return;

            var assets = importedAssets
                .Union(deletedAssets)
                .Union(movedAssets)
                .Union(movedFromAssetPaths);

            if (CheckInSceneDir(assets))
            {
                Create();
            }
        }

        // Sceneディレクトリ以下のアセットが編集されたか
        private static bool CheckInSceneDir(IEnumerable<string> assets)
        {
            return assets.Any(asset =>
            {
                string directoryName = Path.GetDirectoryName(asset);
                return directoryName == GetAssetsPath(sceneDirLevel) || directoryName == GetAssetsPath(sceneDirTitle);
            });
        }

        // 存在チェック
        private static bool CheckExists()
        {
            string sceneDirFullPath = GetFullPath(sceneDirLevel);
            string initialLoadSceneFullPath = GetFullPath(initialLoadScene);

            if (!Directory.Exists(sceneDirFullPath))
            {
                Debug.LogError("Not Found Dir :" + sceneDirFullPath);
                return false;
            }

            if (!File.Exists(initialLoadSceneFullPath))
            {
                Debug.LogError("Not Found Inital Load Scene : " + initialLoadSceneFullPath);
                return false;
            }

            return true;
        }

        private static void Create()
        {
            string initialLoadSceneAssetsPath = GetAssetsPath(initialLoadScene);

            var scenes = AssetDatabase.FindAssets("t:Scene", new string[] { GetAssetsPath(sceneDirLevel), GetAssetsPath(sceneDirTitle) })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .OrderBy(path => path)
                .Where(path => path != initialLoadSceneAssetsPath)
                .Select(path => new EditorBuildSettingsScene(path, true))
                .ToList();

            // 初回に呼び込まれて欲しいシーンを先頭に配置する
            scenes.Insert(0, new EditorBuildSettingsScene(initialLoadSceneAssetsPath, true));

            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();

            Debug.Log("Created BuildSettings.");
        }

        private static string GetFullPath(string path)
        {
            return Application.dataPath + "\\" + path;
        }

        private static string GetAssetsPath(string path)
        {
            return "Assets\\" + path;
        }
    }
}