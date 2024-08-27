using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

namespace Module.PlayTest
{
    public class SelectScenes : MonoBehaviour
    {
        [SerializeField] private GameObject selectStageButton;
        [SerializeField] private float ButtonInterval;
        [SerializeField] private GameObject ButtonParent;
        private string scenesPath = @"Assets/Scenes/Level";
        private List<string> sceneNames = new List<string>();
        private int index = 0;
        void Start()
        {
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

            for (int i = 0; i < sceneNames.Count; i++)
            {
                Debug.Log(sceneNames[i]);
                GameObject button = Instantiate(selectStageButton, ButtonParent.transform);
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sceneNames[i];
                button.transform.localPosition = new Vector3(0, -i * ButtonInterval, 0);
            }
        }

        public void UpSelect()
        {
            if (index <= 0)
                return;

            index--;

            ButtonParent.transform.localPosition = new Vector3(0, index * ButtonInterval, 0);
        }

        public void DownSelect()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (index >= sceneNames.Count - 1)
                    return;

                index++;

                ButtonParent.transform.localPosition = new Vector3(0, index * ButtonInterval, 0);
            }
        }
     
        public void Decision()
        {
            SceneManager.LoadScene(sceneNames[index]);
        }
    }
}
