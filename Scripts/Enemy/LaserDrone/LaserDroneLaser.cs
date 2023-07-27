using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Enemy.BossArsenal;
using UnityEngine;

public class LaserDroneLaser : MonoBehaviour {
    
    private bool isSet = false;
    public float aliveTime;
    public CollisionController CoC;
    
    // Use this for initialization

    private void FixedUpdate()
    {
        if(!isSet)
            return;

        isSet = true;
        Destroy(this, aliveTime);
    }

    

}
