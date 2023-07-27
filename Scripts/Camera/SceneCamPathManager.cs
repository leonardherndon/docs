using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jacovone;
using UnityEngine.Serialization;

public class SceneCamPathManager : MonoBehaviour {


    PathMagic scenePath;
    [FormerlySerializedAs("beginLevelTrigger")] public Transform beginLoopMarker;
    [FormerlySerializedAs("endLevelTrigger")] public Transform endLoopMarker;
    [FormerlySerializedAs("levelDistance")] public float loopDistance;
    [SerializeField] private int loopIndex = 0;
    public float targetPosition;
    public float distanceFromPlayer;
    public float distPercentage;
    private Vector3 cameraPositionCache;
    [SerializeField] private float verticalSensitivity = 1f;
	
    // Use this for initialization
	void Start () {
        scenePath = GameObject.Find("CameraPath").GetComponent<PathMagic>();
        CalculateLoopDistance();
        CacheCameraVerticalPosition();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
       
        if (loopDistance != 0)
        {
            CalculateCurrentDistance();
            CalculateCurrentVerticalPosition();
        }
	}


    void SetScenePath(PathMagic path)
    {
        scenePath = path;
    }

    public void CalculateLoopDistance()
    {
        loopDistance = Math.Abs(beginLoopMarker.position.x - endLoopMarker.position.x);
        
    }

    private void CalculateCurrentDistance()
    {
        targetPosition = GameManager.Instance.playerShip.transform.position.x;
        loopIndex = (int)Math.Floor(targetPosition / loopDistance); //3654 / 3000 = 1.218 -- 1
        
        distanceFromPlayer = Mathf.Abs(beginLoopMarker.position.x - targetPosition);
        distPercentage = (distanceFromPlayer / loopDistance);

        if (distPercentage < 0.001f)
            distPercentage = 0.01f;
        scenePath.CurrentPos = distPercentage;
        UltimateStatusBar.UpdateStatus( "SceneDistanceBar", "SceneDistance", (distPercentage * 100), 100 );
    }

    private void CalculateCurrentVerticalPosition()
    {
        Vector3 newPosition = new Vector3(cameraPositionCache.x,
            cameraPositionCache.y + (GameManager.Instance.playerShip.transform.localPosition.y * verticalSensitivity), cameraPositionCache.z);
        transform.position = newPosition;;
    }

    private void CacheCameraVerticalPosition()
    {
        cameraPositionCache = gameObject.transform.position;
    }
}
