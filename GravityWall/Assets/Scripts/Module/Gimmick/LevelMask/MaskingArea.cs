using System;
using Constants;
using UnityEngine;

namespace Module.Gimmick.LevelMask
{
    [ExecuteAlways]
    public class MaskingArea : MonoBehaviour
    {
        [SerializeField] private Renderer maskRenderer;

        void Update()
        {
            if (!Application.isPlaying)
            {
                maskRenderer.enabled = false;
            }
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                maskRenderer.enabled = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                maskRenderer.enabled = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                maskRenderer.enabled = true;
            }
        }
    }
}