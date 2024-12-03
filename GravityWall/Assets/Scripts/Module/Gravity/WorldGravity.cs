using System;
using UnityEngine;

namespace Module.Gravity
{
    /// <summary>
    /// 全体の重力を管理するクラス
    /// </summary>
    public class WorldGravity
    {
        [Flags]
        public enum Type : uint
        {
            Player = 1 << 0,
            Object = 1 << 1
        }

        public static WorldGravity Instance { get; private set; }

        /// <summary>
        /// 全体の重力
        /// </summary>
        public Vector3 Gravity { get; private set; }

        /// <summary>
        /// 全体の重力の方向
        /// </summary>
        public Vector3 Direction { get; private set; }

        private uint activeGravityMask;

        private WorldGravity()
        {
            Gravity = Vector3.down;
            Direction = Vector3.down;
            activeGravityMask = uint.MaxValue;
        }

        public static void Create()
        {
            Instance = new WorldGravity();
        }

        public static void Destroy()
        {
            Instance = null;
        }

        /// <summary>
        /// 重力を設定します
        /// </summary>
        public void SetValue(Vector3 gravity)
        {
            Gravity = gravity;
            Direction = gravity.normalized;
        }

        /// <summary>
        /// 対象のマスクの重力を有効化します
        /// </summary>
        public void SetEnable(Type mask)
        {
            activeGravityMask |= (uint)mask;
        }

        /// <summary>
        /// 対象のマスクの重力を無効化します
        /// </summary>
        public void SetDisable(Type mask)
        {
            activeGravityMask &= ~(uint)mask;
        }

        /// <summary>
        /// 対象のマスクの重力が有効かどうか
        /// </summary>
        public bool IsEnable(Type mask)
        {
            return (activeGravityMask & (uint)mask) == (uint)mask;
        }
    }
}