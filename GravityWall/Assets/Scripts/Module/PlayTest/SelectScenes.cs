using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module.PlayTest
{
    public class SelectScenes : MonoBehaviour
    {
        [SerializeField] private GameObject selectStageButton;
        [SerializeField] private GameObject buttonParent;
        private string scenesPath = @"Assets/Scenes/Level";

        void Start()
        {
            List<string> sceneNames = GetSceneNames();

            // シーン名を元にボタンを生成
            foreach (var stageName in sceneNames)
            {
                GameObject button = Instantiate(selectStageButton, buttonParent.transform);
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stageName;

                var name1 = stageName;
                button.GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene(name1); });
            }

            EventSystem.current.firstSelectedGameObject = buttonParent.transform.GetChild(0).gameObject;
        }

        private List<string> GetSceneNames()
        {
            List<string> sceneNames = new List<string>();

            // 指定したフォルダの全てのファイルを取得してListに追加
            if (Directory.Exists(scenesPath))
            {
                string[] files = Directory.GetFiles(scenesPath);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == ".unity" && Path.GetFileNameWithoutExtension(file) != "")
                        sceneNames.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            else
            {
                Debug.LogError("フォルダが存在しません: " + scenesPath);
            }

            return sceneNames;
        }
    }
}