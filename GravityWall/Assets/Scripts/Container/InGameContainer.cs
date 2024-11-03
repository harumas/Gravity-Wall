using System;
using System.Buffers;
using System.Collections.Generic;
using Application;
using Application.Sequence;
using Constants;
using Module.Character;
using Module.Gimmick;
using Module.Gravity;
using Module.InputModule;
using Presentation;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;
using View;
using PlayerInput = Module.InputModule.PlayerInput;

namespace Container
{
    /// <summary>
    /// インゲームのDIコンテナ
    /// </summary>
    public class InGameContainer : LifetimeScope
    {
        [SerializeField] private ViewBehaviourNavigator behaviourNavigator;

        protected override void Configure(IContainerBuilder builder)
        {
            //ゲームが初期化されていなかったらコンテナを構築しない
            if (!GameBoot.IsBooted)
            {
                return;
            }

            //重力の生成 (後で消す)
            WorldGravity.Create();

            UnityEngine.Application.targetFrameRate = 120;


            builder.RegisterEntryPoint<PlayerInputPresenter>();

            //コンフィグ変更のリスナーを登録
            builder.RegisterEntryPoint<InputConfigChangedListener>();
            builder.RegisterEntryPoint<AudioConfigChangedListener>();

#if UNITY_EDITOR
            builder.RegisterEntryPoint<ExternalAccessor>();
#endif

            builder.Register<PlayerInput>(Lifetime.Singleton).As<IGameInput>();

            RegisterInstanceWithNullCheck(builder, behaviourNavigator);
            RegisterPlayerComponents(builder);


            builder.RegisterInstance<List<SavePoint>>(new List<SavePoint>(20));
        }

        private void RegisterInstanceWithNullCheck<T>(IContainerBuilder builder, T instance) where T : class
        {
            if (instance == null)
            {
                throw new NullReferenceException($"{typeof(T).Name} がアタッチされていません");
            }

            builder.RegisterInstance(instance);
        }

        private void RegisterPlayerComponents(IContainerBuilder builder)
        {
            //プレイヤーのコンポーネントを取得して登録
            builder.RegisterComponentInHierarchy<PlayerController>();
            builder.RegisterComponentInHierarchy<CameraController>();
            builder.RegisterComponentInHierarchy<PlayerTargetSyncer>();
            builder.RegisterComponentInHierarchy<GravitySwitcher>();
        }
    }
}