using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class Switch : AbstractSwitch
    {
        [SerializeField] private bool initializeIsOn = false;
        [SerializeField] private List<AbstractGimmickAffected> gimmickAffecteds = new List<AbstractGimmickAffected>();

        public override bool isOn { get => _isOn; protected set => _isOn = value; }
        private bool _isOn;

        void Start()
        {
            isOn = initializeIsOn;
        }

        public override void OnSwitch(bool isOn)
        {
            this.isOn = isOn;

            foreach (var gimmick in gimmickAffecteds)
            {
                gimmick.Affect(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("BatteryBox"))
            {
                OnSwitch(!isOn);
            }
        }

    }
}