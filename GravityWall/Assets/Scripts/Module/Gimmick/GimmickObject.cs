using System;
using System.Text;
using R3;
using TriInspector;
using UnityEngine;

namespace Module.Gimmick
{
    public abstract class GimmickObject : MonoBehaviour
    {
        [SerializeField] protected SerializableReactiveProperty<bool> isEnabled;
        [SerializeField, ReadOnly] private string gimmickPath;

        public ReadOnlyReactiveProperty<bool> IsEnabled => isEnabled;
        public string Path => gimmickPath;

        private void OnValidate()
        {
            gimmickPath = GetGimmickPath();
        }

        private void Awake()
        {
            gimmickPath = GetGimmickPath();
        }

        public abstract void Enable(bool doEffect = true);
        public abstract void Disable(bool doEffect = true);
        public abstract void Reset();

        private string GetGimmickPath()
        {
            StringBuilder builder = new StringBuilder();
            Transform current = transform;
            const char slash = '/';

            do
            {
                builder.Insert(0, current.name + slash);
                current = current.parent;
            }
            while (current != null);

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}