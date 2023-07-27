using UnityEngine;
using System.Collections;
using DG.Tweening;
using Chronos;

public class MagMoveState : IMoveState {

    public string stateName = "MagMoveState";
	private readonly Hostile eObject;
	private readonly MovableObjectController mObject;
	public Quaternion rotateThis;
	private Transform target;
	private Vector3 step;
    
	//Movement Params
	private float _objectActivationRadius;
	private float _magFactor;
    public MagMoveState (Hostile enemy)
	{
		mObject = enemy.MoC;
		eObject = enemy;
		target = GameManager.Instance.playerShip.transform;
		step = Vector3.zero;
	}

	public void UpdateState() {
		GatherParams(mObject.useBlockParams);
		CalculateMagnetism ();
		MagMove ();
	}

	public void Update() {}


	private void GatherParams(bool useBlockParams = false)
	{
		if (useBlockParams)
		{
			_objectActivationRadius = mObject.moveList.MoveStack[eObject.currentBlockIndex].objectActivationRadius;
			_magFactor = mObject.moveList.MoveStack[eObject.currentBlockIndex].magFactor;
		}
		else
		{
			_objectActivationRadius = mObject.objectActivationRadii[eObject.currentBlockIndex];
			_magFactor = mObject.magFactors[eObject.currentBlockIndex];
		}
	}

	private void CalculateMagnetism() {
		

	}

	private void MagMove() 
	{
		//Debug.Log ("MagMove: MoveToObject()");

		//FIGURE OUT IF ANYTHING SHOULD BE MANIPULATING THE SHIP'S SPEED
		mObject.speedMod = mObject.FindSpeedModifiers();
		
		float vectorDistance = Vector3.Distance (eObject.transform.position, GameManager.Instance.playerShip.transform.position);
		

        //If Player is close enough to object
            
        if (vectorDistance < _objectActivationRadius * 1.15f) {
			eObject.rB.drag = 0.6f;
			eObject.rB.isKinematic = true;

			if(_magFactor > 0)
				eObject.transform.LookAt(target);

			//SET MAG SPEED BASED ON CURRENT GEAR LEVEL
			// float magMultiplier = GameManager.Instance.playerShip.gearSystem.moveSystem.gearIndex - 0.35f;
			float magMultiplier = 2 - 0.35f;
			
            
            //REDUCE SPEED BASED ON COLOR FREQUENCY
            if (ColorManager.Instance.isColorStrongerOrEqual(GameManager.Instance.playerShip.gameObject, eObject.gameObject))
	            magMultiplier = magMultiplier / 1.5f;
            
            if (magMultiplier < 0)
	            magMultiplier = 0.05f;
            
            
            if (magMultiplier > 2f)
	            magMultiplier = 2f;
            

            float moveStep = (eObject.GetComponent<Timeline>().deltaTime * _magFactor) * mObject.speedMod * magMultiplier;
            //Debug.Log("GearIndex: " + GameManager.Instance.playerShip.PGS.moveSystem.gearIndex  + " | MoveStep: " +moveStep);
            //THIS IS STUPID DON'T KNOW WHAT THE RIGIDBODY VERSION OF THIS MOVMENT IS NOT WORKING. MUST INVESTIGATE FURTHER IF THIS BECOMES A PROBLEM.
            eObject.transform.position = Vector3.MoveTowards(eObject.transform.position, target.transform.position, moveStep);
            //eObject.rB.MovePosition(eObject.transform.position + step);
               

        } else 
        {
			if (eObject.rB.isKinematic) {
				eObject.rB.isKinematic = false;
				eObject.rB.drag = 2.5f;
			}
		}
        

	}

}
