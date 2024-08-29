using UnityEngine;

namespace Module.Gravity
{
    public class LocalGravity : MonoBehaviour
    {
        public WorldGravity.Type GravityType => gravityType;

        [SerializeField] private float multiplier = 1f;
        [SerializeField] private WorldGravity.Type gravityType;
        private Rigidbody rigBody;

        private bool isSetFrameMultiplier;
        private float frameMultiplier;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (WorldGravity.Instance.IsEnable(gravityType))
            {
                float currentMultiplier = multiplier;

                if (isSetFrameMultiplier)
                {
                    currentMultiplier = frameMultiplier;
                    isSetFrameMultiplier = false;
                }

                rigBody.AddForce(WorldGravity.Instance.Gravity * currentMultiplier, ForceMode.Acceleration);
            }
            else
            {
                rigBody.velocity = Vector3.zero;
            }
        }

        public void SetMultiplierAtFrame(float multiplier)
        {
            frameMultiplier = multiplier;
            isSetFrameMultiplier = true;
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