using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module.Character
{
    [Serializable]
    public class PlayerControlParameter
    {
        [Header("加速度"), SerializeField] private float acceleration;
        [Header("最大速度"), SerializeField] private float maxSpeed;
        [Header("速度減衰率"), SerializeField] private float speedDamping;

        [Header("ジャンプ力"), SerializeField] private float jumpPower;
        [Header("ジャンプ中の重力"), SerializeField] private float jumpingGravity;
        [Header("ジャンプ中移動係数"), SerializeField] private float airControl;
        [Header("連続ジャンプを許可する間隔"), SerializeField] private float allowJumpInterval;
        [Header("ジャンプを許可する地面との距離"), SerializeField] private float allowJumpDistance;
        [Header("接地判定を許可する間隔"), SerializeField] private float allowLandingInteval;
        [Header("追加ジャンプ量を加算していくカーブ"), SerializeField] private AnimationCurve additionalJumpCurve;
        [Header("追加ジャンプ量の係数"), SerializeField] private float additionalJumpMultiplier;
        [Header("カメラの向きを軸に回転する角度"), SerializeField] private float cameraAxisRotateAngle;

        [Header("回転のイージング係数"), SerializeField] private float rotateStep;
        [Header("回転のイージング時間係数"), SerializeField] private float rotateTime;
        [Header("回転のイージングオフセット"), SerializeField] private float easingOffset;
        [Header("回転中とみなす角度"), SerializeField] private float rotatingAngle;

        public float Acceleration => acceleration;
        public float MaxSpeed => maxSpeed;
        public float SpeedDamping => speedDamping;

        public float JumpPower => jumpPower;
        public float JumpingGravity => jumpingGravity;
        public float AirControl => airControl;

        public float AllowJumpInterval => allowJumpInterval;
        public float AllowJumpDistance => allowJumpDistance;
        public float AllowLandingInterval => allowLandingInteval;
        public float CameraAxisRotateAngle => cameraAxisRotateAngle;

        public float RotateStep => rotateStep;
        public float RotateTime => rotateTime;
        public float EasingOffset => easingOffset;
        public float RotatingAngle => rotatingAngle;

        public float GetAdditionalJumpPower(float time)
        {
            return additionalJumpCurve.Evaluate(time) * additionalJumpMultiplier;
        }
    }
}