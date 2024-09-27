using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class TimerGate : AbstractGimmickAffected
    {
        [SerializeField] private float openTime;
        [SerializeField] private GameObject gate;
        [SerializeField] private Transform lightBasePosition;
        [SerializeField] private GameObject Counterlight;
        private Material lightMaterial;
        public bool isOpen { get; private set; }
        private float timer;

        private const float intensity = 8.0f;
        private void Start()
        {
            var light = Instantiate(Counterlight, transform);
            light.transform.localPosition = lightBasePosition.localPosition;

            lightMaterial = light.GetComponent<MeshRenderer>().material;
        }

        public override void Affect(AbstractSwitch switchObject)
        {
            if (!switchObject.isOn) return;

            isOpen = true;
            timer = openTime;

            gate.SetActive(false);

            lightMaterial.EnableKeyword("_EMISSION");
            lightMaterial.SetColor("_EmissionColor", Color.green * (switchObject.isOn ? intensity : 0.0f));
        }

        private void Update()
        {
            if (!isOpen) return;

            timer -= Time.deltaTime;

            Debug.Log(timer);
            if (timer <= 0)
            {
                Reset();
            }
        }

        public override void Reset()
        {
            isOpen = false;

            lightMaterial.EnableKeyword("_EMISSION");
            lightMaterial.SetColor("_EmissionColor", Color.green * 0.0f);

            gate.SetActive(true);

            timer = openTime;
        }

        public float GetTime()
        {
            return timer;
        }
    }
}