using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class StageLocker : MonoBehaviour
    {
        [SerializeField] private GameObject[] lockObjects;

        private void Awake()
        {
            foreach (GameObject lockObject in lockObjects)
            {
                lockObject.SetActive(false);
            }
        }

        public void Lock()
        {
            foreach (GameObject lockObject in lockObjects)
            {
                lockObject.SetActive(true);
            }
        }
    }
}