using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Module.Gimmick.LevelGimmick;
using R3;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    /// <summary>
    /// 部屋の表示を管理するクラス
    /// </summary>
    public class LevelActivator : MonoBehaviour
    {
        [SerializeField, Header("部屋の本体オブジェクト")] private GameObject roomObject;
        [SerializeField, Header("部屋に所属するゲート")] private List<Gate> levelGates;
        [SerializeField, Header("開始時に部屋を有効化するか")] private bool startActive;
        [SerializeField, Header("入室ゲートの参照")] private string observeGate;
        [SerializeField, Header("入室ゲートを元に有効化するか")] private bool activateOnOpen;

        /// <summary>
        /// プレイヤーが入室しているか
        /// </summary>
        public bool IsPlayerEnter { get; private set; }

        /// <summary>
        /// 開始時に部屋を有効化するか
        /// </summary>
        public bool StartActive => startActive;

        /// <summary>
        /// 有効状態が変化した際に呼ばれるイベント
        /// </summary>
        public event Action<bool> OnActivateChanged;

        private void Start()
        {
            if (activateOnOpen)
            {
                GimmickReference.OnGimmickReferenceUpdated += OnGimmickReferenceUpdated;
            }

            if (startActive)
            {
                IncrementGateUsingCount();
            }

            SetActivate(startActive);

            foreach (Gate gate in levelGates)
            {
                gate.IsEnabled.Skip(1).Subscribe(SetActivate).AddTo(gate);
            }
        }

        private void OnGimmickReferenceUpdated(GimmickReference reference)
        {
            if (reference.TryGetGimmick(observeGate, out Gate gate))
            {
                gate.IsEnabled.Skip(1).Subscribe(SetActivate).AddTo(gate);
            }
        }

        public void SetActivate(bool isActivate)
        {
            if (isActivate)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        public void Activate()
        {
            if (roomObject != null)
            {
                roomObject.SetActive(true);
            }

            foreach (Gate levelGate in levelGates)
            {
                levelGate.gameObject.SetActive(true);
            }

            OnActivateChanged?.Invoke(true);
        }

        public void Deactivate()
        {
            bool allDisabled = levelGates.All(g => !g.IsEnabled.CurrentValue);

            if (!allDisabled || IsPlayerEnter)
            {
                return;
            }

            if (roomObject != null)
            {
                roomObject.SetActive(false);
            }

            foreach (Gate levelGate in levelGates)
            {
                if (!levelGate.IsUsing)
                {
                    levelGate.gameObject.SetActive(false);
                }
            }

            OnActivateChanged?.Invoke(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                IsPlayerEnter = true;

                if (!startActive)
                {
                    IncrementGateUsingCount();
                }

                startActive = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                IsPlayerEnter = false;
                DecrementGateUsingCount();
            }
        }

        private void IncrementGateUsingCount()
        {
            foreach (Gate gate in levelGates)
            {
                gate.UsingCount++;
            }
        }

        private void DecrementGateUsingCount()
        {
            foreach (Gate gate in levelGates)
            {
                gate.UsingCount--;
            }
        }
    }
}