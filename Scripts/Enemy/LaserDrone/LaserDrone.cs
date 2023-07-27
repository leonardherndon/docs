using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrone : Hostile {
    private enum LaserDroneBroadcastType {Transmitter, Receiver}

    [Header("[LASER DRONE]")] [SerializeField]
    private LaserDroneBroadcastType broadcastType;
    [SerializeField]
    float hitDistance;
    [SerializeField]
    public float holdTime = 2f;
    [SerializeField]
    int waitForCheckFramesMax = 10;
    [SerializeField]
    int waitForCheckFramesCurrent = 0;
    [SerializeField]
    private float ActivationAngle = 0; 
    [SerializeField]
    float AnglePadding = 5;
    [SerializeField]
    bool needsToRotate;
    [SerializeField]
    private float rotateSpeed = 0.25f;
    [SerializeField]
    bool needsDualConnection;
    [SerializeField]
    public GameObject laserBarrier;
    [SerializeField]
    public LaserDrone otherDrone;
    [SerializeField]
    GameObject payload;
    [SerializeField] private float rotationCurrent;
    [SerializeField] private float droneDelta;
    [SerializeField] private float distanceBetweenDrones;
    protected override void Start()
    {
        base.Start();
        rotationCurrent = transform.eulerAngles.z;
        if(otherDrone)
            distanceBetweenDrones = Vector3.Distance(transform.position,otherDrone.transform.position);
    }

    public override void FixedUpdate()
    {
        if (ableToThreatenPlayer)
        {
            if (!laserBarrier)
            {
                if (waitForCheckFramesCurrent % waitForCheckFramesMax == 0)
                {
                    if(broadcastType == LaserDroneBroadcastType.Transmitter)
                        CheckForConnection();
                }
                
                if (needsToRotate)
                {
                    RotateTheDrone();
                }
                
                waitForCheckFramesCurrent++;
            }
            
        }

        base.FixedUpdate();
    }

    private void CheckForConnection()
    {
        //Checks Angle of This object within it's padding
        bool isConnectionMade = (rotationCurrent <= (ActivationAngle + AnglePadding)) && (rotationCurrent >= (ActivationAngle - AnglePadding));
        
        if (needsDualConnection)
        {
            droneDelta = Mathf.Abs(Mathf.DeltaAngle(rotationCurrent, otherDrone.rotationCurrent));    
            isConnectionMade = (droneDelta >= 180 - AnglePadding);
        }

        if(isConnectionMade)
            Spawn(otherDrone.gameObject);
    }

    

    private void Spawn(GameObject other)
    {
        Vector3 midpoint = (transform.position + other.transform.position) / 2;
        
        laserBarrier = Instantiate(payload, midpoint, Quaternion.identity);
        otherDrone.laserBarrier = laserBarrier;
        LaserDroneLaser laser = laserBarrier.GetComponent<LaserDroneLaser>();
        
        laserBarrier.transform.position = midpoint;
        laserBarrier.transform.localRotation = Quaternion.Euler(0,0, ActivationAngle);
        laserBarrier.transform.localScale = new Vector3(distanceBetweenDrones, 0.4f, 0.4f);
        
        //Debug.Log("Spawning Laser Drone Laser at Angle: " + transform.rotation.z * 360);
        
        
        laser.aliveTime = holdTime;
        laser.CoC.CSM.ChromaShift(CoC.CSM.CurrentColor);
        //laser.CoC.SetCollisionColor();

        if (!needsToRotate)
            laserBarrier.transform.parent = transform;
        else
            laserBarrier.transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;

        //laser.CoC.CSM.ChromaShift(ColorManager.Instance.GetColorMix(CoC.CSM.CurrentColor,other.GetComponent<LaserDrone>().CoC.CSM.CurrentColor));
    }

    void RotateTheDrone ()
    {
        distanceBetweenDrones = Vector3.Distance(transform.position,otherDrone.transform.position);
        transform.Rotate(0, 0, rotateSpeed, Space.Self);
        rotationCurrent = transform.eulerAngles.z;
    }
}
