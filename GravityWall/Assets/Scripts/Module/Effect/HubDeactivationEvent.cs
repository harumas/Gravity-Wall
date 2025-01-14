using UnityEngine;

namespace Module.Effect
{
    /// <summary>
    /// ハブ全体を非表示にするイベント
    /// </summary>
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
