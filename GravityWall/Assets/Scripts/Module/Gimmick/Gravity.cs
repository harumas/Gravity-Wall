using UnityEditor;
using UnityEngine;

namespace Module.Gimmick
{
    public static class Gravity
    {
        public static float GravityMultiplier = -800f;
        public static Vector3 Value { get; private set; } = Physics.gravity * -GravityMultiplier;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    Value = Physics.gravity * -GravityMultiplier;
                }
            };
        }

        public static void SetDir(Vector3 value)
        {
            Value = value * GravityMultiplier;
        }
    }
}