using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class Switch : AbstractSwitch
    {
        [SerializeField] private bool initializeIsOn = false;
        [SerializeField] private List<AbstractGimmickAffected> gimmickAffecteds = new List<AbstractGimmickAffected>();
        [SerializeField, Tag] private List<string> targetTags;
        [SerializeField] private UnityEvent onEvent, offEvent;
        [SerializeField] private MeshRenderer meshRenderer, RayMeshRenderer;
        public override bool isOn { get => _isOn; protected set => _isOn = value; }
        private bool _isOn;

        void Start()
        {
            isOn = initializeIsOn;
        }

        public override void OnSwitch(bool isOn)
        {
            this.isOn = isOn;

            if (isOn)
            {
                onEvent.Invoke();
            }
            else
            {
                offEvent.Invoke();
            }


            meshRenderer.material.SetFloat("_EmissionIntensity", isOn ? 1.0f : 0.0f);
            RayMeshRenderer.material.SetInt("_PowerOn", isOn ? 0 : 1);

            foreach (var gimmick in gimmickAffecteds)
            {
                gimmick.Affect(this);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (targetTags.Any(tag => collider.CompareTag(tag)))
            {
                OnSwitch(!isOn);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (targetTags.Any(tag => collider.CompareTag(tag)))
            {
                OnSwitch(!isOn);
            }
        }
    }
}