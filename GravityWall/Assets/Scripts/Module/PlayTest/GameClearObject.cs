using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.PlayTest
{
    public class GameClearObject : MonoBehaviour
    {
        [SerializeField] private string SceneName;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                SceneManager.LoadScene(SceneName);

        }
    }
}

