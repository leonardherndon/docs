using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class MoveSwitchState : IMoveState {

    public string stateName = "MoveSwitchState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	private bool moveSet = false;

	//Movement Params
	private bool _playerLaneSwitch;
	private float _baseSpeed;


	public MoveSwitchState (Hostile hostile)
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
		}
		else
		{
			_playerLaneSwitch = mObject.playerLaneSwitches[eObject.currentBlockIndex];
			_baseSpeed = mObject.baseSpeeds[eObject.currentBlockIndex];
		}
	}

	private void MoveObject() 
	{

		float speedFinal;
		Vector3 shipVelocity;
		float vertMove = 0;


		//SETUP VARS FOR LANE SWITCHING
		mObject.movableObject.GetComponent<Hostile>().destinationLanePosition = new Vector3 (mObject.movableObject.transform.position.x, LaneManager.Instance.laneArray [mObject.movableObject.GetComponent<Hostile>().enemyLaneDestination].y, 0);
		//Debug.Log ("Destination Lane Position: " + destinationLanePosition);

		mObject.movableObject.GetComponent<Hostile>().laneDistance = Vector3.Distance (LaneManager.Instance.laneArray [mObject.movableObject.GetComponent<Hostile>().enemyLaneDestination], LaneManager.Instance.laneArray [mObject.movableObject.GetComponent<Hostile>().enemyLaneCurrent]);
		//		//Debug.Log ("Distance from lane: " + laneDistance);

		mObject.movableObject.GetComponent<Hostile>().distanceFromEnemyToDestinationLane = Vector3.Distance (mObject.movableObject.GetComponent<Hostile>().destinationLanePosition, new Vector3(mObject.movableObject.transform.position.x, mObject.movableObject.transform.position.y,0));

		mObject.movableObject.GetComponent<Hostile>().fracJourney = mObject.movableObject.GetComponent<Hostile>().distanceFromEnemyToDestinationLane / mObject.movableObject.GetComponent<Hostile>().laneDistance;

		if (!moveSet) {
			moveSet = true;
			mObject.movableObject.GetComponent<EnemyShip>().enemyLaneDestination = _playerLaneSwitch ? GameManager.Instance.playerShip.shipLaneCurrent : mObject.switchDestinationLane;
		}



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
			vertMove = -mObject.moveVertSpeed;
			mObject.movableObject.GetComponent<Hostile>().targetRot = Quaternion.AngleAxis (-(mObject.movableObject.GetComponent<Hostile>().tiltRotateDegree * mObject.movableObject.GetComponent<Hostile>().fracJourney), -mObject.transform.right);
			///Debug.Log ("Ship Moving Down");
			//targetRot = Quaternion.AngleAxis (tiltRotateDegree, transform.forward);
		} else if (mObject.movableObject.GetComponent<Hostile>().enemyLaneCurrent > mObject.movableObject.GetComponent<Hostile>().enemyLaneDestination) {
			vertMove = mObject.moveVertSpeed;
			//Debug.Log ("Ship Moving Up");
			mObject.movableObject.GetComponent<Hostile>().targetRot = Quaternion.AngleAxis (mObject.movableObject.GetComponent<Hostile>().tiltRotateDegree * mObject.movableObject.GetComponent<Hostile>().fracJourney, -mObject.transform.right);
			//targetRot = Quaternion.AngleAxis (-tiltRotateDegree, transform.forward);
		}


		//SHIP SHOULD ALWAYS BE MOVING FORWARD
		if (mObject.horizontalMoveRight) {
			Vector3 rot = eObject.transform.rotation.eulerAngles;
			rot = new Vector3(rot.x,rot.y,180);
			eObject.transform.rotation = Quaternion.Euler(rot);
			//Debug.Log ("Rotation should be working");
			shipVelocity = new Vector3 (1, vertMove, 0).normalized;
		} else {
			shipVelocity = new Vector3 (-1, vertMove, 0).normalized;
		}

//		if (mObject.movableObject.GetComponent<Hostile>().enemyLaneCurrent != mObject.movableObject.GetComponent<Hostile>().enemyLaneDestination) {
//			mObject.movableObject.transform.rotation = Quaternion.Lerp (mObject.movableObject.GetComponent<Hostile>().defaultRot, mObject.movableObject.GetComponent<Hostile>().targetRot,  eObject.GetComponent<Timeline>().deltaTime * 1);
//		}
		//Debug.Log ("VertMove: " + vertMove);
		mObject.movableObject.GetComponent<Hostile>().rB.MovePosition (mObject.movableObject.transform.position + shipVelocity * speedFinal * eObject.GetComponent<Timeline>().deltaTime);

		//Debug.Log("MOVABLE OBJECT SCRIPT | GameObject : " + mObject.gameObject.name + " | CurrentState: " + stateName);

	}
}
