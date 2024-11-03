using System;
using System.Collections.Generic;
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

    public class InGameSequencer : IStartable
    {
        private readonly AdditiveSceneLoader additiveSceneLoader;
        private AssetReference levelReference;
        private GameState gameState;

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

        private void OnLoadRequested(AssetReference levelReference)
        {
            this.levelReference = levelReference;
            gameState = GameState.Playing;
        }

        private void OnUnloadRequested(AssetReference levelReference)
        {
            this.levelReference = levelReference;
            gameState = GameState.StageSelect;
        }

        public async UniTaskVoid Sequence()
        {
            Debug.Log("Waiting");
            //ステージセレクト待機
            await UniTask.WaitUntil(IsGameState(GameState.Playing));
            
            await additiveSceneLoader.Load(levelReference);
            
            //クリア待機
            await UniTask.WaitUntil(IsGameState(GameState.StageSelect));
        }

        private Func<bool> IsGameState(GameState gameState)
        {
            return () => this.gameState == gameState;
        }
    }
}