﻿using Application.Sequence;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.Gimmick.SystemGimmick;
using UnityEngine;
using UnityEngine.Playables;
using VContainer.Unity;

namespace Presentation
{
    public class GameClearPresenter : IStartable
    {
        private readonly GameState gameState;
        private readonly SaveManager<SaveData> saveManager;
        private readonly PlayableDirector mainGateDirector;

        public GameClearPresenter(GameState gameState, SaveManager<SaveData> saveManager, DirectorTable directorTable)
        {
            this.gameState = gameState;
            this.saveManager = saveManager;

            mainGateDirector = directorTable.GetDirector("MainGateDirector");
        }

        public void Start()
        {
            var clearPoints = Object.FindObjectsByType<GameClearPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var savePoints = Object.FindObjectsByType<ClearSavePoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            if (clearPoints.Length == 0 || savePoints.Length == 0)
            {
                Debug.LogError("クリアセーブポイントが見つかりませんでした");
                return;
            }

            foreach (GameClearPoint clearPoint in clearPoints)
            {
                clearPoint.OnClear += () =>
                {
                    gameState.SetState(GameState.State.StageSelect);
                };
            }

            foreach (ClearSavePoint savePoint in savePoints)
            {
                savePoint.OnSave += stageId =>
                {
                    bool[] stageList = saveManager.Data.ClearedStageList;

                    // 既にクリア済みの場合はスキップ
                    if (stageList[stageId])
                    {
                        Debug.Log("既にクリア済みです");
                        return;
                    }

                    // クリアデータの保存
                    if (stageId < stageList.Length)
                    {
                        Debug.Log("セーブしました");
                        stageList[stageId] = true;
                        saveManager.Save().Forget();
                    }
                    else
                    {
                        Debug.LogError("ステージIDが範囲外です");
                    }

                    // チュートリアルは演出を入れない
                    if (stageId == 0)
                    {
                        return;
                    }

                    // クリア演出を再生
                    mainGateDirector.Play();
                };
            }
        }
    }
}