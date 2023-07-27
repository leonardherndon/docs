using System;
using UnityEngine;
using System.Collections;
using Chronos;
using DG.Tweening.Plugins.Options;

public class HoverSwitchState : IMoveState {

    public string stateName = "HoverSwitchState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	private bool finishedHovering = false;
	private bool moveSet = false;
	[ShowOnly] public float timer;

	private Vector3 targetPosition;
	private StickWithPlayer _stickWithPlayer;
	private PlayerShip _playerShip;
	private EnemyShip _enemyShip;
	private Timeline _timeline;
	private bool _isBehindPlayer;

	private Rigidbody _rigidbody;
	private Rigidbody _playerRB;

	
	//Movement Params
	private int _destinationLane;
	private float _hoverDuration;
	
	public HoverSwitchState (Hostile enemy)
	{
		mObject = enemy.MoC;
		eObject = enemy;
		
		_stickWithPlayer = GameManager.Instance.playerShip.hoverCollider.GetComponent<StickWithPlayer>();
		_playerShip = GameManager.Instance.playerShip;
		_enemyShip = mObject.GetComponent<EnemyShip>();
		_timeline = eObject.GetComponent<Timeline>();
		_rigidbody = eObject.GetComponent<Rigidbody>();
		_playerRB = GameManager.Instance.playerShip.GetComponent<Rigidbody>();

	}

	public void UpdateState()
	{
		// These are only going to run on the update.
//		GatherParams(mObject.useBlockParams);
//		MoveObject();
	}

	public void Update()
	{
		GatherParams(mObject.useBlockParams);
		MoveObject();
	}

	private void GatherParams(bool useBlockParams = false)
	{
		if (useBlockParams)
		{
			_destinationLane = mObject.moveList.MoveStack[eObject.currentBlockIndex].destinationLane;
			_hoverDuration = mObject.moveList.MoveStack[eObject.currentBlockIndex].hoverDuration;
		}
		else
		{
			_destinationLane = mObject.laneSwitchDestinationLanes[eObject.currentBlockIndex];
			_hoverDuration = mObject.hoverDuration;
		}
	}
	private void MoveObject()
	{
		if (mObject.isBehindPlayer != null && eObject.currentBlockIndex < mObject.isBehindPlayer.Count)
		{
			_isBehindPlayer = mObject.isBehindPlayer[eObject.currentBlockIndex];
		}
//		MoveObject ();
		var targetLanePos =  mObject.laneSwitchDestinationLanes.Count > eObject.currentBlockIndex 
			? _destinationLane
			: 0;
//		_enemyShip.enemyLaneDestination = mObject.moveList.MoveStack[eObject.currentBlockIndex].switchToPlayerlane 
//			? _playerShip.shipLaneCurrent 
//			: targetLanePos;
		var targetY = (LaneManager.Instance.laneArray.Length > targetLanePos) ? LaneManager.Instance.laneArray[targetLanePos].y : eObject.transform.position.y;

		if (mObject.moveList.MoveStack[eObject.currentBlockIndex].switchToPlayerlane)
		{
			targetY = GameManager.Instance.playerShip.transform.position.y;
		}
		
		var xpos = GameManager.Instance.playerShip.transform.position.x + _stickWithPlayer.playerPositionOffset;

		if (_isBehindPlayer)
		{
			xpos = GameManager.Instance.playerShip.transform.position.x + (_stickWithPlayer.playerPositionOffset * -1);
		}

		
		targetPosition = new Vector3 (_playerShip.transform.position.x + _stickWithPlayer.playerPositionOffset, targetY, 0f);

		var tmpY = Mathf.Lerp(eObject.transform.position.y, targetPosition.y, _timeline.fixedDeltaTime);
		if (tmpY <= targetY * 0.975 && tmpY >= targetY * 1.025)
		{
			tmpY = targetY;
		}
		var tmpPos = new Vector3(xpos, tmpY, eObject.transform.position.z);
//            eObject.rB.MovePosition(tmpPos);
		mObject.movableObject.transform.position = tmpPos;
		
//		Debug.LogFormat("The current velocity is {0} while player velocity is {1}", _rigidbody.velocity, _playerRB.velocity);

//		_rigidbody.AddForce(_playerRB.velocity, ForceMode.VelocityChange);// = _playerRB.velocity;
		var v = _playerRB.velocity.x * Vector3.right;
		_rigidbody.velocity = v;
	}
	/*private void MoveObject()
	{
		var targetLanePos =  mObject.laneSwitchDestinationLanes.Count > eObject.currentBlockIndex 
			? mObject.laneSwitchDestinationLanes[eObject.currentBlockIndex]
			: 0;
		if (!moveSet) {
			moveSet = true;
            if (!eObject.MoC.useNewMovementSystem)
            {
	            _enemyShip.enemyLaneDestination = mObject.switchToPlayerlane 
		            ? _playerShip.shipLaneCurrent 
		            : targetLanePos;
            }
            else
            {
	            _enemyShip.enemyLaneDestination = mObject.moveList.MoveStack[eObject.currentBlockIndex].switchToPlayerlane 
		            ? _playerShip.shipLaneCurrent 
		            : targetLanePos;
            }
		}

		//FIGURE OUT IF ANYTHING SHOULD BE MANIPULATING THE SHIP'S SPEED
		mObject.speedMod = mObject.FindSpeedModifiers();

        if (eObject.isHovering == false && finishedHovering == false) {
            eObject.isHovering = true;
			mObject.StartCoroutine (HoverTimer());
		}

		if (eObject.isHovering) {
            if (eObject.enemyShipTrail != null && eObject.enemyShipTrail.activeSelf)
            {
                Debug.Log("Enemy Trail should be turning off.");
                eObject.enemyShipTrail.SetActive(false);
            }
            
            var targetY = (LaneManager.Instance.laneArray.Length > targetLanePos) ? LaneManager.Instance.laneArray[targetLanePos].y : eObject.transform.position.y;
            targetPosition = new Vector3 (_playerShip.transform.position.x + _stickWithPlayer.playerPositionOffset, targetY, 0f);

            var tmpY = Mathf.Lerp(eObject.transform.position.y, targetPosition.y, _timeline.fixedDeltaTime);
            if (tmpY <= targetY * 0.975 && tmpY >= targetY * 1.025)
            {
	            tmpY = targetY;
            }
            var tmpPos = new Vector3(_playerShip.transform.position.x + _stickWithPlayer.playerPositionOffset, tmpY, eObject.transform.position.z);
//            eObject.rB.MovePosition(tmpPos);
            mObject.movableObject.transform.position = tmpPos;

		}
		if (!finishedHovering) return;
		
		if (eObject.enemyShipTrail != null && !eObject.enemyShipTrail.activeSelf)
		{
			Debug.Log("Enemy Trail should be turning back on.");
			eObject.enemyShipTrail.SetActive(true);
		}

//		if (mObject.movableObject.transform.position.y > (targetPosition.y * 1.025) || mObject.movableObject.transform.position.y < (targetPosition.y * 0.975) ) return;
//		if (eObject.enemyLaneDestination != eObject.enemyLaneCurrent) return;
		eObject.isHovering = false;
		if (!eObject.MoC.useNewMovementSystem)
		{
			eObject.currentState = eObject.firstState == eObject.hoverSwitchState ? eObject.secondState : eObject.thirdState;
		}
		else
		{
//			Debug.Log("Updating currentBlockIndex from the HoverSwitch state.");
//			eObject.currentBlockIndex++;
//			eObject.InitMoveBlock(eObject.currentBlockIndex);
		}
	}*/

	public IEnumerator HoverTimer()
	{

		timer = _timeline.time + _hoverDuration;
		//Debug.Log ("Hover Timer Activated: " + eObject.GetComponent<Timeline>().time + " | " + timer);

		while (_timeline.time < timer) {
			yield return null;
		}
		//Debug.Log ("Hover Timer Finished: " + eObject.GetComponent<Timeline>().time + " | " + timer);
		finishedHovering = true;

        yield return null;

	}

}
