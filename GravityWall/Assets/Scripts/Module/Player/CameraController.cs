using CoreModule.Helper;
using UnityEngine;

namespace Module.Player
{
    /// <summary>
    /// カメラの回転を制御するクラス
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("ロールピッチの回転軸")][SerializeField] private Transform pivotHorizontal;
        [SerializeField] private MinMaxValue horizontalRange;
        [SerializeField] private MinMaxValue verticalRange;
        [SerializeField] private bool isFreeCamera = true;

        public void OnRotateCameraInput(Vector2 mouseDelta)
        {
            if (!isFreeCamera || Cursor.lockState != CursorLockMode.Locked)
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

        private float rotationDuration = 0.2f; // アニメーションの持続時間（秒）
        private bool isRotating = false; // 現在回転中かどうかを判定

        public System.Collections.IEnumerator RotateCamera(Vector3 axis, float angle)
        {
            isRotating = true;

            Quaternion startRotation = pivotHorizontal.rotation;
            Quaternion endRotation = startRotation * Quaternion.AngleAxis(angle, axis);

            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                pivotHorizontal.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null; // 次のフレームまで待機
            }

            // 回転終了後に90度単位で丸める
            Vector3 finalEulerAngles = pivotHorizontal.rotation.eulerAngles;
            finalEulerAngles.y = Mathf.Round(finalEulerAngles.y / 90) * 90; // Y軸を90度単位で丸める
            finalEulerAngles.x = Mathf.Round(finalEulerAngles.x / 90) * 90; // 必要に応じてX軸も丸める
            finalEulerAngles.z = Mathf.Round(finalEulerAngles.z / 90) * 90; // 必要に応じてZ軸も丸める

            pivotHorizontal.rotation = Quaternion.Euler(finalEulerAngles); // 丸めた角度で再設定
            isRotating = false;
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