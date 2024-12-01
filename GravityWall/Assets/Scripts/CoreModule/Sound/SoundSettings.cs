using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CoreModule.Sound
{
    /// <summary>
    /// サウンド設定を管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(menuName = "AudioClipTable", fileName = "AudioClipTable")]
    public class SoundSettings : ScriptableObject
    {
        [Serializable]
        internal class AudioClipPair : SerializablePair<string, AudioClip>
        {
            public AudioClipPair(string key, AudioClip value) : base(key, value)
            {
            }
        }

        [Header("SoundManager設定")]
        [SerializeField, Header("最低の再生間隔時間(音割れ防止用)")]
        private float minPlayInterval;

        [SerializeField, Header("同時再生ソースの最大数")] private int maxSourceCount;

        [Header("サウンドリソース設定")] [SerializeField] private string exportPath;
        [SerializeField] private List<AudioClipPair> audioClips;
        [SerializeField] private List<AudioMixerGroup> audioMixerGroups;

        public float MinPlayInterval => minPlayInterval;
        public int MaxSourceCount => maxSourceCount;

        public List<AudioClip> GetAudioClips()
        {
            return audioClips.ConvertAll(pair => pair.Value);
        }

        public List<AudioMixerGroup> GetAudioMixerGroups()
        {
            return audioMixerGroups;
        }

        [Button("SoundKeyを生成する")]
        private void Generate()
        {
            SoundKeyGenerator.Generate(audioClips, audioMixerGroups, exportPath);
        }
    }
}