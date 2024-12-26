using System.Linq;
using Constants;
using Module.Gimmick.LevelGimmick;
using R3;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class StartRoom : MonoBehaviour
    {
        [SerializeField] private AdditiveLevelLoadTrigger loadTrigger;
        [SerializeField] private string observeLevelName;
        [SerializeField] private Gate roomGate;
        [SerializeField] private ObjectHider objectHider;
        [SerializeField] private GameObject roomObject;
        [SerializeField] private JustOnceStartGate justOnceStartGate;

        public bool IsPlayerEnter => isPlayerEnter;
        private bool isPlayerEnter;

        private void Start()
        {
            loadTrigger.OnSceneLoaded += OnSceneLoaded;
            loadTrigger.OnSceneUnload += OnSceneUnload;

            roomGate.IsEnabled.Skip(1).Subscribe(isEnable =>
            {
                if (!isEnable && !isPlayerEnter)
                {
                    roomObject.SetActive(false);
                    objectHider.Enable();
                }
            });
        }

        private void OnSceneLoaded()
        {
            // 有効化するレベルを取得
            var observeLevel = GameObject.FindGameObjectsWithTag(Tag.LevelSegment).FirstOrDefault(obj => obj.name == observeLevelName);

            if (observeLevel == null)
            {
                Debug.LogError($"{nameof(observeLevelName)}: {observeLevelName}が存在しません");
                return;
            }

            var levelActivator = observeLevel.GetComponent<LevelActivator>();
            levelActivator.OnActivateChanged += isActive =>
            {
                if (!isActive && !isPlayerEnter)
                {
                    objectHider.Disable();
                    roomGate.gameObject.SetActive(false);
                }
            };
        }

        private void OnSceneUnload()
        {
            justOnceStartGate.Reset();
            roomObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = false;
            }
        }
    }
}