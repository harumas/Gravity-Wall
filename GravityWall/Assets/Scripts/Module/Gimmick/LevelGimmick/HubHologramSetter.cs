using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class HubHologramSetter : MonoBehaviour
    {
        [SerializeField] private Material arrowMaterial, checkMaterial;
        [SerializeField] private MeshRenderer meshRenderer;

        public void SetHologramMaterial(bool isClear)
        {
            meshRenderer.material = isClear ? checkMaterial : arrowMaterial;
        }
    }
}
