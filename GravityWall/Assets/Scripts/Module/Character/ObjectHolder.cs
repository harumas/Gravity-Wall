using Core.Input;
using CoreModule.Input;
using Module.Gravity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Module.Character
{
    public class ObjectHolder : MonoBehaviour
    {
        private Rigidbody myBody;
        private Collider stayingCollider;

        private FixedJoint dependingJoint;
        private LocalGravity dependingGravity;
        private Rigidbody dependingBody;
        private SpaceConstraintObject dependingConstraint;
        private float originalMass;

        private InputEvent holdEvent;

        private void Start()
        {
            myBody = transform.parent.GetComponent<Rigidbody>();

            holdEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Hold);
            holdEvent.Started += OnHoldStart;
            holdEvent.Canceled += OnHoldEnd;
        }

        private void OnHoldStart(InputAction.CallbackContext ctx)
        {
            if (stayingCollider == null)
            {
                return;
            }

            dependingGravity = stayingCollider.GetComponent<LocalGravity>();
            Gravity.WorldGravity.Type gravityType = dependingGravity.GravityType;

            if (gravityType != Gravity.WorldGravity.Type.Object)
            {
                return;
            }

            dependingConstraint = stayingCollider.gameObject.GetComponentInChildren<SpaceConstraintObject>();
            dependingConstraint.Disable();

            //RigidBodyのセットアップ
            dependingBody = stayingCollider.gameObject.GetComponent<Rigidbody>();
            dependingBody.constraints = RigidbodyConstraints.None;
            dependingBody.isKinematic = false;
            originalMass = dependingBody.mass;
            dependingBody.mass = 1f;

            dependingGravity.Disable();

            dependingJoint = stayingCollider.gameObject.AddComponent<FixedJoint>();
            dependingJoint.connectedBody = myBody;
        }

        private void OnHoldEnd(InputAction.CallbackContext ctx)
        {
            if (dependingJoint != null)
            {
                Destroy(dependingJoint);
                dependingGravity.Enable();
                dependingConstraint.Enable();

                //RigidBodyのセットアップ
                dependingBody.constraints = RigidbodyConstraints.FreezeRotation;
                dependingBody.mass = originalMass;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            stayingCollider = other.transform.parent.GetComponent<Collider>();
        }

        private void OnTriggerExit(Collider other)
        {
            stayingCollider = null;
        }
    }
}