using System.Collections.Generic;
using System.Threading;
using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Application.Sequence
{
    public class AdditiveSceneLoadExecutor
    {
        private readonly AdditiveSceneLoader additiveSceneLoader = new();
        private CancellationToken cancellationToken;
        private (SceneField mainScene, List<SceneField> fields) loadContext;

        public AdditiveSceneLoadExecutor()
        {
            var triggers = Object.FindObjectsByType<AdditiveLevelLoadTrigger>(FindObjectsSortMode.None);

            foreach (AdditiveLevelLoadTrigger trigger in triggers)
            {
                trigger.OnLoadRequested += (scene, fields) => OnLoadRequested(scene, fields, trigger).Forget();
            }
        }

        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }
        
        public UniTask UnloadAdditiveScenes()
        {
            return additiveSceneLoader.UnloadAdditiveScenes(cancellationToken);
        }

        private async UniTaskVoid OnLoadRequested(SceneField mainScene, List<SceneField> fields, AdditiveLevelLoadTrigger trigger)
        {
            loadContext = (mainScene, fields);

            // レベルロード 
            await additiveSceneLoader.Load((mainScene, fields), cancellationToken);

            trigger.CallLoaded();
        }
    }
}