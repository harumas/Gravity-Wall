using Constants;
using Module.Character;
using UnityEngine;

namespace Module.PlayTest
{
    public class DeathFloor : MonoBehaviour
    {
        private RespawnManager respawnManager => RespawnManager.instance;
        private PlayerController player;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tag.Player))
            {
                if (player == null)
                {
                    player = collision.gameObject.GetComponent<PlayerController>();
                }

                player.Death();

                Invoke("DeathFalse", 1.5f);

                respawnManager.Damage();
            }
        }

        void DeathFalse()
        {
            player.Respawn();
        }
    }
}