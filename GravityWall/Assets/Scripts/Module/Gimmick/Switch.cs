using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module.Gimmick
{
    public class Switch : AbstractSwitch
    {
        [SerializeField] private bool initializeIsOn = false;
        [SerializeField] private List<AbstractGimmickAffected> gimmickAffecteds = new List<AbstractGimmickAffected>();
        [SerializeField, Tag] private List<string> targetTags;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material material;
        public override bool isOn { get => _isOn; protected set => _isOn = value; }
        private bool _isOn;

        void Start()
        {
            isOn = initializeIsOn;
            //meshRenderer.material = new Material(material);
        }

        public override void OnSwitch(bool isOn)
        {
            this.isOn = isOn;

            meshRenderer.material.SetFloat("EmissionIntensity", isOn ? 1.0f : 0.0f);

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