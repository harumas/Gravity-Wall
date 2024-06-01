using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class LocalGravity : MonoBehaviour
    {
        public Gravity.Type GravityType => gravityType;

        [SerializeField] private float multiplier = 1f;
        [SerializeField] private Gravity.Type gravityType;
        private readonly Queue<float> externalMultipliers = new Queue<float>();
        private Rigidbody rigBody;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (Gravity.IsEnable(gravityType))
            {
                float externalMultiplier = externalMultipliers.Count == 0 ? 1f : externalMultipliers.Dequeue();
                rigBody.AddForce(Gravity.Value * (multiplier * externalMultiplier), ForceMode.Acceleration);
            }
            else
            {
                rigBody.velocity = Vector3.zero;
            }
        }

        public void AddExternalMultiplier(ReadOnlySpan<float> multipliers)
        {
            foreach (float m in multipliers)
            {
                externalMultipliers.Enqueue(m);
            }
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            rigBody.velocity = Vector3.zero;
        }
    }
}