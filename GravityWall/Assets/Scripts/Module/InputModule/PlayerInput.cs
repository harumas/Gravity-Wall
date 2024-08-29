using Core.Input;
using CoreModule.Input;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Module.InputModule
{
    public class PlayerInput : IGameInput
    {
        private readonly ReactiveProperty<Vector2> lookDeltaProperty = new ReactiveProperty<Vector2>();
        private readonly ReactiveProperty<Vector2> moveProperty = new ReactiveProperty<Vector2>();
        private readonly Subject<Unit> jumpSubject = new Subject<Unit>();

        public PlayerInput()
        {
            //回転入力の更新イベントを登録
            InputEvent lookEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Look);

            Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate)
                .Select(_ => lookEvent.ReadValue<Vector2>() * Time.deltaTime)
                .Subscribe(value => lookDeltaProperty.Value = value);
                
            //移動入力のイベントを登録
            InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
            moveEvent.Started += OnMove;
            moveEvent.Performed += OnMove;
            moveEvent.Canceled += OnMove;

            //ジャンプ入力のイベントを登録
            InputEvent jumpEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Jump);
            jumpEvent.Started += _ => jumpSubject.OnNext(Unit.Default);
        }

        public ReadOnlyReactiveProperty<Vector2> LookDelta => lookDeltaProperty;
        public ReadOnlyReactiveProperty<Vector2> Move => moveProperty;
        public Observable<Unit> Jump => jumpSubject;

        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveProperty.Value = ctx.ReadValue<Vector2>();
        }
    }
}