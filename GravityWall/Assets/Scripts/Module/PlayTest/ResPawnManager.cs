using Module.Gimmick;
using System;
using System.Collections;
using System.Collections.Generic;
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
            GravityScale = Gravity.Value;

            if (MoveObject == null)
                return;

            MoveObjectPosition = new Vector3[MoveObject.Length];
            for (int i = 0; i < MoveObjectPosition.Length; i++)
            {
                MoveObjectPosition[i] = MoveObject[i].transform.position;
            }
        }

        public void Respawn()
        {
            Player.transform.position = RetryPosition;
            Gravity.SetValue(GravityScale);

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
                Debug.Log("•œŠˆ‚µ‚Ü‚µ‚½");
                Respawn();
            }
        }
    }
}

