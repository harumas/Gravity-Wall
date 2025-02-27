using System;
using Constants;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    /// <summary>
    /// リスポーン情報を格納する構造体
    /// </summary>
    public struct RespawnContext
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 Gravity;
        public bool IsGravitySwitcherEnabled;
        public LevelResetter LevelResetter;

        public RespawnContext(
            Vector3 position,
            Quaternion rotation,
            Vector3 velocity,
            Vector3 gravity,
            LevelResetter levelResetter,
            bool canSwitchGravity
        )
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            Gravity = gravity;
            LevelResetter = levelResetter;
            IsGravitySwitcherEnabled = canSwitchGravity;
        }
    }

    /// <summary>
    /// セーブポイントを設定するコンポーネント
    /// </summary>
    public class SavePoint : MonoBehaviour
    {
        [SerializeField] private LevelResetter levelResetter;
        [SerializeField] private bool canSwitchGravity = true;
        [SerializeField] private bool canResave = false;

        public event Action<SavePoint, RespawnContext> OnEnterPoint;
        public event Action OnSaveExecuted;
        public RespawnContext LatestContext { get; private set; }

        public bool IsSaved => isSaved;
        private bool isSaved = false;

        private void Start()
        {
            levelResetter.RegisterObjects();
            levelResetter.OnResetLevel += Reset;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isSaved || !other.CompareTag(Tag.Player))
            {
                return;
            }

            Save(canSwitchGravity);
        }

        private void OnTriggerStay(Collider other)
        {
            if (isSaved || !other.CompareTag(Tag.Player))
            {
                return;
            }

            Save(canSwitchGravity);
        }

        private void Save(bool canSwitchGravity)
        {
            isSaved = true;
            LatestContext = new RespawnContext(transform.position, transform.rotation, Vector3.zero, -transform.up, levelResetter, canSwitchGravity);
            OnEnterPoint?.Invoke(this, LatestContext);
        }

        public void Reset()
        {
            if (canResave)
            {
                isSaved = false;
            }
        }
    }
}