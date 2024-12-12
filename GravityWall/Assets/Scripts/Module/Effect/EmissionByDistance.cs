using CoreModule.Helper;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    /// <summary>
    /// カメラとの距離によってエミッションの強さを変えるクラス
    /// </summary>
    public class EmissionByDistance : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField, Header("エミッションを変化させる範囲")] private MinMaxValue distanceRange;
        [SerializeField, Header("最低エミッション")] private float minEmission;

        private static readonly int emissionIntensityProperty = Shader.PropertyToID("_EmissionIntensity");

        private void Update()
        {
            Camera cam = Camera.main;
            
            if (cam == null)
            {
                return;
            }
            
            Vector3 p = transform.position - cam.transform.position;

            // カメラとの距離を0 ~ 1にリマップしてエミッションの強さに変換する
            float intensity = Mathf.Max(minEmission, distanceRange.Remap01Squared(p.x * p.x + p.y * p.y + p.z * p.z));

            targetRenderer.material.SetFloat(emissionIntensityProperty, intensity);
        }
    }
}