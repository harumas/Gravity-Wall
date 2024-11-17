using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

public class QuickViewActivator : MonoBehaviour
{
    [SerializeField] private ViewBehaviourState behaviourType;
    [SerializeField] private ViewBehaviourNavigator navigator;
    
    void Start()
    {
        navigator.ActivateBehaviour(behaviourType);
    }
}
