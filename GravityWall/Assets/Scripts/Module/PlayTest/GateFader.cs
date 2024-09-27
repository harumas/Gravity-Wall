using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateFader : MonoBehaviour
{
    [SerializeField] private MeshRenderer gate;

    public void FadeGate(bool isAlpha)
    {
        gate.material.color = new Color(gate.material.color.r, gate.material.color.g, gate.material.color.b, isAlpha ? 0.3f : 1.0f);
    }
}
