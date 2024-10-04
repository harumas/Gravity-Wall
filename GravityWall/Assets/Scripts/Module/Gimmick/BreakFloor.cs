using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class BreakFloor : MonoBehaviour
    {
        [SerializeField] private GameObject floor, breakFloor, item;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                floor.SetActive(false);
                item.SetActive(false);
                breakFloor.SetActive(true);
            }
        }
    }
}