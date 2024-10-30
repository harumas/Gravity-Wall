using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class PressurePlate : GimmickObject
    {
        [SerializeField, Tag] private string[] targetTags;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private UnityEvent onEvent;

        private void Start()
        {
            Reset();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (targetTags.Any(tag => collider.CompareTag(tag)) && !isEnabled.Value)
            {
                Enable();
            }
        }

        public override void Enable(bool doEffect = true)
        {
            isEnabled.Value = true;
            onEvent.Invoke();

            // Emissionの色を変更
            meshRenderer.material.SetFloat("_PushRatio", 1.0f);
        }

        public override void Disable(bool doEffect = true)
        {
            isEnabled.Value = false;
            meshRenderer.material.SetFloat("_PushRatio", 0f);
        }

        public override void Reset()
        {
            Disable(false);
        }
    }
}