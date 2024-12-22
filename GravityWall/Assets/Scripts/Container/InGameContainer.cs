using System;
using System.Collections.Generic;
using Application;
using Application.Sequence;
using Application.Spawn;
using CoreModule.Helper;
using Module.Gimmick.LevelGimmick;
using Module.Gimmick.SystemGimmick;
using Module.InputModule;
using Module.Player;
using Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;
using View.Behaviour;
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
        [SerializeField] private DirectorTable directorTable;
        [SerializeField] private MainGateOpenSequencer mainGateOpenSequencer;

        protected override void Configure(IContainerBuilder builder)
        {
            //ゲームが初期化されていなかったらコンテナを構築しない
            if (!GameBoot.IsBooted)
            {
                return;
            }

            // ゲームの進行を管理するクラスの登録
            builder.RegisterEntryPoint<InGameSequencer>();

            // プレイヤーの入力を受け取るプレゼンターを登録
            builder.RegisterEntryPoint<PlayerInputPresenter>();
            builder.RegisterEntryPoint<PlayerVibrationPresenter>();

            // コンフィグ変更のリスナーを登録
            builder.RegisterEntryPoint<InputConfigChangedListener>();
            builder.RegisterEntryPoint<AudioConfigChangedListener>();
            builder.RegisterEntryPoint<OptionChangedPresenter>();

            // UIのプレゼンターを登録
            builder.RegisterEntryPoint<ViewBehaviourInitializer>();
            builder.RegisterEntryPoint<TitleBehaviourPresenter>();
            builder.RegisterEntryPoint<LicenseBehaviourPresenter>();
            builder.RegisterEntryPoint<PauseBehaviourPresenter>();
            builder.RegisterEntryPoint<OptionBehaviourPresenter>();
            builder.RegisterEntryPoint<CreditBehaviourPresenter>();
            builder.RegisterEntryPoint<ConfirmNewGamePresenter>();

            builder.Register<PlayerInput>(Lifetime.Singleton).As<IGameInput>();
            builder.Register<CursorLocker>(Lifetime.Singleton);
            builder.Register<RespawnManager>(Lifetime.Singleton);
            builder.Register<HubSpawner>(Lifetime.Singleton);

            gimmickReference.UpdateReference();
            builder.RegisterInstance(gimmickReference);
            builder.RegisterInstance(hubSpawnPoint);
            builder.RegisterInstance(directorTable);
            RegisterWithNullCheck(builder, mainGateOpenSequencer);

            // UIパネルの登録
            RegisterBehaviourComponents(builder);

            // プレイヤーのコンポーネントの登録
            RegisterPlayerComponents(builder);

            // シーン間で使い回すコンポーネントを登録
            RegisterReusableComponents(builder);

#if UNITY_EDITOR
            // エディタからコンポーネントにアクセスするためのクラスを登録
            builder.RegisterEntryPoint<ExternalAccessor>();
#endif
        }

        private void RegisterWithNullCheck<T>(IContainerBuilder builder, T instance) where T : class
        {
            if (instance == null)
            {
                throw new NullReferenceException($"{typeof(T).Name} がアタッチされていません");
            }

            builder.RegisterInstance(instance);
        }

        private void RegisterBehaviourComponents(IContainerBuilder builder)
        {
            behaviourNavigator.RegisterBehaviours();
            RegisterWithNullCheck(builder, behaviourNavigator);
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<OptionBehaviour>(ViewBehaviourState.Option));
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<LoadingBehaviour>(ViewBehaviourState.Loading));
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<ClearBehaviour>(ViewBehaviourState.Clear));
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<TitleBehaviour>(ViewBehaviourState.Title));
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<LicenseBehaviour>(ViewBehaviourState.License));
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<PauseBehaviour>(ViewBehaviourState.Pause));
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<CreditBehaviour>(ViewBehaviourState.Credit));
            RegisterWithNullCheck(builder, behaviourNavigator.GetBehaviour<ConfirmNewGameBehaviour>(ViewBehaviourState.ConfirmNewGame));
        }

        private void RegisterReusableComponents(IContainerBuilder builder)
        {
            var reusableComponents = new List<IReusableComponent>
            {
                RegisterReusableComponent<SavePoint>(builder),
                RegisterReusableComponent<DeathFloor>(builder),
                RegisterReusableComponent<LevelVolumeCamera>(builder)
            };

            builder.RegisterInstance(reusableComponents).As<IReadOnlyList<IReusableComponent>>();
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