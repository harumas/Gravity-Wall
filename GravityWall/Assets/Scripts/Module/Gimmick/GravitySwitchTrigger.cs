using System.Collections;
using System.Collections.Generic;
using Module.Character;
using UnityEngine;

namespace Module.Gimmick
{
    public class GravitySwitchTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<GravitySwitcher>().Disable();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<GravitySwitcher>().Enable();
            }
        }
    }
}