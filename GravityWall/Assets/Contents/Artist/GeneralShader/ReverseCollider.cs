using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Physics.queriesHitBackfaces = true;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.queriesHitBackfaces = true;
    }
}
