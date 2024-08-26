using System;
using Core.Input;
using Module.Config;
using Module.Core.Input;
using Module.Core.Save;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Module.InputModule
{
    public class PlayerInput : IGameInput
    {
        private readonly InputEvent mouseEvent;
        
        private readonly ReactiveProperty<Vector2> moveProperty = new ReactiveProperty<Vector2>();
        private readonly Subject<Unit> jumpSubject = new Subject<Unit>();
        
        public PlayerInput()
        {
            mouseEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Look);
            
            InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
            moveEvent.Started += OnMove;
            moveEvent.Performed += OnMove;
            moveEvent.Canceled += OnMove;
            
            InputEvent jumpEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Jump);
            jumpEvent.Started += _ => jumpSubject.OnNext(Unit.Default);
        }

        public Vector2 MouseDelta => mouseEvent.ReadValue<Vector2>();
        public ReadOnlyReactiveProperty<Vector2> Move => moveProperty;
        public Observable<Unit> Jump => jumpSubject;

        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveProperty.Value = ctx.ReadValue<Vector2>();
        }
    }
}