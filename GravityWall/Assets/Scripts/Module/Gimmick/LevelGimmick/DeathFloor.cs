using System;
using Constants;
using UnityEngine;
using static Module.Player.PlayerController;

namespace Module.Gimmick.LevelGimmick
{
    /// <summary>
    /// 死亡通知を送信するコンポーネント
    /// </summary>
    public class DeathFloor : MonoBehaviour
    {
        [SerializeField] private DeathType floorType;

        public event Action<DeathType> OnEnter;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tag.Player))
            {
                OnEnter?.Invoke(floorType);
            }
        }
    }
}