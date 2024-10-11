using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

namespace Module.Gimmick
{
    public class BreakFloor : MonoBehaviour
    {
        [SerializeField] private GameObject floor, breakFloor, item;
        [SerializeField] private AudioSource audioSource;
        private bool wasBreak;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player) && !wasBreak)
            {
                floor.SetActive(false);
                item.SetActive(false);
                breakFloor.SetActive(true);
                audioSource.Play();
                wasBreak = true;
            }
        }
    }
}