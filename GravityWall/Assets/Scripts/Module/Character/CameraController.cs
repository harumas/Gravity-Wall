using CoreModule.Helper;
using UnityEngine;

namespace Module.Character
{
    /// <summary>
    /// カメラの回転を制御するクラス
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("ロールピッチの回転軸")]
        [SerializeField]
        private Transform pivotVertical;

        [SerializeField] private Transform pivotHorizontal;
        [SerializeField] private MinMaxValue horizontalRange;
        [SerializeField] private MinMaxValue verticalRange;
        [SerializeField] private bool isFreeCamera = true;

        private void Start()
        {
            //カーソルロック
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void OnRotateCameraInput(Vector2 mouseDelta)
        {
            if (!isFreeCamera)
            {
                return;
            }
            
            float dx = mouseDelta.x;
            float dy = mouseDelta.y;

            Vector3 localEulerAngles = pivotHorizontal.localEulerAngles;
            float eulerX = localEulerAngles.x;
            float eulerY = localEulerAngles.y;

            //回転を制限
            eulerX = ClampAngle(eulerX - dy, horizontalRange.Min, horizontalRange.Max);
            eulerY = ClampAngle(eulerY + dx, verticalRange.Min, verticalRange.Max);

            pivotHorizontal.localEulerAngles = new Vector3(eulerX, eulerY, 0f);
        }
        
        public void SetCameraRotation(Quaternion rotation)
        {
            pivotHorizontal.rotation = rotation;
        }

        public void SetFreeCamera(bool isFreeCamera)
        {
            this.isFreeCamera = isFreeCamera;
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