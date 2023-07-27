using UnityEngine;
using System.Collections;
using Chronos;

public class MoveHorizontalState : IMoveState {

    public string stateName = "MoveHorizontalState";

	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	private bool moveSet = false;

	//Movement Params
	private bool _horizontalMoveRight;
	private float _baseSpeed;
	
	
	public MoveHorizontalState (Hostile enemy)
	{
		mObject = enemy.MoC;
		eObject = enemy;
	}

	public void UpdateState()
	{
		GatherParams(mObject.useBlockParams);
		MoveObject();
	}

	public void Update() {}

	private void GatherParams(bool useBlockParams = false)
	{
		if (useBlockParams)
		{
			_horizontalMoveRight = mObject.moveList.MoveStack[eObject.currentBlockIndex].horizontalMoveRight;
			_baseSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].baseSpeed;
		}
		else
		{
			_horizontalMoveRight = mObject.horizontalMoveRights[eObject.currentBlockIndex];
			_baseSpeed = mObject.baseSpeeds[eObject.currentBlockIndex];
		}
	}

	private void MoveObject() 
	{

		float speedFinal;
		Vector3 shipVelocity;

		//SHIP SHOULD ALWAYS BE MOVING FORWARD
		if (_horizontalMoveRight) {
			Vector3 rot = eObject.transform.rotation.eulerAngles;
			rot = new Vector3(rot.x,rot.y,180);
			eObject.transform.rotation = Quaternion.Euler(rot);
			//Debug.Log ("Rotation should be working");
			shipVelocity = new Vector3 (1, 0, 0).normalized;
		} else {
			shipVelocity = new Vector3 (-1, 0, 0).normalized;
		}

		//FIGURE OUT IF ANYTHING SHOULD BE MANIPULATING THE SHIP'S SPEED
		mObject.speedMod = mObject.FindSpeedModifiers();

        //CALCULATE FINAL SPEED
        speedFinal = (_baseSpeed * mObject.speedMod);

        //OVERRIDE FINAL SPEED
		if (mObject.isSpeedOverwritten) {
			speedFinal = mObject.speedOverride;
		}

		//Debug.Log ("SppedFinal: " + speedFinal);

		//FINALLY TIME TO MOVE THE SHIP
//		eObject.rB.AddForce(mObject.movableObject.transform.position + shipVelocity * speedFinal * eObject.GetComponent<Timeline>().deltaTime);
		eObject.rB.MovePosition (mObject.movableObject.transform.position + shipVelocity * speedFinal * eObject.GetComponent<Timeline>().deltaTime);

	}

}
