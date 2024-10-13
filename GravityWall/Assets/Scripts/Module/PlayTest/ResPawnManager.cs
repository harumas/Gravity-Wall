using Module.Gimmick;
using System;
using System.Collections;
using System.Collections.Generic;
using Module.Gravity;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

        [SerializeField] private GameObject LoadingCanvas;
        private ObsManager obsManager => ObsManager.instance;
        public bool isClear = false;


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
            LoadingCanvas.SetActive(true);
            Invoke("WorldReset", 1.5f);
        }

        public void WorldReset()
        {
            ObjectReset();
            PlayerReset();
        }
        public void ObjectReset()
        {
            LoadingCanvas.SetActive(false);

            if (MoveObject == null)
                return;

            for (int i = 0; i < MoveObjectPosition.Length; i++)
            {
                MoveObject[i].transform.position = MoveObjectPosition[i];
            }
        }

        public void PlayerReset()
        {
            LoadingCanvas.SetActive(false);
            Player.transform.position = RetryPosition;
            WorldGravity.Instance.SetValue(GravityScale);
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadStageSelectScene()
        {
            SceneManager.LoadScene("PlayTestTitle");
        }
        private void Update()
        {
            if (isClear)
                return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                LoadingCanvas.SetActive(true);
                Invoke("ReloadScene", 0.3f);

            }

            else if (Input.GetKeyDown(KeyCode.P))
            {
                LoadingCanvas.SetActive(true);
                obsManager.StopOBS();
                Invoke("LoadStageSelectScene", 0.3f);
            }

            else if (Input.GetKeyDown(KeyCode.U) && !isClear)
                Damage();
        }
    }
}

