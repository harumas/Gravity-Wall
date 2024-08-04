using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class BreakGlass : MonoBehaviour
    {
        [SerializeField] private float breakSpeed;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log(collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
                gameObject.SetActive(!(collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude > breakSpeed));
            }
        }
    }
}