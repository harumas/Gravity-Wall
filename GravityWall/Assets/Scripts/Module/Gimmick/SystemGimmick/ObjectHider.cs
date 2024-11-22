using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Module.Gimmick
{
    [DefaultExecutionOrder(10)]
    public class ObjectHider : MonoBehaviour
    {
        [Header("オブジェクトを隠す角度")]
        [SerializeField]
        private float hideAngle = 80f;

        [SerializeField] private bool activeOnStart;

        private Camera mainCamera;
        private List<Renderer> renderers = new List<Renderer>();
        private ReactiveProperty<bool> isHide = new ReactiveProperty<bool>();

        private void Start()
        {
            mainCamera = Camera.main;

            renderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));

            //隠すイベントを登録
            isHide.Subscribe(isHide =>
            {
                foreach (Renderer rend in renderers)
                {
                    rend.enabled = !isHide;
                }
            });

            if (!activeOnStart)
            {
                Disable();
            }
        }

        private void Update()
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 objectForward = transform.forward;

            float angle = Vector3.Angle(cameraForward, objectForward);
            isHide.Value = angle < hideAngle;
        }

        public void AddRenderer(Renderer renderer)
        {
            renderers.Add(renderer);
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            isHide.Value = false;
        }
    }
}