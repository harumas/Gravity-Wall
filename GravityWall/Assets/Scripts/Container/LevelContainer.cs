using System;
using System.Collections.Generic;
using Application.Sequence;
using Constants;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container
{
    public class LevelContainer : LifetimeScope
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

            builder.RegisterEntryPoint<LevelVolumeCameraPresenter>();
            builder.RegisterEntryPoint<SequenceViewPresenter>();

            builder.Register<RespawnManager>(Lifetime.Singleton);
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