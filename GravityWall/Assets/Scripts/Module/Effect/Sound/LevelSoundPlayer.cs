using System;
using Core.Sound;
using CoreModule.Sound;
using Module.Gimmick.SystemGimmick;
using UnityEngine;

namespace Module.Effect.Sound
{
    public class LevelSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AdditiveLevelLoadTrigger loadTrigger;
        [SerializeField] private SoundKey soundKey;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        private int soundHandle = -1;

        private void Awake()
        {
            loadTrigger.OnSceneLoaded += OnSceneLoaded;
            loadTrigger.OnSceneUnload += OnSceneUnload;
        }

        private void OnSceneLoaded()
        {
            PlayContext context = new PlayContext(volume, 1f);
            soundHandle = SoundManager.Instance.Play(soundKey, MixerType.BGM, context, true);
        }

        private void OnSceneUnload()
        {
            SoundManager.Instance.Stop(soundHandle);
        }
    }
}