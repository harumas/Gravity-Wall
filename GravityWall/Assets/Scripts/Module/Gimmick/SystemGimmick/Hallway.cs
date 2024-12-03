using System.Collections.Generic;
using System.Linq;
using Constants;
using Module.Gimmick.LevelGimmick;
using R3;
using TriInspector;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class Hallway : MonoBehaviour
    {
        [SerializeField, Header("廊下のオブジェクト")] private GameObject hallwayBody;
        [SerializeField, Header("廊下に隣接しているゲートの参照")] private List<string> gimmickObjects;
        [SerializeField, ReadOnly] private List<Gate> referencedGates;

        private bool isPlayerEnter;

        private void Start()
        {
            GimmickReference.OnGimmickReferenceUpdated += OnGimmickReferenceUpdated;
        }

        private void OnGimmickReferenceUpdated(GimmickReference reference)
        {
            foreach (string path in gimmickObjects)
            {
                // 登録されたギミックの参照を取得
                if (reference.TryGetGimmick(path, out Gate gate))
                {
                    referencedGates.Add(gate);
                }
                else
                {
                    Debug.LogWarning("Gimmick not found: " + path);
                }

                // 廊下に隣接するゲートを登録
                foreach (Gate referencedGate in referencedGates)
                {
                    referencedGate.IsEnabled
                        .Skip(1)
                        .Subscribe(isEnabled => OnGateAction(isEnabled))
                        .AddTo(this);
                }
            }

            Disable();
        }

        private void OnGateAction(bool isEnabled)
        {
            // ゲートが開いたら廊下も表示する
            if (isEnabled)
            {
                Enable();
            }
            // 扉が閉じてプレイヤーが入ってなかったら無効化
            else if (!isPlayerEnter)
            {
                Disable();
            }
        }

        private void Enable()
        {
            hallwayBody.SetActive(true);

            foreach (Gate otherGate in referencedGates)
            {
                otherGate.gameObject.SetActive(true);
            }
        }

        private void Disable()
        {
            hallwayBody.SetActive(false);

            // 他の部屋から使用されていないゲートを無効化
            foreach (Gate otherGate in referencedGates.Where(otherGate => !otherGate.IsUsing))
            {
                otherGate.Disable(false);
                otherGate.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Tag.Player))
            {
                return;
            }

            // プレイヤーが入室してきたら使用カウントを増やす
            isPlayerEnter = true;

            foreach (Gate gate in referencedGates)
            {
                gate.UsingCount++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(Tag.Player))
            {
                return;
            }

            // プレイヤーが退室したら使用カウントを減らす
            isPlayerEnter = false;

            foreach (Gate gate in referencedGates)
            {
                gate.UsingCount--;
            }
        }
    }
}