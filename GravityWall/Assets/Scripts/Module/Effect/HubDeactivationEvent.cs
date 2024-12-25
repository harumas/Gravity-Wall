using UnityEngine;

namespace Module.Effect
{
    public class HubDeactivationEvent : MonoBehaviour
    {
        [SerializeField] private GameObject[] hubObject;
        
        public void DeactivateHub()
        {
            foreach (var obj in hubObject)
            {
                obj.SetActive(false);
            }
        }
    }
}
