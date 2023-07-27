using UnityEngine;
using System.Collections;
using DG.Tweening;
using Chronos;

public class MoveToPlayerState : IMoveState {

    public string stateName = "MoveToPlayerState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	public Quaternion rotateThis;
	private Transform target;
	private Vector3 step;

	//Movement Params
	private float _baseSpeed;
	private float _alignSpeed;
	
	public MoveToPlayerState (Hostile enemy)
	{
		mObject = enemy.MoC;
		eObject = enemy;
		target = GameManager.Instance.playerShip.transform;
		step = Vector3.zero;
	}

	public void UpdateState()
	{
		GatherParams(mObject.useBlockParams);
		MoveObject();
	}

	public void Update()
	{
		
	}

	private void GatherParams(bool useBlockParams = false)
	{
		if (useBlockParams)
		{
			_baseSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].baseSpeed;
			_alignSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].alignSpeed;
		}
		else
		{
			_baseSpeed = mObject.baseSpeeds[eObject.currentBlockIndex];
			_alignSpeed = mObject.alignSpeed;
		}
	}

	private void MoveObject() 
	{

		float speedFinal;
		Vector3 shipVelocity;
		//SHIP SHOULD ALWAYS BE MOVING FORWARD
		shipVelocity = new Vector3(1,0,0).normalized;

		//FIGURE OUT IF ANYTHING SHOULD BE MANIPULATING THE SHIP'S SPEED
		mObject.speedMod = mObject.FindSpeedModifiers();

        //CALCULATE FINAL SPEED
        speedFinal = (_baseSpeed * mObject.speedMod);


        //OVERRIDE FINAL SPEED
        if (mObject.isSpeedOverwritten) {
			speedFinal = mObject.speedOverride;
		}



		if ( GameManager.Instance.playerShip) {
//            Debug.Log("MoveToPlayerState Active");
            
            if (mObject.moveToPlayerRotation)
            {
                mObject.transform.rotation = Quaternion.Lerp(mObject.transform.rotation, Quaternion.LookRotation(target.position - mObject.transform.position), eObject.GetComponent<Timeline>().deltaTime * _alignSpeed);
                step = mObject.transform.forward * eObject.GetComponent<Timeline>().deltaTime * speedFinal;
                mObject.transform.position += step;
            } else
            {
                Vector3 moveDir = (target.position - mObject.transform.position).normalized;
                step = moveDir * eObject.GetComponent<Timeline>().deltaTime * speedFinal;
                mObject.transform.position += step;
            }
            
		}
			

	}

}
