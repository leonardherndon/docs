using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Chronos;

public class RotateState : IMoveState
{

    public string stateName = "WaypointState";
    private readonly Hostile eObject;
    private readonly MovableObjectController mObject;
    private Transform target;
    private float _rotationCurrent;
    private bool _targetLocked;
    private bool _isMoveSet;
    private float _anglePadding;
    private float _activationAngle;

    //Movement Params
    private float _alignSpeed;

    public RotateState(Hostile enemy)
    {
        
        mObject = enemy.MoC;
        eObject = enemy;
        target = eObject.targetObject.transform;
        _rotationCurrent = eObject.transform.eulerAngles.z;
        _activationAngle = 0;
        _anglePadding = 5;
    }

    public void UpdateState()
    {
        GatherParams(mObject.useBlockParams);
        RotateObject();
    }

    public void Update()
    {

    }

    private void GatherParams(bool useBlockParams = false)
    {
        if (useBlockParams)
        {
            _alignSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].alignSpeed;
        }
        else
        {
            _alignSpeed = mObject.alignSpeed;
        }
    }

    
    private void CheckForConnection()
    {
        //Checks Angle of This object within it's padding
        bool isConnectionMade = (_rotationCurrent <= (_activationAngle + _anglePadding)) && (_rotationCurrent >= (_activationAngle - _anglePadding));

        if (isConnectionMade)
            eObject.currentMovementCompleted = true;
    }
    
    private void RotateObject()
    {
        var transform = eObject.transform;
        mObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - target.position), eObject.GetComponent<Timeline>().deltaTime * _alignSpeed);
        //eObject.transform.Rotate(0, 0, _alignSpeed, Space.Self);
        //_rotationCurrent = eObject.transform.eulerAngles.z;
        
    }
}