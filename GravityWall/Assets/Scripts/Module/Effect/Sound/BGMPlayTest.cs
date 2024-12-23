using System;
using System.Collections;
using System.Collections.Generic;
using Core.Sound;
using CoreModule.Sound;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BGMPlayTest : MonoBehaviour
{
    async void Start()
    {
        SoundManager.Instance.Play(SoundKey.MagicPower, MixerType.BGM);

        await UniTask.Delay(TimeSpan.FromSeconds(5));

        SoundManager.Instance.Pause(0.5f);
        
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        
        SoundManager.Instance.Resume(0.5f);
    }
}