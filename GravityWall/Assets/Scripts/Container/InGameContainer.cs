using Application;
using Constants;
using Module.Character;
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
        protected override void Configure(IContainerBuilder builder)
        {
            //ゲームが初期化されていなかったらコンテナを構築しない
            if (!GameBoot.IsBooted)
            {
                return;
            }

            WorldGravity.Create();

            builder.RegisterEntryPoint<InputConfigChangedListener>();
            builder.RegisterEntryPoint<PlayerInputPresenter>();

#if UNITY_EDITOR
            builder.RegisterEntryPoint<ExternalAccessor>();
#endif

            builder.Register<PlayerInput>(Lifetime.Singleton).As<IGameInput>();

            RegisterPlayerComponents(builder);
        }

        private void RegisterPlayerComponents(IContainerBuilder builder)
        {
            //プレイヤーのコンポーネントを取得して登録
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(Tag.Player))
            {
                if (obj.TryGetComponent(out PlayerController playerController))
                {
                    builder.RegisterComponent(playerController);
                }

                if (obj.TryGetComponent(out CameraController cameraController))
                {
                    builder.RegisterComponent(cameraController);
                }

                if (obj.TryGetComponent(out PlayerTargetSyncer playerTargetSyncer))
                {
                    builder.RegisterComponent(playerTargetSyncer);
                }
            }
        }
    }
}