using System;
using Module.Config;
using Module.Core.Save;
using R3;
using UnityEngine;

namespace Module.InputModule
{
    public static class GameInput
    {
        private static IGameInput gameInput;
        
        //TODO: DIに移す
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void Initialize()
        {
            await SaveManager<ConfigData>.Load();
            gameInput = new PlayerInput();
        }

        public static Vector2 MouseDelta => gameInput.MouseDelta;
        public static ReadOnlyReactiveProperty<Vector2> Move => gameInput.Move;
        public static Observable<Unit> Jump => gameInput.Jump;
    }
}