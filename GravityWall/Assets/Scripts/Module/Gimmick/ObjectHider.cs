using System;
using R3;
using UnityEngine;

namespace Module.Gimmick
{
    public class ObjectHider : MonoBehaviour
    {
        [Header("オブジェクトを隠す角度")]
        [SerializeField] private float hideAngle = 80f;

        private Camera mainCamera;
        private Renderer[] renderers;
        private ReactiveProperty<bool> isHide = new ReactiveProperty<bool>();

        private void Start()
        {
            mainCamera = Camera.main;
            renderers = gameObject.GetComponentsInChildren<Renderer>(true);

            //隠すイベントを登録
            isHide.Subscribe(isHide =>
            {
                foreach (Renderer rend in renderers)
                {
                    rend.enabled = !isHide;
                }
            });
        }

        private void Update()
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 objectForward = transform.forward;

            float angle = Vector3.Angle(cameraForward, objectForward);
            isHide.Value = angle < hideAngle;
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