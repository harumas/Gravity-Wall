using UnityEngine;

namespace Module.Gimmick
{
    public class LocalGravity : MonoBehaviour
    {
        [SerializeField] private float multiplier = 1f;
        private Rigidbody rigBody;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            rigBody.AddForce(Gravity.Value * (multiplier), ForceMode.Acceleration);
        }
    }
}