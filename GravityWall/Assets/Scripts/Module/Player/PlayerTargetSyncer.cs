﻿using UnityEngine;

namespace Module.Player
{
    /// <summary>
    /// 移動方向にプレイヤーの体を回転させるクラス
    /// </summary>
    public class PlayerTargetSyncer : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform axis;
        [SerializeField] private float damping;

        private Vector2 moveInput;
        public bool HoldLock { get; set; }

        public void OnMoveInput(Vector2 moveInput)
        {
            this.moveInput = moveInput;
        }

        public void SetRotation(Quaternion targetRotation)
        {
            Vector3 planedDirection = Vector3.ProjectOnPlane(targetRotation * Vector3.forward, axis.up);
            transform.rotation = Quaternion.LookRotation(planedDirection, axis.up);
        }

        public Vector3 GetTargetDirection()
        {
            Vector3 inputDirection = target.rotation * new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 planedDirection = Vector3.ProjectOnPlane(inputDirection, axis.up);

            return planedDirection;
        }

        private void FixedUpdate()
        {
            PerformBodyRotate();
        }

        private void PerformBodyRotate()
        {
            if (moveInput == Vector2.zero)
            {
                return;
            }

            //目標の回転方向を算出
            Vector3 targetDirection = GetTargetDirection();

            if (targetDirection == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, axis.up);

            //補完して回転
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, damping);
        }

        public void Reset()
        {
            moveInput = Vector2.zero;
        }

        public void Lock()
        {
            Reset();
            enabled = false;
        }

        public void Unlock()
        {
            if (HoldLock)
            {
                return;
            }

            enabled = true;
        }
    }
}