using Constants;
using Module.Gimmick.SystemGimmick;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class BreakFloor : MonoBehaviour
    {
        [SerializeField] private GameObject floor, breakFloor, item;
        [SerializeField] private GravitySwitchTrigger trigger1, trigger2;
        [SerializeField] private AudioSource audioSource;
        private bool wasBreak;

        void Start()
        {
            trigger1.SetEnable(false);
            trigger2.SetEnable(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player) && !wasBreak)
            {
                floor.SetActive(false);
                item.SetActive(false);
                breakFloor.SetActive(true);
                trigger1.SetEnable(true);
                trigger2.SetEnable(true);
                audioSource.Play();
                wasBreak = true;
            }
        }
    }
}