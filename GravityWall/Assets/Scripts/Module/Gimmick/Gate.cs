using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class Gate : AbstractGimmickAffected
    {
        [SerializeField] private GameObject gate;
        public override void Affect(AbstractSwitch switchObject)
        {
            gate.SetActive(!switchObject.isOn);
        }
    }
}