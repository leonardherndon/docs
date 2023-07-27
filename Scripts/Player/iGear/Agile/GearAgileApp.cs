using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Chronos;
using CS_Audio;

public class GearAgileApp : MonoBehaviour, IGearAgileApp
{
    private int _gearAgileAppId;
    public int gearAgileAppId;
    protected PlayerShip playerShip;
    
    protected virtual void Awake()
    {
        playerShip = GameManager.Instance.playerShip;
    }

    public virtual void ActivateAgile()
    {
    }

    public virtual void EnterAgile()
    {
    }

    public void ExitAgile()
    {
    }
}