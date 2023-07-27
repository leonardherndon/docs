using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Chronos;

public class HunterMineState : IMoveState {

    public string stateName = "WaypointState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	private bool _isMoveSet;

	//Movement Params
	private float _baseSpeed;
	private float _alignSpeed;
	private float _detectRange;
	
	public HunterMineState (Hostile enemy)
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
		_detectRange = mObject.moveList.MoveStack[eObject.currentBlockIndex].targetDetectRange;
		_alignSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].alignSpeed;
	}

	private void MoveObject() 
	{
		
		if (ColorManager.Instance.isColorWeaker(GameManager.Instance.playerShip.gameObject, eObject.gameObject))
		{
			_alignSpeed *= 4;
			_detectRange *= 2;
		} 
		
		if (!_isMoveSet)
		{
			if(!eObject._targetPositionLocked)
				RotateObjectTowardsTarget(_alignSpeed);
			else
				RotateObjectTowardsTarget(_alignSpeed, eObject.targetPosition);
		}
		
		if (!eObject._targetPositionLocked)
		{
			hitCheck(eObject.targetObject.transform, mObject.moveList.MoveStack[eObject.currentBlockIndex].targetDetectRange);
			return;
		}
		
		if (!_isMoveSet)
		{
			float dashSpeed = 10 / mObject.moveList.MoveStack[eObject.currentBlockIndex].baseSpeed;
			eObject.transform.DOMove(eObject.targetPosition, dashSpeed).SetEase(mObject.moveList.MoveStack[eObject.currentBlockIndex].attackEase).SetDelay(mObject.moveList.MoveStack[eObject.currentBlockIndex].attackDelay).OnComplete(CompleteMove);
			_isMoveSet = true;
		}


	}

	private void CompleteMove()
	{
		eObject.currentMovementCompleted = true;
	}

	/*private void SpeedCheck(int gearSpeed)
	{
		if (eObject._targetPositionLocked)
			return;
		if(GameManager.Instance.playerShip.gearSystem.moveSystem.gearIndex > gearSpeed)
			SetTarget();
	}*/
	
	private void hitCheck(Transform target, float length)
	{
		if (eObject._targetPositionLocked)
			return;
		var transform = eObject.transform;
		
		Vector3 targetDir = target.position - transform.position;
		float angle = Vector3.Angle(targetDir, transform.position);
		Vector3 objectDirection = transform.forward;
		//Debug.Log("Target Angle: " + angle);
		//Vector3.

		RaycastHit[] raycastHits;

		raycastHits = Physics.RaycastAll(transform.position, objectDirection, length).OrderBy(h => h.distance).ToArray();
		//= Physics.RaycastNonAlloc(target.transform.position, targetDir, raycastHits, length);
		
		// Debug.DrawRay(transform.position,(objectDirection * length), Color.blue,2f);
		// Debug.Log("Hits: " + raycastHits.Length);
		for (var i = 0; i < raycastHits.Length; i++)
		{
			// Debug.Log("WaypointState Ray hit: " + raycastHits[i].transform.name);
			var hitlayer = raycastHits[i].transform.gameObject.layer;
			// Debug.DrawRay(transform.position,(objectDirection * length), Color.green,5f);
			
			if (raycastHits[i].transform.name != GameManager.Instance.playerShip.transform.name)
			{
				if (hitlayer == 28)
				{
					// Debug.Log("WaypointState Ray hit a wall");
					return;
				}
			}
			else
			{
				// Debug.DrawRay(transform.position,(objectDirection * length), Color.green,10f);
				SetTarget();
				return;
			}
			
		}
	}

	public void SetTarget()
	{
		eObject.targetPosition = new Vector3(eObject.targetPosition.x + mObject.moveList.MoveStack[eObject.currentBlockIndex].targetPrediction, eObject.targetPosition.y, eObject.targetPosition.z);
		eObject._targetPositionLocked = true;
	}
	
	private void RotateObjectTowardsTarget(float rotateSpeed)
	{
		var transform = eObject.transform;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(eObject.targetObject.transform.position - transform.position), eObject.GetComponent<Timeline>().deltaTime * rotateSpeed);
		transform.position += -(Vector3.forward * eObject.GetComponent<Timeline>().deltaTime * _baseSpeed/2f);
	}
	
	private void RotateObjectTowardsTarget(float rotateSpeed, Vector3 positionPrediction)
	{
		var transform = eObject.transform;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(positionPrediction - transform.position), eObject.GetComponent<Timeline>().deltaTime * rotateSpeed);
		transform.position += -(Vector3.forward * eObject.GetComponent<Timeline>().deltaTime * _baseSpeed/2f);
	}

}
