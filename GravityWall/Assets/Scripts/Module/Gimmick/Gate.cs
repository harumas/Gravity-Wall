using UnityEngine;

namespace Module.Gimmick
{
    public class Gate : AbstractGimmickAffected
    {
        [SerializeField] private GameObject gate;
        [SerializeField] private int switchMaxCount = 1;
        private int switchCount = 0;
        private bool isOpen;
        public override void Affect(AbstractSwitch switchObject)
        {
            switchCount += switchObject.isOn ? 1 : -1;
            isOpen = switchCount >= switchMaxCount;
            gate.SetActive(!isOpen);
        }
    }
}