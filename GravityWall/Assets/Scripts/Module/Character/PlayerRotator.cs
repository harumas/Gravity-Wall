using Core.Input;
using UnityEngine;

namespace Module.Character
{
    public class PlayerRotator : MonoBehaviour
    {
        [Header("Playerの矢印キー回転速度")]
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float damping;

        private InputEvent rotateEvent;
        private Rigidbody rigBody;

        private void Start()
        {
            rotateEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Look);
            rigBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Quaternion target = Quaternion.AngleAxis(rotateEvent.ReadValue<float>() * rotateSpeed, transform.up) * rigBody.rotation;
            rigBody.rotation = Quaternion.Slerp(rigBody.rotation, target, damping);
        }
    }
}