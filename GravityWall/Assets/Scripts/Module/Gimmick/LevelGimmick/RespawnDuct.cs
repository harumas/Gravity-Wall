using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class RespawnDuct : MonoBehaviour
    {
        [SerializeField] private RespawnDuctControllerWrapper controller;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float closeInterval = 1.0f;
        [SerializeField] private float returnCameraInterval = 1.0f;
        
        private void OnTriggerEnter(Collider other)
        {
            virtualCamera.Priority = 1000;
            controller.IsOpen = true;
            CloseAsync().Forget();
        }

        private async UniTaskVoid CloseAsync()
        {
            // ダクトの扉が閉まるアニメーションを待機
            await UniTask.Delay(TimeSpan.FromSeconds(closeInterval), cancellationToken: destroyCancellationToken);
            controller.IsOpen = false;
            
            // カメラがプレイヤーに戻るまで待
            await UniTask.Delay(TimeSpan.FromSeconds(returnCameraInterval), cancellationToken: destroyCancellationToken);
            virtualCamera.Priority = 0;
        }
    }
}