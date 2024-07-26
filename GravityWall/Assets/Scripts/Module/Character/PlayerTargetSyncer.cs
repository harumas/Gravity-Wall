using System;
using Core.Input;
using Module.Core.Input;
using Module.InputModule;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module.Character
{
    /// <summary>
    /// 移動方向にプレイヤーの体を回転させるクラス
    /// </summary>
    public class PlayerTargetSyncer : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform axis;
        [SerializeField] private float damping;

        private Vector2 input;

        private void Start()
        {
            GameInput.Move.Subscribe(value => input = value).AddTo(this);
        }

        public Vector3 GetTargetDirection()
        {
            Vector3 inputDirection = target.rotation * new Vector3(input.x, 0f, input.y);
            Vector3 planedDirection = Vector3.ProjectOnPlane(inputDirection, axis.up);

            return planedDirection;
        }

        private void FixedUpdate()
        {
            PerformBodyRotate();
        }

        private void PerformBodyRotate()
        {
            if (input == Vector2.zero)
            {
                return;
            }

            //目標の回転方向を算出
            Vector3 targetDirection = GetTargetDirection();
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, axis.up);
            
            //補完して回転
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, damping);
        }
    }
}