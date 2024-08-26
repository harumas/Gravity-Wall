using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module.Gimmick;

namespace Module.PlayTest
{
    public class SavePointObject : MonoBehaviour
    {
        private RespawnManager respawnManager => RespawnManager.instance;
        private bool FirstTouch = false;
        [SerializeField] private GameObject RetryPositionObject;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !FirstTouch)
            {
                Debug.Log("�ۑ�����܂���");
                respawnManager.RetryPosition = RetryPositionObject.transform.position;
                respawnManager.GravityScale = Gravity.Value;
                FirstTouch = true;
                respawnManager.Respawn();
            }
        }
    }
}
