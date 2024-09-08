using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Module.Gravity;
namespace Module.Gimmick
{
    public class BreakGlass : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    director.Play();
                }
            }
        }
    }
}