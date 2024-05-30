using System;
using UnityEditor;
using UnityEngine;

namespace Module.Gimmick
{
    public static class Gravity
    {
        [Flags]
        public enum Type : uint
        {
            Player = 1 << 0,
            Object = 1 << 1
        }

        public static Vector3 Value { get; private set; }
        private static uint activeGravityMask;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    Value = Vector3.down;
                    activeGravityMask = uint.MaxValue;
                }
            };
        }

        public static void SetValue(Vector3 gravity)
        {
            Value = gravity;
        }

        public static void SetEnable(Type mask)
        {
            activeGravityMask |= (uint)mask;
        }

        public static void SetDisable(Type mask)
        {
            activeGravityMask |= ~(activeGravityMask & (uint)mask);
        }

        public static bool IsEnable(Type mask)
        {
            return (activeGravityMask & (uint)mask) == (uint)mask;
        }
    }
}