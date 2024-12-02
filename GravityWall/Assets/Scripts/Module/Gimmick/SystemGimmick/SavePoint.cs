using System;
using Application.Sequence;
using Constants;
using UnityEngine;

namespace Module.Gimmick
{
    /// <summary>
    /// リスポーン情報を格納する構造体
    /// </summary>
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
                LatestContext = new RespawnContext(transform.position, transform.rotation, -transform.up, levelResetter);
                OnEnterPoint?.Invoke(LatestContext);
            }
        }
    }
}