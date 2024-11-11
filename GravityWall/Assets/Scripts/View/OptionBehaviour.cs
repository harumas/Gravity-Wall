using System;
using Cysharp.Threading.Tasks;
using Module.InputModule;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace View
{
    public class OptionBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.Option;

        [SerializeField] private OptionView optionView;

        public OptionView OptionView => optionView;

        protected override async UniTask OnPreActivate(ViewBehaviourType beforeType)
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate() { }

        protected override void OnDeactivate() { }

        protected override async UniTask OnPostDeactivate(ViewBehaviourType nextType)
        {
            await UniTask.CompletedTask;
        }
    }
}