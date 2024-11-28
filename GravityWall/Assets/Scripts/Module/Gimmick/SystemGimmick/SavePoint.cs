using System;
using Application.Sequence;
using Constants;
using Module.Gravity;
using R3;
using UnityEngine;

namespace Module.Gimmick
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

    /// <summary>
    /// セーブポイントを設定するコンポーネント
    /// </summary>
    public class SavePoint : MonoBehaviour
    {
        [SerializeField] private LevelResetter levelResetter;

        public event Action<RespawnContext> OnEnterPoint;

        private bool firstTouch = false;

        private void Start()
        {
            levelResetter.RegisterObjects();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!firstTouch && other.CompareTag(Tag.Player))
            {
                Debug.Log("セーブしました");

                firstTouch = true;
                var respawnContext = new RespawnContext(transform.position, transform.rotation, -transform.up, levelResetter);
                OnEnterPoint?.Invoke(respawnContext);
            }
        }
    }
}