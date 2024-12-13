using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class ObjectDither : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        private const float ditherStrength = 0.5f;

        private static readonly int wholeDitherStrengthProperty = Shader.PropertyToID("_WholeDitherStrength");

        private void Start()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        public void Show()
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.SetFloat(wholeDitherStrengthProperty, ditherStrength);
            }
        }

        public void Hide()
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.SetFloat(wholeDitherStrengthProperty, 0f);
            }
        }
    }
}