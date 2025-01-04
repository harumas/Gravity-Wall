using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
namespace Module.Effect
{
    public class EffectPlayer : MonoBehaviour
    {
        private VisualEffect[] visualEffects;
        // Start is called before the first frame update
        void Start()
        {
            visualEffects = GetComponentsInChildren<VisualEffect>();
            PlayEffect();
        }

        private void OnEnable()
        {
            //PlayEffect();
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