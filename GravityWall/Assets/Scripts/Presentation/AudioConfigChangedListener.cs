using CoreModule.Save;
using Module.Config;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    /// <summary>
    /// 音量設定の変更を反映するクラス
    /// </summary>
    public class AudioConfigChangedListener : IStartable
    {
        private readonly ConfigData configData;
        private readonly AudioMixer audioMixer;

        [Inject]
        public AudioConfigChangedListener(SaveManager<ConfigData> saveManager, AudioMixer audioMixer)
        {
            configData = saveManager.Data;
            this.audioMixer = audioMixer;
        }

        public void Start()
        {
            UpdateAllVolumes();

            configData.MasterVolume.Subscribe(UpdateMasterVolume);
            configData.BgmVolume.Subscribe(UpdateBgmVolume);
            configData.SeVolume.Subscribe(UpdateSeVolume);
            configData.AmbientVolume.Subscribe(UpdateAmbientVolume);
        }

        private void UpdateAllVolumes()
        {
            UpdateMasterVolume(configData.MasterVolume.CurrentValue);
            UpdateBgmVolume(configData.BgmVolume.CurrentValue);
            UpdateSeVolume(configData.SeVolume.CurrentValue);
            UpdateAmbientVolume(configData.AmbientVolume.CurrentValue);
        }

        private void UpdateMasterVolume(float value)
        {
            audioMixer.SetFloat("MasterVolume", GetDecibel(value));
        }

        private void UpdateBgmVolume(float value)
        {
            audioMixer.SetFloat("BGMVolume", GetDecibel(value));
        }

        private void UpdateSeVolume(float value)
        {
            audioMixer.SetFloat("SEVolume", GetDecibel(value));
        }

        private void UpdateAmbientVolume(float value)
        {
            audioMixer.SetFloat("AmbientVolume", GetDecibel(value));
        }

        //音量の相対量からデシベル値に変換する
        private float GetDecibel(float value)
        {
            return Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        }
    }
}