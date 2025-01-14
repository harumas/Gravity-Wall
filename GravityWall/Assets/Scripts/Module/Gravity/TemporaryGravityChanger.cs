using Constants;
using UnityEngine;

namespace Module.Gravity
{
    public class TemporaryGravityChanger : MonoBehaviour
    {
        [SerializeField] private float temporaryGravityMultiplier;

        private bool isPlayerEnter;
        private LocalGravity targetGravity;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                targetGravity = other.gameObject.GetComponent<LocalGravity>();
                isPlayerEnter = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                isPlayerEnter = false;
            }
        }

        private void FixedUpdate()
        {
            if (isPlayerEnter)
            {
                targetGravity.SetMultiplierAtFrame(temporaryGravityMultiplier);
            }
        }
    }
}