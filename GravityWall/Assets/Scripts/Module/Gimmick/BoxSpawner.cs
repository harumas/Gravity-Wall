using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class BoxSpawner : AbstractGimmickAffected
    {
        [SerializeField] private bool startActive;
        [SerializeField] private GameObject box;
        private bool isSpawnLock;
        private Vector3 boxPos;

        void Start()
        {
            boxPos = box.transform.position;
            box.gameObject.SetActive(false);
        }

        public override void Affect(AbstractSwitch switchObject)
        {
            if (isSpawnLock) return;

            box.SetActive(true);
            box.transform.position = boxPos;
        }

        public override void Reset()
        {
            if (isSpawnLock) return;

            box.SetActive(true);
            box.transform.position = boxPos;
        }

        public void LockSpawn(bool isLock)
        {
            isSpawnLock = isLock;
        }
    }
}