using Module.Character;
using Module.Gravity;
using UnityEngine;

namespace Module.Gimmick
{
    public class FallTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject fallColliuder;
        [SerializeField] private GravitySwitchTrigger[] gravitySwitchTriggers;
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    foreach (var trigger in gravitySwitchTriggers)
                    {
                        trigger.SetEnable(false);
                    }

                    other.GetComponent<GravitySwitcher>().Disable();
                    fallColliuder.SetActive(true);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    GetComponent<GravitySwitchTrigger>().SetEnable(false);
                    other.GetComponent<GravitySwitcher>().Disable();
                }
            }
        }
    }
}