using Core.Helper;
using Core.Input;
using Module.Core.Input;
using Module.InputModule;
using R3;
using R3.Triggers;
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

        private void Start()
        {
            //カーソルロック
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //マウスの移動イベントを登録
            this.LateUpdateAsObservable()
                .Select(_ => GameInput.MouseDelta * Time.deltaTime)
                .Subscribe(OnRotateCamera);
        }

        private void OnRotateCamera(Vector2 mouseDelta)
        {
            float dx = mouseDelta.x;
            float dy = mouseDelta.y;

            float eulerX = pivotHorizontal.localEulerAngles.x;
            float eulerY = pivotVertical.localEulerAngles.y;

            //回転を制限
            eulerX = ClampAngle(eulerX - dy, horizontalRange.Min, horizontalRange.Max);
            eulerY = ClampAngle(eulerY + dx, verticalRange.Min, verticalRange.Max);

            pivotVertical.localEulerAngles = new Vector3(0f, eulerY, 0f);
            pivotHorizontal.localEulerAngles = new Vector3(eulerX, eulerY, 0f);
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