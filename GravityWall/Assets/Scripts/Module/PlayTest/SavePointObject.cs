using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module.Gimmick;
using Module.Gravity;
using Constants;
namespace Module.PlayTest
{
    public class SavePointObject : MonoBehaviour
    {
        private RespawnManager respawnManager => RespawnManager.instance;
        private bool FirstTouch = false;
        [SerializeField] private GameObject RetryPositionObject;
        [SerializeField] private GameObject puzzleBox;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player) && !FirstTouch)
            {
                Debug.Log("セーブしました");
                respawnManager.RetryPosition = RetryPositionObject.transform.position;
                respawnManager.GravityScale = WorldGravity.Instance.Gravity;
                FirstTouch = true;
                respawnManager.ObjectReset();
            }
        }
    }
}
