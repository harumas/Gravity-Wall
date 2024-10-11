using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Constants;
using UnityEngine.SceneManagement;

namespace Module.PlayTest
{
    public class GameClearObject : MonoBehaviour
    {
        private RespawnManager respawnManager => RespawnManager.instance;
        [SerializeField]private GameObject ClearCanvas;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                respawnManager.isClear = true;
                ClearCanvas.SetActive(true);
            }
              
        }
    }
}

