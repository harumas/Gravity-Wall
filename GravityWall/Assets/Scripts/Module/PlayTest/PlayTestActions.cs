using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Module.PlayTest
{
    public class PlayTestActions : MonoBehaviour
    {
        [SerializeField] private GameObject TestObj;
        [SerializeField] private GameObject note;
        void Start()
        {
            EventSystem.current.firstSelectedGameObject = TestObj;

        }

        public void OnButton()
        {
            note.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}

