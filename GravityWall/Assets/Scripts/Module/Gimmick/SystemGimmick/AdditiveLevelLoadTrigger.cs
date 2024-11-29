using System;
using System.Collections.Generic;
using Constants;
using CoreModule.Helper.Attribute;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    /// <summary>
    /// 追加シーン読み込みを行うための判定トリガー
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class AdditiveLevelLoadTrigger : MonoBehaviour
    {
        [SerializeField, Header("追加シーン読み込み後にアクティブにするシーン名")]
        private SceneField mainScene;

        [SerializeField, Header("読み込むシーン名")] private List<SceneField> levelReference;
        [SerializeField, Header("トリガーによる読み込みを行うか")] private bool triggerDetect;
        [SerializeField] private bool isTouched;

        /// <summary>
        /// シーン読み込みが行われた際に呼ばれるイベント
        /// </summary>
        public event Action OnSceneLoaded;

        /// <summary>
        /// シーン読み込みを要求された際に呼ばれるイベント
        /// </summary>
        public event Action<SceneField, List<SceneField>> OnLoadRequested;

        public void Load()
        {
            OnLoadRequested?.Invoke(mainScene, levelReference);
        }

        public void CallLoaded()
        {
            OnSceneLoaded?.Invoke();
        }

        public void Reset()
        {
            isTouched = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            bool isTriggerDetected = triggerDetect && !isTouched;
            
            // プレイヤーがトリガーに触れた場合
            if (isTriggerDetected && other.CompareTag(Tag.Player))
            {
                Load();
                isTouched = true;
            }
        }
    }
}