using UnityEngine;

namespace Module.Player
{
    public class OnCollisionSEPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private void OnCollisionEnter(Collision collision)
        {
            audioSource.Play();
        }
    }
}
