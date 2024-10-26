using Constants;
using Module.Gravity;
using R3;
using UnityEngine;

namespace Application.Respawn
{
    public readonly struct RespawnContext
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Gravity;
        public readonly LevelResetter LevelResetter;

        public RespawnContext(Vector3 position, Quaternion rotation, Vector3 gravity, LevelResetter levelResetter)
        {
            Position = position;
            Rotation = rotation;
            Gravity = gravity;
            LevelResetter = levelResetter;
        }
    }

    public class SavePoint : MonoBehaviour
    {
        [SerializeField] private LevelResetter levelResetter;

        private readonly Subject<RespawnContext> onEnterPoint = new Subject<RespawnContext>();
        public Observable<RespawnContext> OnEnterPoint => onEnterPoint;

        private bool firstTouch = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!firstTouch && other.CompareTag(Tag.Player))
            {
                Debug.Log("セーブしました");

                firstTouch = true;
                var respawnContext = new RespawnContext(transform.position, transform.rotation, WorldGravity.Instance.Gravity, levelResetter);
                onEnterPoint.OnNext(respawnContext);
            }
        }
    }
}