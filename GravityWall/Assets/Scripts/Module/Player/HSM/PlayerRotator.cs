using Module.Gravity;
using UnityEngine;

namespace Module.Player.HSM
{
    public class PlayerRotator
    {
        public bool Enabled { get; set; }

        private readonly PlayerControlParameter parameter;
        private readonly PlayerControlContext controlContext;
        private readonly PlayerControlEvent controlEvent;
        private readonly PlayerComponent component;
        private readonly Camera mainCamera;

        public PlayerRotator(
            PlayerControlParameter parameter,
            PlayerControlContext controlContext,
            PlayerControlEvent controlEvent,
            PlayerComponent component)
        {
            this.parameter = parameter;
            this.controlContext = controlContext;
            this.controlEvent = controlEvent;
            this.component = component;
            mainCamera = Camera.main;
        }

        public void Update()
        {
            if (Enabled)
            {
                PerformGravityRotate();
            }
        }

        private void PerformGravityRotate()
        {
            WorldGravity worldGravity = WorldGravity.Instance;
            Rigidbody rigidbody = component.RigidBody;
            Transform transform = component.Transform;

            Vector3 gravity = worldGravity.Gravity;
            Quaternion currentRotation = rigidbody.rotation;

            // 自分と重力の角度の差を求める
            float angle = Vector3.Angle(transform.up, -gravity);
            angle = Mathf.Max(angle, Mathf.Epsilon);

            Quaternion targetRotation;

            // カメラのZ軸で回転する(天井に張り付く等)
            if (angle >= parameter.CameraAxisRotateAngle)
            {
                targetRotation = Quaternion.AngleAxis(angle, mainCamera.transform.forward) * currentRotation;
            }
            // 重力に合わせて回転する
            else
            {
                targetRotation = Quaternion.FromToRotation(transform.up, -gravity) * currentRotation;
            }

            if (angle <= Mathf.Epsilon)
            {
                controlContext.IsLastRotating = false;
                controlEvent.IsRotating.Value = false;
                controlEvent.RotatingAngle.Value = angle;
            }

            bool isRotating = angle - parameter.RotateStep >= parameter.RotatingAngle;
            if (!controlContext.IsLastRotating)
            {
                controlContext.LastRotateTime = Time.time;
            }

            controlContext.IsLastRotating = true;

            // Out-Sineで補間する
            float time = Time.time - controlContext.LastRotateTime;
            float t = parameter.RotateStep * (Mathf.Sin(time / parameter.RotateTime * Mathf.PI * 0.5f) + 1.0f) + parameter.EasingOffset;

            if (isRotating)
            {
                rigidbody.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, t);
            }
            else
            {
                // 回転がほぼ完了したら回転の目標に合わせる
                rigidbody.rotation = targetRotation;
            }

            controlEvent.IsRotating.Value = isRotating;
            controlEvent.RotatingAngle.Value = angle;
        }
    }
}