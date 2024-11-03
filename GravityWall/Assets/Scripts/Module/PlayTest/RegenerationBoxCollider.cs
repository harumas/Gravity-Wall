using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.PlayTest
{
    public class RegenerationBoxCollider : MonoBehaviour
    {
        [SerializeField] private Transform RegenerationPosition;

        private void OnTriggerEnter(Collider other)
        {
            other.transform.position = RegenerationPosition.position;
        }
    }
}

