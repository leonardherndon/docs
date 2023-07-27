using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class ColorNodeGroupSimple : MonoBehaviour, IColorNodeGroup
{

    [SerializeField] private ColorNodeSet[] _NodeGroup;
    
    public ColorNodeSet[] NodeGroup {
        get => _NodeGroup;
        set => _NodeGroup = value;
    }

    [SerializeField] private UnityEvent _unlocked;
    public UnityEvent Unlocked
    {
        get => _unlocked;
        set => _unlocked = value;
    }

    void Start()
    {
        //Debug.Log("Node Listener Registered.");
        foreach (ColorNodeSet node in _NodeGroup)
        {
            node.ColorNode.CSM.OnColorChange += RunNodeColorChecks;
        }
        
    }

    public void RunNodeColorChecks (GameColor newColor)
    {
        foreach (ColorNodeSet node in _NodeGroup)
        {
            if (node.ColorNode.CSM.CurrentColor != node.ColorIndex)
            {
                Debug.Log (gameObject.GetInstanceID() + " " + node.ColorNode.gameObject.name + ": Group Failed Color Test | " + node.ColorNode.CSM.CurrentColor + "/" + node.ColorIndex);
                return;
            }
            else
            {
                Debug.Log (gameObject.GetInstanceID() + " " + node.ColorNode.gameObject.name + ": Group Passed Color Test | " + node.ColorNode.CSM.CurrentColor + "/" + node.ColorIndex);
            }
        }
        Debug.Log (gameObject.GetInstanceID() + ": Event On Group Success");
        _unlocked.Invoke();
    }
}
