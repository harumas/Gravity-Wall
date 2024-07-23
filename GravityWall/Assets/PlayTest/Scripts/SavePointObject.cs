using Module.Gimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointObject : MonoBehaviour
{
    private RespawnManager respawnManager => RespawnManager.instance;
    private bool FirstTouch = false;
    [SerializeField] private GameObject RetryPositionObject;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"&&!FirstTouch)
        {
            Debug.Log("•Û‘¶‚³‚ê‚Ü‚µ‚½");
            respawnManager.RetryPosition = RetryPositionObject.transform.position;
            respawnManager.GravityScale = Gravity.Value;
            FirstTouch = true;
        }
            
        
    }

   
}
