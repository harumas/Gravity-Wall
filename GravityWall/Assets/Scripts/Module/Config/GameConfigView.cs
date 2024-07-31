﻿using System;
using Module.Core.Save;
using TriInspector;
using UnityEngine;

namespace Module.Config
{
    /// <summary>
    /// ゲーム設定をインスペクタ上で行うクラス
    /// </summary>
    public class GameConfigView : MonoBehaviour
    {
        [SerializeField] private ConfigData configData;

        private void Awake()
        {
            configData = SaveManager<ConfigData>.Instance;
        }

        [Button(ButtonSizes.Large)]
        private async void Save()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("再生中のみ変更できます");
                return;
            }

            await SaveManager<ConfigData>.Save();
            Debug.Log("ゲームコンフィグを保存しました。");
        }
    }
}