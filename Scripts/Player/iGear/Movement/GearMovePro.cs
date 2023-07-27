using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class GearMovePro : GearMoveApp {

    private GearSystem _gearSystem;
    public int gearMoveAppId;
    private GameObject[] _speedTrails;
    public GameObject[] speedTrails;
    private float[] trailstrengthTable = new float[] { 0.25f, 0.4f, 0.65f, 1.2f, 1.2f, 1.2f };

    public int gearIndex; //Max Gear Number is Max index on horiSpeedTable
    public int GearIndex { get => gearIndex; }
    public int gearMax;
    public int gearIndexCache;
    public bool overrideGearMax = false;
    
    protected override void Awake()
    {

        gearMoveAppId = 1;
        horiSpeedTable = new int[]      { -50, 0, 50, 500, 6000, 8000 }; //{ 1750, 5500, 6750, 8000, 24000, 36000 };
        vertFactorTable = new float[]   { 1200, 700, 1200, 2000, 2000, 2000 }; //{ 1500, 2750, 3500, 5000, 8000, 8000 };
        
        gearIndex = 1; //Max Gear Number is Max index on horiSpeedTable
        gearMax = 3;
        overrideGearMax = false;
        _gearSystem = gameObject.GetComponent<GearSystem>();
        // _gearSystem.moveSystem = this;
        SetSpeed();
    }

    public void FixedUpdate()
    {
        if(playerShip.fusionCoreInventory.Count <= 0 && gearIndex >= 3)
        {
            gearIndex--;
            SetTrailFX(gearIndex);
            SetSpeed();
        }
    }
    
    
    public override void SetSpeedGear(int gear, bool setGearDirect = false)
    {
        
        if(setGearDirect)
            gearIndex = gear;
        else
            gearIndex += gear;


        if (!overrideGearMax)
        {
            if (gearIndex >= gearMax)
            {
                //This restricts the player from using a Gear Higher than 2 unless he has at least one core.
                if (playerShip.fusionCoreInventory.Count <= 0)
                {
                    //Debug.Log("Shift Up 1A");
                    gearIndex = gearMax - 1;
                    AudioManager.Instance.PlayClipWrap(3, 22);
                }
                else
                {
                    //Debug.Log("Shift Up 1B");
                    gearIndex = gearMax;
                    AudioManager.Instance.PlayClipWrap(3, 23);
                }
            }
        }
        
        //Debug.Log("Gear Index: " + gearIndex);
        if(gearIndex > gearIndexCache && gearIndex < gearMax)
        {
            if (gearIndex == gearMax)
            {
                //Debug.Log("Shift Up 2A");
                AudioManager.Instance.PlayClipWrap(3, 23);
            }
            else
            {
                //Debug.Log("Shift Up 2B");
                AudioManager.Instance.PlayClipWrap(3, 20);
            }
        }

        if (gearIndex < gearIndexCache && gearIndex >= 0)
        {
            //Debug.Log("Shift Down 1");
            AudioManager.Instance.PlayClipWrap(3, 21);
        }

        if (gearIndex < 0)
        {
            //Debug.Log("Shift Down 2");
            AudioManager.Instance.PlayClipWrap(3, 22);
            gearIndex = 0;
        }


        gearIndexCache = gearIndex;
        SetTrailFX(gearIndex);
        SetSpeed();

    }

    private void SetTrailFX(int gear)
    {
        return;
        ParticleSystem.MainModule pSys;

        foreach (GameObject trail in speedTrails)
        {
            pSys = trail.GetComponent<ParticleSystem>().main;
            pSys.startLifetime = trailstrengthTable[gear];
            if(gear == gearMax)
            {
                pSys.startColor = new Color(1, 0.25f, 0.25f);
            } else
            {
                pSys.startColor = new Color(1, 1, 1);
            }
        }

    }
}
