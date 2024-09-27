using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreModule.Input.Editor
{
    [CreateAssetMenu(menuName = "InputActionAssetProfile", fileName = "InputActionAssetProfile")]
    public class InputActionAssetProfile : ScriptableObject
    {
        [SerializeField] private InputActionAsset inputActionAsset;

        public InputActionAsset GetUsingAsset()
        {
            return inputActionAsset;
        }
    }
}