using System.Collections.Generic;
using System.Threading;
using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Application.Sequence
{
    /// <summary>
    /// 追加シーン読み込みを実行するクラス
    /// </summary>
    public class AdditiveSceneLoadExecutor
    {
        private readonly AdditiveSceneLoader additiveSceneLoader = new();
        private CancellationToken cancellationToken;
        private (SceneField mainScene, List<SceneField> fields) loadContext;

        public AdditiveSceneLoadExecutor()
        {
            // シーン上から追加シーン読み込みを行うトリガーを取得
            var triggers = Object.FindObjectsByType<AdditiveLevelLoadTrigger>(FindObjectsSortMode.None);

            // ロード要求イベントを登録
            foreach (AdditiveLevelLoadTrigger trigger in triggers)
            {
                trigger.OnLoadRequested += (scene, fields) => OnLoadRequested(scene, fields, trigger).Forget();
            }
        }

        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }
        
        /// <summary>
        /// 非同期で追加読み込みを行ったシーンを削除します
        /// </summary>
        public UniTask UnloadAdditiveScenes()
        {
            return additiveSceneLoader.UnloadAdditiveScenes(cancellationToken);
        }
        
        private async UniTaskVoid OnLoadRequested(SceneField mainScene, List<SceneField> fields, AdditiveLevelLoadTrigger trigger)
        {
            loadContext = (mainScene, fields);

            // 追加シーン読み込みを非同期で行う
            await additiveSceneLoader.Load((mainScene, fields), cancellationToken);

            trigger.CallLoaded();
        }
    }
}