using System;
using R3;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    [Serializable]
    public class PowerObserver
    {
        [SerializeField, Header("最低いくつの動力が入ったら出力するか")] private int minPowerCount = 1;
        [SerializeField, Header("動力を検知するするギミック")] private GimmickObject[] observedSwitches;
        [SerializeField] private bool isPowered;

        // 現在動力が入っている数
        private int powerCount = 0;
        private GimmickObject targetGimmick;

        public void StartObserve(GimmickObject targetGimmick)
        {
            if (targetGimmick == null)
            {
                Debug.LogError("対象のギミックがnullです");
                return;
            }

            this.targetGimmick = targetGimmick;

            // 動力を必要としなかったら直ちに送信
            if (minPowerCount <= 0)
            {
                Send(true);
                return;
            }

            foreach (GimmickObject gimmick in observedSwitches)
            {
                gimmick.IsEnabled.Skip(1).Subscribe(UpdatePowerCount).AddTo(targetGimmick);
            }
        }

        private void UpdatePowerCount(bool isGimmickEnabled)
        {
            // 動力をカウント
            powerCount += isGimmickEnabled ? 1 : -1;
            bool doSendPower = powerCount >= minPowerCount;

            // 既に動力を送信していたら送信しない
            if (isPowered == doSendPower)
            {
                return;
            }

            // 動力を送信
            Send(doSendPower);
        }

        private void Send(bool doSendPower)
        {
            if (doSendPower)
            {
                targetGimmick.Enable();
            }
            else
            {
                targetGimmick.Disable();
            }

            isPowered = doSendPower;
        }
    }
}