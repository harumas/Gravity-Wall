using UnityEngine;

namespace Module.InputModule
{
    [CreateAssetMenu(menuName = "InputValueAdjustParameter", fileName = "InputValueAdjustParameter")]
    public class InputValueAdjustParameter : ScriptableObject
    {
        [SerializeField] private float padSensitivity = 20f;
        [SerializeField] private float mouseSensitivity = 100f;

        public float PadSensitivity => padSensitivity;
        public float MouseSensitivity => mouseSensitivity;
    }
}