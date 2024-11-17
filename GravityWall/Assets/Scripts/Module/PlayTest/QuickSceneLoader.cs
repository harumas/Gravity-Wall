using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.PlayTest
{
    [RequireComponent(typeof(BoxCollider))]
    public class QuickSceneLoader : MonoBehaviour
    {
        [SerializeField] private string targetSceneName;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}
