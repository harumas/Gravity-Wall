using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module.Gimmick;
using Module.Gravity;
using Constants;
using Cinemachine;
namespace Module.PlayTest

{
    public class SavePointObject : MonoBehaviour
    {
        private enum Gravity
        {
            down,
            left,
            up,
            right,
            forward,
            back
        }
        [SerializeField] private Gravity gravity;
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
                respawnManager.GravityScale = ChangeGravity();
                FirstTouch = true;
                respawnManager.ObjectReset();
            }
        }

        private Vector3 ChangeGravity()
        {
            switch(gravity)
            { 
                case Gravity.down:return Vector3.down;
                case Gravity.left:return Vector3.left;
                case Gravity.right:return Vector3.right;
                case Gravity.up:return Vector3.up;
                case Gravity.forward:return Vector3.forward;
                case Gravity.back:return Vector3.back;
            }
            return Vector3.down;
        }
    }
}
