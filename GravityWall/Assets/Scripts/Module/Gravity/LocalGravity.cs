using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gravity
{
    public class LocalGravity : MonoBehaviour
    {
        public WorldGravity.Type GravityType => gravityType;

        [SerializeField] private float multiplier = 1f;
        [SerializeField] private WorldGravity.Type gravityType;
        [SerializeField] private bool verticalConstraint;

        private Rigidbody rigBody;
        private bool isSetFrameMultiplier;
        private float frameMultiplier;

        private List<Vector3> verticalDirections;
        private List<RigidbodyConstraints> verticalConstraints;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();

            verticalDirections = new List<Vector3>()
            {
                transform.up,
                -transform.up,
                transform.right,
                -transform.right,
                transform.forward,
                -transform.forward
            };

            verticalConstraints = new List<RigidbodyConstraints>()
            {
                RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ,
                RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ,
                RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY
            };
        }

        private void FixedUpdate()
        {
            if (WorldGravity.Instance.IsEnable(gravityType))
            {
                Vector3 gravity = GetConstrainedGravity();
                float currentMultiplier = multiplier;
                
                if (isSetFrameMultiplier)
                {
                    currentMultiplier = frameMultiplier;
                    isSetFrameMultiplier = false;
                }

                rigBody.AddForce(gravity * currentMultiplier, ForceMode.Acceleration);
            }
            else
            {
                rigBody.velocity = Vector3.zero;
            }
        }

        private Vector3 GetConstrainedGravity()
        {
            if (verticalConstraint)
            {
                (Vector3 direction, RigidbodyConstraints constraint) = GetVerticalConstraint(WorldGravity.Instance.Gravity);
                
                rigBody.constraints = constraint;
                return direction * WorldGravity.Instance.Length;
            }

            return WorldGravity.Instance.Gravity;
        }

        private (Vector3 direction, RigidbodyConstraints constraint) GetVerticalConstraint(Vector3 gravity)
        {
            const float error = 0.05f;

            for (var i = 0; i < verticalDirections.Count; i++)
            {
                var direction = verticalDirections[i];

                if (1f - Vector3.Dot(direction, gravity) < error)
                {
                    int constraintIndex = Mathf.FloorToInt(i / 2f);
                    return (direction, verticalConstraints[constraintIndex]);
                }
            }

            return (Vector3.zero, RigidbodyConstraints.FreezeAll);
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
            Reset();
        }

        public void Reset()
        {
            rigBody.velocity = Vector3.zero;
        }
    }
}