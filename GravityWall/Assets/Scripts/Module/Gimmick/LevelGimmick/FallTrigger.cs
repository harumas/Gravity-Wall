using Module.Gravity;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick
{
    public class FallTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject fallColliuder;
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    other.GetComponent<GravitySwitcher>().Disable();
                    fallColliuder.SetActive(true);
                }
            }
        }
    }
}