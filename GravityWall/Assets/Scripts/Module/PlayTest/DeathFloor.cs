using UnityEngine;

namespace Module.PlayTest
{
    public class DeathFloor : MonoBehaviour
    {
        private RespawnManager respawnManager => RespawnManager.instance;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                respawnManager.Respawn();
            }
        }
    }
}