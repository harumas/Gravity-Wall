using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyButtonToQuit : MonoBehaviour
{
    [SerializeField] private float delay;

    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime < delay)
        {
            return;
        }
        
        if (Input.anyKey)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}