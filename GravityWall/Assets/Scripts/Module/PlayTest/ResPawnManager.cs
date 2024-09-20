using Module.Gimmick;
using System;
using System.Collections;
using System.Collections.Generic;
using Module.Gravity;
using UnityEngine;

namespace Module.PlayTest
{
    public class RespawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject Player;
        [SerializeField] private GameObject[] MoveObject;
        [NonSerialized] public Vector3 RetryPosition;
        private Vector3[] MoveObjectPosition;
        [NonSerialized] public Vector3 GravityScale;
        public static RespawnManager instance;

        [SerializeField] private GameObject DeadCanvas;

        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
                instance = this;
        }

       
        private void Start()
        {
            RetryPosition = Player.transform.position;
            GravityScale = WorldGravity.Instance.Gravity;

            if (MoveObject == null)
                return;

            MoveObjectPosition = new Vector3[MoveObject.Length];
            for (int i = 0; i < MoveObjectPosition.Length; i++)
            {
                MoveObjectPosition[i] = MoveObject[i].transform.position;
            }
        }

        public void Damage()
        {
            DeadCanvas.SetActive(true);
            Invoke("Respawn", 1f);
        }
        public void Respawn()
        {
            DeadCanvas.SetActive(false);
            Player.transform.position = RetryPosition;
            WorldGravity.Instance.SetValue(GravityScale);

            if (MoveObject == null)
                return;

            for (int i = 0; i < MoveObjectPosition.Length; i++)
            {
                MoveObject[i].transform.position = MoveObjectPosition[i];
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("復活しました");
                Damage();
            }
        }
    }
}

