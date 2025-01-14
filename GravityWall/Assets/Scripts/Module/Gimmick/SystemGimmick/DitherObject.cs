using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    /// <summary>
    /// オブジェクトを透過処理するコンポーネント
    /// </summary>
    public class DitherObject : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private float ditherStrength = 0.5f;

        private static readonly int wholeDitherStrengthProperty = Shader.PropertyToID("_WholeDitherStrength");

        private void Start()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        public void Dither()
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.SetFloat(wholeDitherStrengthProperty, ditherStrength);
            }
        }

        public void Show()
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.SetFloat(wholeDitherStrengthProperty, 0f);
            }
        }
    }
}