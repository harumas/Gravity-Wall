using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SelectScenes : MonoBehaviour
{
    [SerializeField] private GameObject selectStageButton;
    private string scenesPath = "";
    private List<string> sceneNames = new List<string>();

    void Start()
    {
        // 指定したフォルダの全てのファイルを取得してListに追加
        if (Directory.Exists(scenesPath))
        {
            string[] files = Directory.GetFiles(scenesPath);
            foreach (string file in files)
            {
                sceneNames.Add(Path.GetFileName(file));
            }
        }
        else
        {
            Debug.LogError("フォルダが存在しません: " + scenesPath);
        }

        for (int i = 0; i < sceneNames.Count; i++)
        {

        }
    }

    public void LoadScene()
    {

    }
}