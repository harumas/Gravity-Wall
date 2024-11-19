using CoreModule.Save;
using Cysharp.Threading.Tasks;
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
        private readonly SaveManager<ConfigData> saveManager;
        private readonly ConfigData configData;
        private readonly AudioMixer audioMixer;

        [Inject]
        public AudioConfigChangedListener(SaveManager<ConfigData> saveManager, AudioMixer audioMixer)
        {
            configData = saveManager.Data;
            this.saveManager = saveManager;
            this.audioMixer = audioMixer;
        }

        public void Start()
        {
            UpdateAllVolumes();

            configData.MasterVolume.Subscribe(UpdateMasterVolume);
            configData.BgmVolume.Subscribe(UpdateBgmVolume);
            configData.SeVolume.Subscribe(UpdateSeVolume);
            configData.AmbientVolume.Subscribe(UpdateAmbientVolume);

            configData.MasterVolume.Subscribe(SaveConfig);
            configData.BgmVolume.Subscribe(SaveConfig);
            configData.SeVolume.Subscribe(SaveConfig);
            configData.AmbientVolume.Subscribe(SaveConfig);
        }

        private void UpdateAllVolumes()
        {
            UpdateMasterVolume(configData.MasterVolume.CurrentValue);
            UpdateBgmVolume(configData.BgmVolume.CurrentValue);
            UpdateSeVolume(configData.SeVolume.CurrentValue);
            UpdateAmbientVolume(configData.AmbientVolume.CurrentValue);
        }

        private void SaveConfig(float _)
        {
            saveManager.Save().Forget();
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