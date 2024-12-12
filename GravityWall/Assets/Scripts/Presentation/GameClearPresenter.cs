using Application.Sequence;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.Gimmick.SystemGimmick;
using UnityEngine;
using VContainer.Unity;

namespace Presentation
{
    public class GameClearPresenter : IStartable
    {
        private readonly GameState gameState;
        private readonly SaveManager<SaveData> saveManager;

        public GameClearPresenter(GameState gameState, SaveManager<SaveData> saveManager)
        {
            this.gameState = gameState;
            this.saveManager = saveManager;
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
                clearPoint.OnClear += () => gameState.SetState(GameState.State.StageSelect);
            }

            foreach (ClearSavePoint savePoint in savePoints)
            {
                savePoint.OnSave += stageId =>
                {
                    bool[] stageList = saveManager.Data.ClearedStageList;

                    if (stageId < stageList.Length)
                    {
                        stageList[stageId] = true;
                        saveManager.Save().Forget();
                    }
                    else
                    {
                        Debug.LogError("ステージIDが範囲外です");
                    }
                };
            }
        }
    }
}