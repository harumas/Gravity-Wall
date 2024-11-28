using System;
using System.Collections.Generic;
using Application.Sequence;
using Constants;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container
{
    public class InLevelContainer : LifetimeScope
    {
        private async void Start()
        {
            //シーン読み込みと被らないようにフレームをずらす
            await UniTask.DelayFrame(5);
            Build();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            SetReusableComponents();

            builder.RegisterEntryPoint<InLevelSequencer>();
            builder.RegisterEntryPoint<LevelVolumeCameraPresenter>();
            builder.RegisterEntryPoint<SequenceViewPresenter>();
            builder.RegisterEntryPoint<GameClearPresenter>();
            builder.RegisterEntryPoint<LevelEventPresenter>();

            var gimmickReference = Parent.Container.Resolve<GimmickReference>();
            gimmickReference.UpdateReference();
        }

        private void SetReusableComponents()
        {
            var reusableComponents = Parent.Container.Resolve<IReadOnlyList<IReusableComponent>>();
            var parents = GameObject.FindGameObjectsWithTag(Tag.LevelSegment);

            foreach (IReusableComponent component in reusableComponents)
            {
                component.SetComponentsInChildren(parents);
            }
        }
    }
}