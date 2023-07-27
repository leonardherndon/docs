using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityActionOnTrigger : MonoBehaviour
{
    
    [SerializeField] private UnityEvent doAction;
    public UnityEvent DoAction
    {
        get => doAction;
        set => doAction = value;
    }


    private void OnTriggerEnter(Collider other)
    {
        doAction.Invoke();
    }
}
