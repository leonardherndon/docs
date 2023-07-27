using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Chronos;
using Language.Lua;

public class WaypointState : IMoveState {

    public string stateName = "WaypointState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	private bool _isMoveSet;

	//Movement Params
	private float _baseSpeed;
	private float _alignSpeed;
	private float _detectRange;
	private bool _runLocal;
	private WaypointSet _waypointSet;
	private Vector3[] _waypoints;
	
	public WaypointState (Hostile enemy)
	{
		mObject = enemy.MoC;
		eObject = enemy;
	}

	public void UpdateState()
	{
		GatherParams(mObject.useBlockParams);
		CleanWaypoints();
		MoveObject();
	}

	public void Update()
	{

	}

	private void CleanWaypoints()
	{
		for (int i = 0; i > _waypoints.Length; i++)
		{
			_waypoints[i] = GameManager.Instance.mainCamera.GetComponent<Camera>().ScreenToWorldPoint(_waypoints[i]);
		}
	}
	private void GatherParams(bool useBlockParams = false)
	{
		if (useBlockParams)
		{
			_baseSpeed = mObject.moveList.MoveStack[eObject.currentBlockIndex].waypointGroup.y;
			_waypointSet = mObject.GetComponent<WaypointManager>().waypointSets[(int)mObject.moveList.MoveStack[eObject.currentBlockIndex].waypointGroup.x];
			_runLocal = _waypointSet.isLocal;
			_waypoints = _waypointSet.waypoints;

		}
		else
		{
			_baseSpeed = mObject.waypointGroup.y;
			_waypointSet = mObject.GetComponent<WaypointManager>().waypointSets[(int)mObject.waypointGroup.x];
			_waypoints = _waypointSet.waypoints;
		}
	}

	private void MoveObject() 
	{
		if (ColorManager.Instance.isColorWeaker(GameManager.Instance.playerShip.gameObject, eObject.gameObject))
		{
			//ADD COLOR CHANGE BEHAVIOURS HERE
		} 
		
		if (!_isMoveSet)
		{
			
		}

		Debug.Log("Target is locked");
		if (!_isMoveSet)
		{
			float moveSpeed = 10 / mObject.moveList.MoveStack[eObject.currentBlockIndex].baseSpeed;
			
			if (_runLocal == true)
			{
				eObject.transform.DOLocalPath(_waypoints, moveSpeed).SetEase(Ease.InOutQuint).OnComplete(CompleteMove);
			}
			else
			{
				eObject.transform.DOPath(_waypoints, moveSpeed).SetEase(Ease.InOutQuint).OnComplete(CompleteMove);
			}

			_isMoveSet = true;
		}


	}

	private void CompleteMove()
	{
		eObject.currentMovementCompleted = true;
	}
	
	

}
