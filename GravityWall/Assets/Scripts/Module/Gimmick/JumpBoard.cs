using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Module.Gimmick
{
    public class JumpBoard : MonoBehaviour
    {
        [SerializeField] private float jumpPower;

        //TODO: Tagをプルダウンで選べるように
        [SerializeField, Tag] private string[] targetTags;

        private void OnTriggerEnter(Collider collider)
        {
            if (targetTags.Any(tag => collider.CompareTag(tag)))
            {
                collider.GetComponent<Rigidbody>().AddForce(transform.up * jumpPower, ForceMode.VelocityChange);
            }
        }
    }
}