using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.ObjectAttributeSystem;
using UnityEngine;

public class UIAttributeUpdater : MonoBehaviour
{
    [SerializeField]  AttributeController _attributeController;

    [SerializeField] private EnergyBar HullBar;
    [SerializeField] private EnergyBar DataBar;
    [SerializeField] private EnergyBar MentalBar;
    [SerializeField] private EnergyBar ThermalBar;
    
    // Start is called before the first frame update
    void Start()
    {
        _attributeController.AttributeChange += UpdateStatus;
    }

    void OnEnable()
    {
        _attributeController.AttributeChange += UpdateStatus;
    }

    void UpdateStatus(AttributeType type, float currVal, float maxVal)
    {

        /*switch (type)
        {
            case AttributeType.HullIntegrity:
                HullBar.valueCurrent = (int)currVal;
                break;
                
            case AttributeType.ThermalIntegrity:
                ThermalBar.valueCurrent = (int)currVal;
                break;
                
            case AttributeType.DataIntegrity:
                DataBar.valueCurrent = (int)currVal;
                break;
                
            case AttributeType.MentalIntegrity:
                MentalBar.valueCurrent = (int)currVal;
                break;
        }*/
        
        Debug.Log("Attribute Updater Event working: " + currVal + " | " + maxVal);
    }
}
