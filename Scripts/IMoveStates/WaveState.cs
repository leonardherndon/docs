using UnityEngine;
using System.Collections;


/// <summary>
/// Wave state.
/// 
/// THIS MOVEMENT STATE IS NOT COMPLETED
/// </summary>
public class WaveState : IMoveState {

    public string stateName = "WaveState";

	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	private bool moveSet = false;

	//Movement Params
	private float _baseSpeed;
	
	public WaveState (Hostile enemy)
	{
		mObject = enemy.MoC;
		eObject = enemy;
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
			
		}
		else
		{
			_baseSpeed = mObject.baseSpeeds[eObject.currentBlockIndex];
		}
	}
	
	public void MoveObject() 
	{
		float speedFinal;
		Vector3 shipVelocity;

		//FIGURE OUT IF ANYTHING SHOULD BE MANIPULATING THE SHIP'S SPEED
		mObject.speedMod = mObject.FindSpeedModifiers();

        //CALCULATE FINAL SPEED
        speedFinal = (mObject.baseSpeeds[eObject.currentBlockIndex] * mObject.speedMod);

            //OVERRIDE FINAL SPEED
        if (mObject.isSpeedOverwritten) {
			speedFinal = mObject.speedOverride;
		}

		float push = (Mathf.Sin(Time.time + mObject.waveOffset)) * mObject.waveHeight;

		//SHIP SHOULD ALWAYS BE MOVING FORWARD
		if(mObject.horizontalMoveRight)
			shipVelocity = new Vector3(push,0, 1).normalized;
		else
			shipVelocity = new Vector3(push,0, -1).normalized;

		//FINALLY TIME TO MOVE THE SHIP
		eObject.rB.MovePosition (mObject.movableObject.transform.position + shipVelocity * speedFinal * Time.deltaTime);


	}

}
