using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class Hallway : MonoBehaviour
    {
        [SerializeField] private List<string> gimmickObjects;

        private void Awake()
        {
            foreach (string path in gimmickObjects)
            {
                string[] names = path.Split('/');
                
            }
        }
    }
}