using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Constants;

namespace Module.PlayTest
{
    public class GameClearObject : MonoBehaviour
    {
        private RespawnManager respawnManager => RespawnManager.instance;
        [SerializeField]private GameObject ClearCanvas;
        [SerializeField] private GameObject Player;
        [SerializeField] private string NextStage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                respawnManager.isClear = true;
                Player.SetActive(false);
                ClearCanvas.SetActive(true);
                Invoke("LoadScene",3);
            }
              
        }

        private void LoadScene()
        {
            SceneManager.LoadScene(NextStage);
        }
    }
}

