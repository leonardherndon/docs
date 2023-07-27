using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class GearAgileTeleport : GearAgileApp {

    private GearSystem gearSystem;
    public GameObject[] speedTrails;
    private float[] trailstrengthTable = new float[] { 0.25f, 0.4f, 0.65f, 1.2f, 1.2f, 1.2f };
    public float horiDistance;
    public float vertDistnace;
    public float speed;

    protected override void Awake()
    {
        base.Awake();
        
        gearAgileAppId = 1;
        gearSystem = gameObject.GetComponent<GearSystem>();
        gearSystem.AgileSystem = this;

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
