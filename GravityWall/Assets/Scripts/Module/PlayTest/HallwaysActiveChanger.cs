using System.Collections;
using System.Collections.Generic;
using Constants;
using Module.Gimmick;
using UnityEngine;

namespace Module.PlayTest
{
    public class HallwaysActiveChanger : MonoBehaviour
    {
        [SerializeField] private Gate gate;
        private MeshRenderer meshRenderer;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tag.Player))
            {
                SetRenderEnabled(true);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tag.Player))
            {
                SetRenderEnabled(false);
            }
        }

        public void SetRenderEnabled(bool isEnable)
        {
            meshRenderer.enabled = isEnable;
        }
    }
}