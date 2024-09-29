using Constants;
using UnityEngine;

namespace Module.Gimmick.LevelMask
{
    /// <summary>
    /// MaskVolumeを透過するためのボリューム
    /// </summary>
    public class FilterVolume : MonoBehaviour
    {
        [SerializeField] private Renderer maskRenderer;
        [SerializeField] private Renderer filterRenderer;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                Enable();
            }
        }

        public void Enable()
        {
            maskRenderer.enabled = false;
            filterRenderer.enabled = true;
        }
        
        public void Disable()
        {
            maskRenderer.enabled = true;
            filterRenderer.enabled = false;
        }

        private void OnDrawGizmosSelected()
        {
            Visualize();
        }

        public void Visualize()
        {
            Matrix4x4 cache = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.color = new Color(1f, 0f, 0f, 0.31f);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = cache;
        }
    }
}