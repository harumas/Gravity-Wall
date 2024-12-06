using UnityEngine;

namespace Module.PlayTest
{
    public class RegenerationBoxCollider : MonoBehaviour
    {
        [SerializeField] private Transform regenerationPosition;

        private void OnTriggerEnter(Collider other)
        {
            other.transform.position = regenerationPosition.position;
        }
    }
}

