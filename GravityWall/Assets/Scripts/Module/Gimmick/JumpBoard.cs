using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Module.Character;
using UnityEngine;
namespace Module.Gimmick
{
    public class JumpBoard : MonoBehaviour
    {
        [Header("ジャンプ力")][SerializeField] private float jumpPower;

        [Header("ジャンプ中の重力")][SerializeField] private float jumpingGravity;
        [SerializeField, Tag] private string[] targetTags;

        private void OnCollisionEnter(Collision collision)
        {
            if (targetTags.Any(tag => collision.gameObject.CompareTag(tag)))
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<PlayerController>().BoadJump(transform.up * jumpPower, jumpingGravity);
                }
                else
                {
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * jumpPower, ForceMode.VelocityChange);
                }
            }
        }
    }
}