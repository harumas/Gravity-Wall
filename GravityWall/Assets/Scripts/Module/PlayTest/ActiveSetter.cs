using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.PlayTest
{
    public class ActiveSetter : MonoBehaviour
    {
        [SerializeField] private bool startActive;
        void Start()
        {
            gameObject.SetActive(startActive);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}