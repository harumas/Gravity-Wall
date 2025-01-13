using UnityEngine;
using UnityEngine.VFX;
namespace Module.Effect
{
    public class EffectPlayer : MonoBehaviour
    {
        private VisualEffect[] visualEffects;
        
        void Start()
        {
            visualEffects = GetComponentsInChildren<VisualEffect>();
            PlayEffect();
        }

        public void PlayEffect()
        {
            foreach (var effect in visualEffects)
            {
                effect.Play();
            }
        }

        public void StopEffect()
        {
            foreach (var effect in visualEffects)
            {
                effect.Play();
            }
        }
    }
}