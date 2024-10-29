using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
namespace Module.PlayTest
{
    public class PlayTestActions : MonoBehaviour
    {
        [SerializeField] private GameObject TestObj;
        [SerializeField] private GameObject note;
        void Start()
        {
            EventSystem.current.firstSelectedGameObject = TestObj;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void OnButton()
        {
            //note.SetActive(true);
            // this.gameObject.SetActive(false);
            SceneManager.LoadScene("Tutorial01");
        }
    }
}

