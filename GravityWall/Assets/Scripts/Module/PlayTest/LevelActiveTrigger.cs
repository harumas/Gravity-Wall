using Constants;
using UnityEngine;

namespace Module.PlayTest
{
    public class LevelActiveTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject[] levelObjects;
        private LevelActiveChanger levelActiveChanger;

        // Start is called before the first frame update
        void Start()
        {
            levelActiveChanger = transform.parent.GetComponent<LevelActiveChanger>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                levelActiveChanger.SetActiveLevel(levelObjects);
            }
        }
    }
}