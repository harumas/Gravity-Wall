using Application.Sequence;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class HubSpawnPoint : MonoBehaviour
    {
        [SerializeField] private LevelResetter levelResetter;
        [SerializeField] private Vector3 respawnVelocity;

        public RespawnContext GetContext()
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 gravity = -transform.up;
            return new RespawnContext(position, rotation, respawnVelocity, gravity, levelResetter);
        }
    }
}