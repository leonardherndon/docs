using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using DG.Tweening;
using UnityEngine;

public class PlayerFusionCoreShaderController : MonoBehaviour
{

    [SerializeField] private MeshRenderer hdrpRend;
    [SerializeField] private ICollidible CoC;
    
    void Awake()
    {
        if (CoC == null) {
            CoC = GetComponent<ICollidible>();
        }
    }

    private void OnEnable()
    {
        
        CoC.OnLostRedCore += DoLoseRedCore;
        CoC.OnLostGreenCore += DoLoseGreenCore;
        CoC.OnLostBlueCore += DoLoseBlueCore;
        
        CoC.OnGainedRedCore += DoGainRedCore;
        CoC.OnGainedGreenCore += DoGainGreenCore;
        CoC.OnGainedBlueCore += DoGainBlueCore;
        //Debug.Log("Events Subscribed");
    }
    
    private void OnDisable()
    {
        
        CoC.OnLostRedCore -= DoLoseRedCore;
        CoC.OnLostGreenCore -= DoLoseGreenCore;
        CoC.OnLostBlueCore -= DoLoseBlueCore;
        
        CoC.OnGainedRedCore -= DoGainRedCore;
        CoC.OnGainedGreenCore -= DoGainGreenCore;
        CoC.OnGainedBlueCore -= DoGainBlueCore;
    }

    public void DoLoseRedCore()
    {
        //Debug.Log("DoLoseRedCore");
        hdrpRend.material.DOFloat(0, "_ChromaControl_R", 0.25f);
    }
    public void DoLoseGreenCore()
    {
        //Debug.Log("DoLoseGreenCore");
        hdrpRend.material.DOFloat(0, "_ChromaControl_G", 0.25f);
    }
    public void DoLoseBlueCore()
    {
        //Debug.Log("DoLoseBlueCore");
        hdrpRend.material.DOFloat(0, "_ChromaControl_B", 0.25f);
    }
    
    public void DoGainRedCore()
    {
        hdrpRend.material.DOFloat(1, "_ChromaControl_R", 0.25f);
    }
    public void DoGainGreenCore()
    {
        hdrpRend.material.DOFloat(1, "_ChromaControl_G", 0.25f);
    }
    public void DoGainBlueCore()
    {
        hdrpRend.material.DOFloat(1, "_ChromaControl_B", 0.25f);
    }
}
