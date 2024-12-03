using System;
using System.Collections.Generic;
using Core.Sound;
using UnityEngine;
using UnityEngine.Audio;
using AudioSource = UnityEngine.AudioSource;

namespace CoreModule.Sound
{
    /// <summary>
    /// サウンド再生を管理するクラス
    /// </summary>
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        private SoundSettings soundSettings;
        private List<AudioMixerGroup> audioMixerGroups;
        private Queue<AudioSource> audioSources;
        private Queue<PlayInfo> scheduleQueue;
        private Queue<PlayInfo> playingQueue;
        private List<AudioClip> audioClips;
        private PlayInfo latestPlayInfo;

        /// <summary>
        /// SoundManagerを初期化します
        /// </summary>
        public void Construct(SoundSettings soundSettings)
        {
            this.soundSettings = soundSettings;

            // AudioSourceの最大数でQueueを初期化
            int maxSourceCount = soundSettings.MaxSourceCount;
            audioSources = new Queue<AudioSource>(maxSourceCount);
            scheduleQueue = new Queue<PlayInfo>(maxSourceCount);
            playingQueue = new Queue<PlayInfo>(maxSourceCount);

            // AudioSourceを生成する
            for (int i = 0; i < maxSourceCount; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                audioSources.Enqueue(source);
            }

            audioClips = soundSettings.GetAudioClips();
            audioMixerGroups = soundSettings.GetAudioMixerGroups();
        }

        /// <summary>
        /// サウンドを再生します
        /// </summary>
        /// <param name="key">AudioClipのキー</param>
        /// <param name="mixerType">AudioMixerのタイプ</param>
        public void Play(SoundKey key, MixerType mixerType)
        {
            Play(key, mixerType, PlayContext.Default);
        }

        /// <summary>
        /// サウンドを再生します
        /// </summary>
        /// <param name="key">AudioClipのキー</param>
        /// <param name="mixerType">AudioMixerのタイプ</param>
        /// <param name="playContext">再生設定</param>
        public void Play(SoundKey key, MixerType mixerType, PlayContext playContext)
        {
            // キーが存在しない or 再生できるAudioSourceがない場合は再生しない
            if ((int)key >= audioClips.Count || audioSources.Count == 0)
            {
                return;
            }

            // 最低再生間隔を保持して再生時間を決める
            float time = Math.Max(Time.time, latestPlayInfo.PlayTime + soundSettings.MinPlayInterval);

            // AudioSourceを設定する
            AudioSource source = audioSources.Dequeue();
            source.outputAudioMixerGroup = audioMixerGroups[(int)mixerType];
            source.clip = audioClips[(int)key];
            source.volume = playContext.Volume;
            source.pitch = playContext.Pitch;

            // 再生をスケジュールする
            latestPlayInfo = new PlayInfo(time, source);
            scheduleQueue.Enqueue(latestPlayInfo);
        }

        private void Update()
        {
            // スケジュールされたサウンドを再生する
            while (scheduleQueue.Count > 0)
            {
                PlayInfo info = scheduleQueue.Peek();

                // 再生時間に到達していない場合は再生しない
                if (info.PlayTime > Time.time)
                {
                    break;
                }

                // 再生する
                info.Source.Play();

                // 再生中のキューに追加
                playingQueue.Enqueue(scheduleQueue.Dequeue());
            }

            // 再生中のキューから再生が終了したものを削除する
            while (playingQueue.Count > 0)
            {
                PlayInfo info = playingQueue.Peek();

                // 再生が終了していない場合は削除しない
                if (info.PlayTime + info.Source.clip.length > Time.time)
                {
                    break;
                }

                // 再生が終了したAudioSourceを返却する
                playingQueue.Dequeue();
                audioSources.Enqueue(info.Source);
            }
        }

        /// <summary>
        /// 再生情報を管理する構造体
        /// </summary>
        private readonly struct PlayInfo
        {
            public readonly float PlayTime;
            public readonly AudioSource Source;

            public PlayInfo(float playTime, AudioSource source)
            {
                PlayTime = playTime;
                Source = source;
            }
        }

        [Serializable]
        private class AudioMixerPair : SerializablePair<MixerType, AudioMixerGroup>
        {
            public AudioMixerPair(MixerType key, AudioMixerGroup value) : base(key, value)
            {
            }
        }
    }

    /// <summary>
    /// 再生設定を管理する構造体
    /// </summary>
    public readonly struct PlayContext
    {
        /// <summary>
        /// ボリューム
        /// </summary>
        public readonly float Volume;
        
        /// <summary>
        /// ピッチ
        /// </summary>
        public readonly float Pitch;

        public static readonly PlayContext Default = new PlayContext(1f, 1f);

        public PlayContext(float volume, float pitch)
        {
            Volume = volume;
            Pitch = pitch;
        }
    }
}