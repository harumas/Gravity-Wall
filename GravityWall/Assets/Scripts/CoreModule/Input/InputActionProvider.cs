using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreModule.Input
{
    /// <summary>
    ///     登録解除不要なInputActionを提供するクラス
    /// </summary>
    public static class InputActionProvider
    {
        public static InputActionAsset ActionAsset => inputActionAsset;
        
        private static InputActionAsset inputActionAsset;
        private static readonly List<InputEvent> inputEvents = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            inputActionAsset = Resources.Load<InputActionAsset>("InputActionAsset");

            if (inputActionAsset == null)
            {
                Debug.LogError("InputActionAssetのロードに失敗しました。");
                return;
            }
            
            inputActionAsset.Enable();

#if UNITY_EDITOR
            Application.quitting += OnDispose;
#else
            EditorApplication.quitting += OnDispose;
#endif
        }

        private static void OnDispose()
        {
            foreach (var inputEvent in inputEvents)
            {
                inputEvent.Clear();
            }
        }

        public static InputEvent CreateEvent(Guid guid)
        {
            var inputAction = inputActionAsset.FindAction(guid);
            Debug.Assert(inputAction != null);

            var inputEvent = new InputEvent(inputAction);
            inputEvents.Add(inputEvent);

            return inputEvent;
        }

        public static InputActionMap GetActionMap(Guid guid)
        {
            return inputActionAsset.FindActionMap(guid);
        }
    }
}