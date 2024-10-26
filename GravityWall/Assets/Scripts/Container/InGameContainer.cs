using System;
using Application;
using Application.Respawn;
using Application.Sequence;
using Constants;
using Module.Character;
using Module.Gimmick;
using Module.Gravity;
using Module.InputModule;
using Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using PlayerInput = Module.InputModule.PlayerInput;

namespace Container
{
    /// <summary>
    /// インゲームのDIコンテナ
    /// </summary>
    public class InGameContainer : LifetimeScope
    {
        [SerializeField] private UISequencer uiSequencer;
        
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

            builder.RegisterEntryPoint<InputConfigChangedListener>();
            builder.RegisterEntryPoint<AudioConfigChangedListener>();
            builder.RegisterEntryPoint<PlayerInputPresenter>();
            builder.RegisterEntryPoint<LevelVolumeCameraPresenter>();
            builder.RegisterEntryPoint<RespawnManager>();

#if UNITY_EDITOR
            builder.RegisterEntryPoint<ExternalAccessor>();
#endif

            builder.Register<PlayerInput>(Lifetime.Singleton).As<IGameInput>();
            RegisterInstanceWithNullCheck(builder, uiSequencer);

            RegisterPlayerComponents(builder);
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