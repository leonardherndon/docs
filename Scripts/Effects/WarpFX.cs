using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpFX : MonoBehaviour {


    public GameObject cameraFX;
    //public CameraFilterPack_Color_GrayScale cameraFilter;
    public GameObject warpClones;
    public GameObject warpFlash;
    public GameObject warpRip;
    public float warpJumpSpeed = 250f;
    public float warpClimbSpeed = 15000f;

	public void FixedUpdate () {
        
        float inverseFilterRate = (-((GameManager.Instance.playerShip.warpCharge - GameManager.Instance.playerShip.warpSuccess) /100f)) + 0.5f;
        //Debug.Log(inverseFilterRate);
        //cameraFilter._Fade = inverseFilterRate;
    }
	
}
