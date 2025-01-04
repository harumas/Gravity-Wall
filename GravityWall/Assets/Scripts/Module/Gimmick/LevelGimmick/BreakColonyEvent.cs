using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using Module.Player;
using CoreModule.Sound;

namespace Module.Gimmick
{
    public class BreakColonyEvent : MonoBehaviour
    {
        [SerializeField] private GameObject startWall, dengerWall,breakGlassHouse;
        private CameraShaker cameraShaker;

        private bool wasBreak;

        async UniTaskVoid OnBreakColony()
        {
            SetBuilding(breakGlassHouse);

            SetBuilding(startWall);

            wasBreak = true;

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            SetBuilding(dengerWall);
        }

        private void SetBuilding(GameObject building)
        {
            if (breakGlassHouse && !wasBreak)
            {
                SoundManager.Instance.Play(Core.Sound.SoundKey.LastBGMSample, Core.Sound.MixerType.BGM);
            }

            if (!building) return;

            var dengerWallLevels = building.transform.Find("Levels").gameObject;
            building.gameObject.SetActive(true);
            dengerWallLevels.transform.DOLocalMove(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    building.transform.Find("Effects").gameObject.SetActive(true);
                    SoundManager.Instance.Play(Core.Sound.SoundKey.Bomb,Core.Sound.MixerType.SE);
                    cameraShaker?.ShakeCamera(0.5f, 1);
                });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (wasBreak) return;

            if (other.CompareTag(Tag.Player))
            {
                OnBreakColony().Forget();
                cameraShaker = other.GetComponent<CameraShaker>();
            }
        }
    }
}