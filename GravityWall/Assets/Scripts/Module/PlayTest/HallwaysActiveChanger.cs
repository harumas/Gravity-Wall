using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwaysActiveChanger : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetRenderEnabled(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetRenderEnabled(false);
        }
    }

    public void SetRenderEnabled(bool isEnable)
    {
        meshRenderer.enabled = isEnable;
    }
}
