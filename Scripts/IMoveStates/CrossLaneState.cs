using UnityEngine;
using System.Collections;
using DG.Tweening;
using Chronos;

public class CrossLaneState : IMoveState {

    public string stateName = "CrossLaneState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	public Quaternion rotateThis;
	private bool isMovementSet;
	private Vector3 myVector3Angle;

	//Movement Params
	private float _baseSpeed;
	private bool _isMoveSet;
	private float _crossLaneAngle;
	private bool _crossLaneRotate;
	
	public CrossLaneState (Hostile enemy)
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
			_baseSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].baseSpeed;
			_isMoveSet = mObject.moveList.MoveStack[eObject.currentBlockIndex].isMovementSet;
			_crossLaneAngle = mObject.moveList.MoveStack[eObject.currentBlockIndex].crossLaneAngle;
			_crossLaneRotate = mObject.moveList.MoveStack[eObject.currentBlockIndex].crossLaneRotate;
		}
		else
		{
			_baseSpeed = mObject.baseSpeeds[eObject.currentBlockIndex];
			_isMoveSet = mObject.moveList.MoveStack[eObject.currentBlockIndex].isMovementSet;
			_crossLaneAngle = mObject.crossLaneAngles[eObject.currentBlockIndex];
			_crossLaneRotate = mObject.crossLaneRotates[eObject.currentBlockIndex];
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
		
		//Debug.Log ("MOVABLE OBJECT SCRIPT | GameObject : " + mObject.gameObject.name + " | CurrentState: " + stateName);
		if (!_isMoveSet) {
			_isMoveSet = true;
//			if (mObject.movableObject.GetComponent<EnemyShip> ()) {
//				mObject.movableObject.GetComponent<EnemyShip> ().shipLaneTween.Kill ();
//			}
			if (_crossLaneRotate) {	
				rotateThis = Quaternion.identity;

				if (mObject.randomizeCrossLaneAngle)
					_crossLaneAngle = Random.Range (-60f, 60f);
				
				rotateThis.eulerAngles = new Vector3 (0, 0, _crossLaneAngle);

				eObject.transform.Rotate (rotateThis.eulerAngles);
				//mObject.movableObject.GetComponent<Rigidbody> ().rotation = rotateThis;
			} else {
				myVector3Angle = Quaternion.AngleAxis(mObject.crossLaneAngles[eObject.currentBlockIndex], Vector3.forward) * -Vector3.right;
				//myVector3Angle = Vector3 (0, Mathf.Sin (Mathf.Deg2Rad * mObject.crossLaneAngle), Mathf.Cos (Mathf.Deg2Rad * mObject.crossLaneAngle));
			}
		}

		if (_crossLaneRotate) {	
			eObject.rB.MovePosition (eObject.transform.position + -eObject.transform.right * speedFinal * eObject.GetComponent<Timeline> ().deltaTime);
		} else {
			eObject.rB.MovePosition (eObject.transform.position + myVector3Angle * speedFinal * eObject.GetComponent<Timeline>().deltaTime);
		}

	}

}