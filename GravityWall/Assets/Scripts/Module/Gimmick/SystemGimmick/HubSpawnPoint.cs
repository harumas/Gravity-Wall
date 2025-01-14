using System;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class HubSpawnPoint : MonoBehaviour
    {
        [SerializeField] private LevelResetter levelResetter;
        [SerializeField] private Vector3 respawnVelocity;
        [SerializeField] private bool enableOnStart;

        public event Action<RespawnContext> OnEnabled;

        private void Start()
        {
            if (enableOnStart)
            {
                Enable();
            }
        }

        public void Enable()
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 gravity = -transform.up;
            var context = new RespawnContext(position, rotation, respawnVelocity, gravity, levelResetter, true);
            
            OnEnabled?.Invoke(context);
        }
    }
}