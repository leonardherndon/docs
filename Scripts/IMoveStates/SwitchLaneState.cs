using UnityEngine;
using System.Collections;
using Chronos;

public class SwitchLaneState : IMoveState {

    public string stateName = "SwitchLaneState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	private bool moveSet = false;

	//Movement Params
	private bool _playerLaneSwitch;
	private float _baseSpeed;
	private int _destinationLane;
	
	public SwitchLaneState (Hostile hostile)
	{
		mObject = hostile.MoC;
		eObject = hostile;
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
			_playerLaneSwitch = mObject.moveList.MoveStack[eObject.currentBlockIndex].switchToPlayerlane;
			_baseSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].baseSpeed;
			_destinationLane = mObject.moveList.MoveStack[eObject.currentBlockIndex].destinationLane;
		}
		else
		{
			_playerLaneSwitch = mObject.playerLaneSwitches[eObject.currentBlockIndex];
			_baseSpeed = mObject.baseSpeeds[eObject.currentBlockIndex];
			_destinationLane = mObject.laneSwitchDestinationLanes[eObject.currentBlockIndex];
		}
	}

	private void MoveObject() 
	{
        if (!moveSet) {
            moveSet = true;
           
            if (_playerLaneSwitch)
                mObject.movableObject.GetComponent<EnemyShip>().enemyLaneDestination = GameManager.Instance.playerShip.shipLaneCurrent;
            else
                //Debug.Log ("Switch Lane State | " + mObject.name + "\nCurrent Lane: " + mObject.startLane + " | Lane Destination: " + mObject.switchDestinationLane);
                mObject.movableObject.GetComponent<EnemyShip>().enemyLaneDestination = _destinationLane;
        }
		float speedFinal;
		Vector3 shipVelocity = new Vector3 (0, 0, 0);

		//FIGURE OUT IF ANYTHING SHOULD BE MANIPULATING THE SHIP'S SPEED
		mObject.speedMod = mObject.FindSpeedModifiers();

        //CALCULATE FINAL SPEED

        speedFinal = (_baseSpeed * mObject.speedMod);

        
		//OVERRIDE FINAL SPEED
		if (mObject.isSpeedOverwritten) {
			speedFinal = mObject.speedOverride;
		}

		//SHIP LANE SWITCHING IS DONE BY MANIPULATION OF THE SHIPLANEDESTINATION VARIABLE.
		if (mObject.movableObject.GetComponent<Hostile>().enemyLaneCurrent < mObject.movableObject.GetComponent<Hostile>().enemyLaneDestination) {
			shipVelocity = new Vector3(0,-1,0).normalized;
			///Debug.Log ("Ship Moving Down");
			//targetRot = Quaternion.AngleAxis (tiltRotateDegree, transform.forward);
		} else if (mObject.movableObject.GetComponent<Hostile>().enemyLaneCurrent > mObject.movableObject.GetComponent<Hostile>().enemyLaneDestination) {
			shipVelocity = new Vector3(0,1,0).normalized;
			//Debug.Log ("Ship Moving Up");
			//targetRot = Quaternion.AngleAxis (-tiltRotateDegree, transform.forward);
		}

		mObject.movableObject.GetComponent<Hostile>().rB.MovePosition (mObject.movableObject.transform.position + shipVelocity * speedFinal * eObject.GetComponent<Timeline>().deltaTime);

		//Debug.Log("MOVABLE OBJECT SCRIPT | GameObject : " + mObject.gameObject.name + " | CurrentState: " + stateName);

	}
}
