using Constants;
using UGizmo;
using UnityEngine;

namespace Module.Gimmick
{
    public class SpaceConstraintObject : MonoBehaviour
    {
        [SerializeField] private float fallDetectOffset;
        [SerializeField] private float detectAllowances;
        [SerializeField] private bool isFalling;
        private Rigidbody rigBody;

        private void Start()
        {
            rigBody = transform.parent.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Vector3 gravityDir = Gravity.Value.normalized;
            Vector3 origin = transform.position + gravityDir * fallDetectOffset;

            bool isHit = Physics.BoxCast(origin,
                transform.localScale,
                gravityDir,
                out RaycastHit hitInfo,
                transform.rotation,
                float.PositiveInfinity);

            UGizmos.DrawBoxCast(origin,
                transform.localScale,
                gravityDir,
                transform.rotation,
                float.PositiveInfinity,
                isHit,
                hitInfo);

            isFalling = isHit && hitInfo.distance > detectAllowances;
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }

        private void FixedUpdate()
        {
            if (isFalling)
            {
                rigBody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (enabled && !isFalling && other.gameObject.CompareTag(Tag.Player))
            {
                rigBody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (enabled && other.gameObject.CompareTag(Tag.Player))
            {
                rigBody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}