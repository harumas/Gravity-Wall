using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.InputModule;
using R3;
using UnityEngine;

namespace Module.Player.HSM
{
    public class InputEventAdapter : IDisposable
    {
        private readonly IGameInput gameInput;
        private readonly CancellationTokenSource adapterCanceller = new();
        
        public InputEventAdapter(IGameInput gameInput, Observable<Vector2> move,
            Observable<bool> jump,
            Observable<Vector2> lookDelta)
        {
            this.gameInput = gameInput;
            move.Subscribe(v =>
             {
                 Move.Value = v;
             }).AddTo(adapterCanceller.Token);
            
            jump.Subscribe(v =>
            {
                Jump.Value = v;
            }).AddTo(adapterCanceller.Token);
            
            lookDelta.Subscribe(v =>
            {
                LookDelta.Value = v;
            }).AddTo(adapterCanceller.Token);
        }

        public readonly ReactiveProperty<Vector2> Move = new ReactiveProperty<Vector2>();
        public readonly ReactiveProperty<bool> Jump = new ReactiveProperty<bool>();
        public readonly ReactiveProperty<Vector2> LookDelta = new ReactiveProperty<Vector2>();
        
        public void Sync()
        {
            Move.Value = gameInput.Move.CurrentValue;
            Jump.Value = false;
            LookDelta.Value = gameInput.LookDelta.CurrentValue;
        }

        public void Dispose()
        {
            adapterCanceller.Cancel();
            adapterCanceller.Dispose();
        }
    }
}