using System;
using System.Collections.Generic;
using System.Threading;
using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Application.Sequence
{
    public enum GameState
    {
        StageSelect,
        Playing
    }

    public class InGameSequencer : IStartable, IDisposable
    {
        private readonly AdditiveSceneLoader additiveSceneLoader;
        private (SceneField mainScene, List<SceneField> sceneFields) loadContext;
        private GameState gameState;
        private CancellationTokenSource cTokenSource;

        [Inject]
        public InGameSequencer()
        {
            additiveSceneLoader = new AdditiveSceneLoader();
        }

        public void Start()
        {
            var triggers = Object.FindObjectsByType<AdditiveLevelLoadTrigger>(FindObjectsSortMode.None);

            foreach (AdditiveLevelLoadTrigger trigger in triggers)
            {
                trigger.OnLoadRequested += OnLoadRequested;
                trigger.OnUnloadRequested += OnUnloadRequested;
            }

            Sequence().Forget();
        }

        private void OnLoadRequested(SceneField mainScene, List<SceneField> fields)
        {
            this.loadContext = (mainScene, fields);
            gameState = GameState.Playing;
        }

        private void OnUnloadRequested(SceneField mainScene, List<SceneField> fields)
        {
            this.loadContext = (mainScene, fields);
            gameState = GameState.StageSelect;
        }

        public async UniTaskVoid Sequence()
        {
            //ステージセレクト待機
            await UniTask.WaitUntil(IsGameState(GameState.Playing));

            cTokenSource = new CancellationTokenSource();
            await additiveSceneLoader.Load(loadContext, cTokenSource.Token);

            //クリア待機
            await UniTask.WaitUntil(IsGameState(GameState.StageSelect));
            await additiveSceneLoader.UnloadAdditiveScenes(cTokenSource.Token);
        }

        private Func<bool> IsGameState(GameState gameState)
        {
            return () => this.gameState == gameState;
        }

        public void Dispose()
        {
            cTokenSource?.Cancel();
            cTokenSource?.Dispose();
        }
    }
}