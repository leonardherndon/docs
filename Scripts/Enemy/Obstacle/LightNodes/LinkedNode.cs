using System.Collections.Generic;
using ChromaShift.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LinkedNode : ColorNode, IColorNodeGroup
{
    
    public ChromaShiftManager CSM { get; set; }
    
    [SerializeField] private ColorNodeSet[] _NodeGroup;
    
    public ColorNodeSet[] NodeGroup { 
        get => _NodeGroup;
        set => _NodeGroup = value;
    }
    
    private UnityEvent _unlocked;
    public UnityEvent Unlocked
    {
        get => _unlocked;
        set => _unlocked = value;
    }

    public void Awake()
    {
        CSM = gameObject.GetComponent<ChromaShiftManager>();
    }

    public void RunNodeColorChecks(GameColor newColor)
    {
        FusionCore[] tempCores = GameManager.Instance.EMPTYFC;
        List<FusionCore[]> tempCoreSets = new List<FusionCore[]>();
        
        foreach (ColorNodeSet node in _NodeGroup)
        {
            tempCoreSets.Add(node.ColorNode.CSM.CoreInventory);
        }

        foreach (FusionCore[] coreSet in tempCoreSets)
        {
            for (int i = 0; i < coreSet.Length; i++)
            {
                if (coreSet[i].IsActive)
                {
                    tempCores[i].IsActive = true;
                }
            }
        }

        CSM.CoreInventory = tempCores;
        CSM.SetColorFromInventory();
    }
}