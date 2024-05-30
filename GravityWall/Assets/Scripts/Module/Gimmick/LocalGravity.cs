using UnityEngine;

namespace Module.Gimmick
{
    public class LocalGravity : MonoBehaviour
    {
        public Gravity.Type GravityType => gravityType;
        
        [SerializeField] private float multiplier = 1f;
        [SerializeField] private Gravity.Type gravityType;
        private Rigidbody rigBody;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();
        }
        
        private void FixedUpdate()
        {
            if (Gravity.IsEnable(gravityType))
            {
                rigBody.AddForce(Gravity.Value * (multiplier), ForceMode.Acceleration);
            }
            else
            {
                rigBody.velocity = Vector3.zero;
            }
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            rigBody.velocity = Vector3.zero;
        }
    }
}