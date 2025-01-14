using System;
using System.Collections.Generic;
using Core.Sound;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        private AudioMixer masterMixer;
        private List<AudioMixerGroup> audioMixerGroups;
        private Queue<AudioSource> audioSources;
        private Queue<PlayInfo> scheduleQueue;
        private List<PlayInfo> playingQueue;
        private List<AudioClip> audioClips;
        private Queue<PlayInfo> resumePlayQueue;
        private HashSet<int> stopSet;
        private PlayInfo latestPlayInfo;
        private float pauseTime;
        private int handleCounter;

        /// <summary>
        /// SoundManagerを初期化します
        /// </summary>
        public void Construct(SoundSettings soundSettings)
        {
            this.soundSettings = soundSettings;
            masterMixer = soundSettings.AudioMixer;

            // AudioSourceの最大数でQueueを初期化
            int maxSourceCount = soundSettings.MaxSourceCount;
            audioSources = new Queue<AudioSource>(maxSourceCount);
            scheduleQueue = new Queue<PlayInfo>(maxSourceCount);
            playingQueue = new List<PlayInfo>(maxSourceCount);
            resumePlayQueue = new Queue<PlayInfo>(maxSourceCount);
            stopSet = new HashSet<int>();

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
        public int Play(SoundKey key, MixerType mixerType, bool isLoop = false)
        {
            return Play(key, mixerType, PlayContext.Default, isLoop);
        }

        /// <summary>
        /// サウンドを再生します
        /// </summary>
        /// <param name="key">AudioClipのキー</param>
        /// <param name="mixerType">AudioMixerのタイプ</param>
        /// <param name="playContext">再生設定</param>
        public int Play(SoundKey key, MixerType mixerType, PlayContext playContext, bool isLoop = false)
        {
            // キーが存在しない or 再生できるAudioSourceがない場合は再生しない
            if ((int)key >= audioClips.Count || audioSources.Count == 0)
            {
                return -1;
            }

            // 最低再生間隔を保持して再生時間を決める
            float time = Math.Max(Time.unscaledTime, latestPlayInfo.PlayTime + soundSettings.MinPlayInterval);
            AudioSource source = GetSource(key, mixerType, playContext, isLoop);

            // 再生をスケジュールする
            int handleId = handleCounter++;
            latestPlayInfo = new PlayInfo(handleId, time, isLoop, mixerType, source);
            scheduleQueue.Enqueue(latestPlayInfo);

            return handleId;
        }

        public AudioSource GetSource(SoundKey key, MixerType mixerType, PlayContext playContext, bool isLoop)
        {
            // AudioSourceを設定する
            AudioSource source = audioSources.Dequeue();
            source.outputAudioMixerGroup = audioMixerGroups[(int)mixerType];
            source.clip = audioClips[(int)key];
            source.volume = playContext.Volume;
            source.pitch = playContext.Pitch;
            source.loop = isLoop;

            return source;
        }

        public void Stop(int handle)
        {
            stopSet.Add(handle);
        }

        public async void Pause(float fadeDuration)
        {
            await DOFadeOut(fadeDuration);

            foreach (PlayInfo playInfo in playingQueue)
            {
                if (playInfo.MixerType == MixerType.BGM)
                {
                    playInfo.Source.Pause();
                    resumePlayQueue.Enqueue(playInfo);
                }
            }

            playingQueue.RemoveAll(info => info.MixerType == MixerType.BGM);
            pauseTime = Time.unscaledTime;
        }

        public async void Resume(float fadeDuration)
        {
            float elapsedTime = Time.unscaledTime - pauseTime;

            while (resumePlayQueue.TryDequeue(out PlayInfo playInfo))
            {
                playInfo.Source.UnPause();
                playInfo = new PlayInfo(playInfo.HandleId, playInfo.PlayTime + elapsedTime, playInfo.IsLoop, playInfo.MixerType, playInfo.Source);

                playingQueue.Add(playInfo);
            }

            await DOFadeIn(fadeDuration);
        }

        private void Update()
        {
            // スケジュールされたサウンドを再生する
            while (scheduleQueue.Count > 0)
            {
                PlayInfo info = scheduleQueue.Peek();

                // 再生時間に到達していない場合は再生しない
                if (info.PlayTime > Time.unscaledTime)
                {
                    break;
                }

                // 再生する
                info.Source.Play();

                // 再生中のキューに追加
                playingQueue.Add(scheduleQueue.Dequeue());
            }

            int removeCount = 0;
            Span<int> removeIndexes = stackalloc int[playingQueue.Count];

            // 再生中のキューから再生が終了したものを削除する
            for (int i = 0; i < playingQueue.Count; i++)
            {
                PlayInfo info = playingQueue[i];

                // 再生が終了していない場合は削除しない
                if ((info.IsLoop || info.PlayTime + info.Source.clip.length > Time.unscaledTime) && !stopSet.Contains(info.HandleId))
                {
                    continue;
                }
                
                info.Source.Stop();

                // 再生が終了したAudioSourceを返却する
                audioSources.Enqueue(info.Source);
                removeIndexes[removeCount++] = i;
                stopSet.Remove(info.HandleId);
            }

            int offset = 0;

            for (int i = 0; i < removeCount; i++)
            {
                playingQueue.RemoveAt(removeIndexes[i] + offset);
                offset--;
            }
        }

        private const string MixerKey = "GameMasterVolume";

        private UniTask DOFadeOut(float duration)
        {
            return DOTween.To(() => 1f, x => masterMixer.SetFloat(MixerKey, GetDecibel(x)), 0f, duration).ToUniTask();
        }

        private UniTask DOFadeIn(float duration)
        {
            return DOTween.To(() => 0f, x => masterMixer.SetFloat(MixerKey, GetDecibel(x)), 1f, duration).ToUniTask();
        }

        private float GetDecibel(float value)
        {
            return Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        }

        /// <summary>
        /// 再生情報を管理する構造体
        /// </summary>
        private readonly struct PlayInfo
        {
            public readonly int HandleId;
            public readonly float PlayTime;
            public readonly bool IsLoop;
            public readonly MixerType MixerType;
            public readonly AudioSource Source;

            public PlayInfo(int handleId, float playTime, bool isLoop, MixerType mixerType, AudioSource source)
            {
                HandleId = handleId;
                PlayTime = playTime;
                IsLoop = isLoop;
                MixerType = mixerType;
                Source = source;
            }
        }

        [Serializable]
        private class AudioMixerPair : SerializablePair<MixerType, AudioMixerGroup>
        {
            public AudioMixerPair(MixerType key, AudioMixerGroup value) : base(key, value) { }
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