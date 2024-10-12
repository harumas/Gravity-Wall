using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopObs : MonoBehaviour
{
   private ObsManager obsManager => ObsManager.instance;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (obsManager == null)
            return;
        obsManager.StopOBS();
    }

    
}
