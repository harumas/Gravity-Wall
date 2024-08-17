using Constants;
using UnityEngine;

namespace Module.Gimmick.LevelMask
{
    /// <summary>
    /// レベルの部屋をマスクするボリューム
    /// </summary>
    [ExecuteAlways]
    public class MaskVolume : MonoBehaviour
    {
        [SerializeField] private Renderer maskRenderer;
        [SerializeField] private FilterVolume[] filterGroup;

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
                Disable();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                Enable();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                Disable();
            }
        }

        public void Enable()
        {
            maskRenderer.enabled = false;

            foreach (FilterVolume filterVolume in filterGroup)
            {
                filterVolume.Enable();
            }
        }

        public void Disable()
        {
            maskRenderer.enabled = true;

            foreach (FilterVolume filterVolume in filterGroup)
            {
                filterVolume.Disable();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Matrix4x4 cache = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.color = new Color(0f, 1f, 0f, 0.31f);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = cache;

            foreach (FilterVolume filter in filterGroup)
            {
                if (filter != null)
                {
                    filter.Visualize();
                }
            }
        }
    }
}