using System;
using System.Collections.Generic;
using Baracuda.Monitoring;
using UnityEngine;

namespace Module.Gimmick.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class PhysicsSoundEffector : MonitoredBehaviour
    {
        [Header("速度変化によって音を鳴らす閾値")]
        [SerializeField] private float powerThreshold;
        
        [Header("速度による音減衰の係数")]
        [SerializeField] private float powerMultiplier;

        private AudioSource audioSource;
        private Rigidbody rigBody;
        private Vector3 lastVelocity;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            rigBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float velocityDiff = (lastVelocity - rigBody.velocity).sqrMagnitude;

            //速度変化が閾値を超えた場合音を再生
            if (velocityDiff > powerThreshold)
            {
                //速度によって音量を減衰させる
                audioSource.volume = velocityDiff * powerMultiplier;
                audioSource.Play();
            }

            lastVelocity = rigBody.velocity;
        }
    }
}