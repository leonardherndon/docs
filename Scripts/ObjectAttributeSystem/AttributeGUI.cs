using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.ObjectAttributeSystem;
using UnityEngine;

public class AttributeGUI : MonoBehaviour
{
    [SerializeField] private AttributeType _type;
    [SerializeField] private AttributeController _attributeController;
    [SerializeField] private ChromaShift.Scripts.ObjectAttributeSystem.AttributeStack _stack;
    [SerializeField] private float _currentNumber;

    [SerializeField] private EnergyBar _bar;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        _bar = GetComponent<EnergyBar>();
        _attributeController = GameManager.Instance.playerShip.GetComponent<AttributeController>();
        _stack = _attributeController.attributes;
    }

    // Update is called once per frame
    void Update()
    {
        _currentNumber = _stack[_type].x;
        _bar.valueCurrent = (int)_currentNumber;
    }
}
