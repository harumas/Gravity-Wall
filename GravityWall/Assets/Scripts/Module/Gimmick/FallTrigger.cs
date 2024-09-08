using Module.Character;
using Module.Gravity;
using UnityEngine;

namespace Module.Gimmick
{
    public class FallTrigger : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    other.GetComponent<GravitySwitcher>().Disable();
                }
            }
        }
    }
}