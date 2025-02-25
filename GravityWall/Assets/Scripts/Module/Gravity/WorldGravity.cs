using System;
using UnityEngine;

namespace Module.Gravity
{
    public class WorldGravity
    {
        [Flags]
        public enum Type : uint
        {
            Player = 1 << 0,
            Object = 1 << 1
        }

        public static WorldGravity Instance { get; private set; }

        public Vector3 Gravity { get; private set; }
        public float Length { get; private set; }
        public Vector3 Direction { get; private set; }
        private uint activeGravityMask;

        private WorldGravity()
        {
            Gravity = Vector3.down;
            Length = 1f;
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

        public void SetValue(Vector3 gravity)
        {
            Gravity = gravity;
            Length = gravity.magnitude;
            Direction = gravity / Length;
        }

        public void SetEnable(Type mask)
        {
            activeGravityMask |= (uint)mask;
        }

        public void SetDisable(Type mask)
        {
            activeGravityMask &= ~(uint)mask;
        }

        public bool IsEnable(Type mask)
        {
            return (activeGravityMask & (uint)mask) == (uint)mask;
        }
    }
}