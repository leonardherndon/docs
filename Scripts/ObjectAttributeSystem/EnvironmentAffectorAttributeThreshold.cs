using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnvironmentAffectorAttributeThreshold : MonoBehaviour
{
    public bool isActive = true;
    public float triggerNumber;
    public UnityEvent action;

    public void DoTriggerAction()
    {
        action.Invoke();
    }
}
