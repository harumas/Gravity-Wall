using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
        private string levelName;
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
        }

        private void OnLoadRequested(string sceneName)
        {
            levelName = sceneName;
            gameState = GameState.Playing;
        }

        private void OnUnloadRequested(string sceneName)
        {
            levelName = sceneName;
            gameState = GameState.StageSelect;
        }

        public async UniTask Sequence()
        {
            //ステージセレクト待機
            await UniTask.WaitUntil(IsGameState(GameState.Playing));
            
            
            await additiveSceneLoader.Load(levelName);
            
            //クリア待機
            await UniTask.WaitUntil(IsGameState(GameState.StageSelect));
        }

        private Func<bool> IsGameState(GameState gameState)
        {
            return () => this.gameState == gameState;
        }
    }
}