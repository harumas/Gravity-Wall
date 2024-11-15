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
        private static readonly int pushRatio = Shader.PropertyToID("_PushRatio");

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
            meshRenderer.material.SetFloat(pushRatio, 1.0f);
        }

        public override void Disable(bool doEffect = true)
        {
            isEnabled.Value = false;
            meshRenderer.material.SetFloat(pushRatio, 0f);
        }

        public override void Reset()
        {
            Disable(false);
        }
    }
}