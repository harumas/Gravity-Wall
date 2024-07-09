using Core.Helper;
using Core.Input;
using UnityEngine;

namespace Module.Character
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform pivotVertical;
        [SerializeField] private Transform pivotHorizontal;
        [SerializeField] private float horizontalSensibility;
        [SerializeField] private float verticalSensibility;
        [SerializeField] private MinMaxValue horizontalRange;
        [SerializeField] private MinMaxValue verticalRange;

        private InputEvent mouseEvent;
        private Vector2 mouseDelta;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            mouseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Look);
        }

        private void Update()
        {
            mouseDelta = mouseEvent.ReadValue<Vector2>();
        }

        private void LateUpdate()
        {
            float x = mouseDelta.y * verticalSensibility * Time.deltaTime;
            float y = mouseDelta.x * horizontalSensibility * Time.deltaTime;

            float eulerX = pivotHorizontal.localEulerAngles.x;
            float eulerY = pivotVertical.localEulerAngles.y;

            eulerX = ClampAngle(eulerX - x, horizontalRange.Min, horizontalRange.Max);
            eulerY = ClampAngle(eulerY + y, verticalRange.Min, verticalRange.Max);

            pivotVertical.localEulerAngles = new Vector3(0f, eulerY, 0f);
            pivotHorizontal.localEulerAngles = new Vector3(eulerX, eulerY, 0f);

            mouseDelta = Vector2.zero;
        }

        private float ClampAngle(float angle, float from, float to)
        {
            if (angle < 0f)
            {
                angle = 360 + angle;
            }

            if (angle > 180f)
            {
                return Mathf.Max(angle, 360 + from);
            }

            return Mathf.Min(angle, to);
        }
    }
}