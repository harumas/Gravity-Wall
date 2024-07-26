using UnityEngine;

namespace Module.Config
{
    [CreateAssetMenu(fileName = "ConfigData", menuName = "ConfigData")]
    public class ConfigData : ScriptableObject
    {
        public Vector2 Sensibility => sensibility;

        [SerializeField] private Vector2 sensibility;
    }
}