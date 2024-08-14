using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.Gimmick
{
    public class DeathFloor : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}