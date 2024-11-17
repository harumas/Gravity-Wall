using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;
using Application;
using Application.Sequence;
using Constants;
using CoreModule.Helper;
using Module.Character;
using Module.Gimmick;
using Module.Gimmick.LevelGimmick;
using Module.Gravity;
using Module.InputModule;
using Presentation;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
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
        [SerializeField] private GimmickReference gimmickReference;
        [SerializeField] private HubSpawnPoint hubSpawnPoint;

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

            builder.RegisterEntryPoint<InGameSequencer>();
            builder.RegisterEntryPoint<PlayerInputPresenter>();

            //コンフィグ変更のリスナーを登録
            builder.RegisterEntryPoint<InputConfigChangedListener>();
            builder.RegisterEntryPoint<AudioConfigChangedListener>();
            builder.RegisterEntryPoint<OptionChangedPresenter>();

            //builder.RegisterEntryPoint<ViewBehaviourInitializer>();
            builder.RegisterEntryPoint<TitleBehaviourPresenter>();
            builder.RegisterEntryPoint<LicenseBehaviourPresenter>();
            builder.RegisterEntryPoint<PauseBehaviourPresenter>();
            builder.RegisterEntryPoint<OptionBehaviourPresenter>();
            builder.RegisterEntryPoint<CreditBehaviourPresenter>();

            builder.Register<PlayerInput>(Lifetime.Singleton).As<IGameInput>();
            builder.Register<CursorLocker>(Lifetime.Singleton);
            builder.Register<RespawnManager>(Lifetime.Singleton);

            //ViewBehaviourの登録
            behaviourNavigator.RegisterBehaviours();
            builder.RegisterComponent(behaviourNavigator.GetBehaviour<OptionBehaviour>(ViewBehaviourState.Option));
            builder.RegisterComponent(behaviourNavigator.GetBehaviour<LoadingBehaviour>(ViewBehaviourState.Loading));
            builder.RegisterComponent(behaviourNavigator.GetBehaviour<ClearBehaviour>(ViewBehaviourState.Clear));
            builder.RegisterComponent(behaviourNavigator.GetBehaviour<TitleBehaviour>(ViewBehaviourState.Title));
            builder.RegisterComponent(behaviourNavigator.GetBehaviour<LicenseBehaviour>(ViewBehaviourState.License));
            builder.RegisterComponent(behaviourNavigator.GetBehaviour<PauseBehaviour>(ViewBehaviourState.Pause));
            builder.RegisterComponent(behaviourNavigator.GetBehaviour<CreditBehaviour>(ViewBehaviourState.Credit));
            RegisterInstanceWithNullCheck(builder, behaviourNavigator);

            RegisterPlayerComponents(builder);

            var reusableComponents = new List<IReusableComponent>
            {
                RegisterReusableComponent<SavePoint>(builder),
                RegisterReusableComponent<DeathFloor>(builder),
                RegisterReusableComponent<LevelVolumeCamera>(builder)
            };

            builder.RegisterInstance(reusableComponents).As<IReadOnlyList<IReusableComponent>>();
            builder.RegisterInstance(gimmickReference);

            builder.Register<HubSpawner>(Lifetime.Singleton);
            builder.RegisterInstance(hubSpawnPoint);

#if UNITY_EDITOR
            builder.RegisterEntryPoint<ExternalAccessor>();
#endif
        }

        private void RegisterInstanceWithNullCheck<T>(IContainerBuilder builder, T instance) where T : class
        {
            if (instance == null)
            {
                throw new NullReferenceException($"{typeof(T).Name} がアタッチされていません");
            }

            builder.RegisterInstance(instance);
        }

        private IReusableComponent RegisterReusableComponent<T>(IContainerBuilder builder) where T : Component
        {
            var reusableComponent = new ReusableComponents<T>();
            builder.RegisterInstance(reusableComponent);

            return reusableComponent;
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