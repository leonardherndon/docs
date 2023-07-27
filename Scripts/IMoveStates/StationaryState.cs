using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class StationaryState : IMoveState {

	public string stateName = "StationaryState";

    private readonly Hostile eObject;
    private readonly MovableObjectController mObject;

	public StationaryState (Hostile enemy)
	{
		mObject = enemy.MoC;
		eObject = enemy;
	}

	public void UpdateState() {
		MoveObject ();
	}

	public void Update()
	{
		
	}

	private void MoveObject() 
	{
		//Debug.Log("MOVABLE OBJECT SCRIPT | GameObject : " + eObject.gameObject.name + " | CurrentState: " + stateName); 
		return;
	}
}
