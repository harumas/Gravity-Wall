using System;
using Constants;
using Module.Player;
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

        public RespawnContext(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 gravity, LevelResetter levelResetter,
            bool canSwitchGravity)
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

        public event Action<RespawnContext> OnEnterPoint;
        public RespawnContext LatestContext { get; private set; }

        public bool IsSaved => isSaved;
        private bool isSaved = false;

        private void Start()
        {
            levelResetter.RegisterObjects();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isSaved && other.CompareTag(Tag.Player))
            {
                Debug.Log("セーブしました");
                isSaved = true;

                GravitySwitcher gravitySwitcher = other.GetComponent<GravitySwitcher>();
                bool canSwitchGravity = gravitySwitcher.IsEnabled;
                LatestContext = new RespawnContext(transform.position,
                    transform.rotation,
                    Vector3.zero,
                    -transform.up,
                    levelResetter,
                    canSwitchGravity);
                
                OnEnterPoint?.Invoke(LatestContext);
            }
        }
    }
}