using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILifeSystemUpdater : MonoBehaviour
{
    [SerializeField] private LifeSystemController _lifeSystem;
    [SerializeField] private ChromaShiftManager _chromaShiftManager;
    public UltimateStatusBar LifeStatusBar;
    
    // Start is called before the first frame update
    void Start()
    {
        _lifeSystem.LifeChanged += UpdateStatus;
        _chromaShiftManager.OnColorChange += UpdateStatusColor;
    }

    void OnEnable()
    {
        _lifeSystem.LifeChanged += UpdateStatus;
        _chromaShiftManager.OnColorChange += UpdateStatusColor;
    }
    
    void OnDisable()
    {
        _lifeSystem.LifeChanged -= UpdateStatus;
        _chromaShiftManager.OnColorChange -= UpdateStatusColor;
    }

    private void OnDestroy()
    {
        _lifeSystem.LifeChanged -= UpdateStatus;
        _chromaShiftManager.OnColorChange -= UpdateStatusColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateStatus(float currVal, float prevVal, float maxVal, bool player)
    {
        UltimateStatusBar.UpdateStatus( "LifeSystemBar", "LifeBattery", currVal, maxVal );
        
//        Debug.Log("LifeSystem Event working: " + currVal + " | " + maxVal);
    }

    void UpdateStatusColor(GameColor newColor)
    {
        LifeStatusBar.GetUltimateStatus("LifeBattery").UpdateStatusColor(ColorManager.Instance.colorArray[ColorManager.Instance.ConvertColorToIndex(newColor)]);
    }
}
