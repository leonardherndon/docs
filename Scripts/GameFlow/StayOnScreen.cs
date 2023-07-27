using UnityEngine;
using System.Collections;
using ChromaShift.Scripts.Managers;

public class StayOnScreen: MonoBehaviour {

	public float xPositionOffset = 25f;
	public float yPositionOffset = 25f;
	public float zPositionOffset = 25f;
	public Transform followTarget;

	// Update is called once per frame 
	void Update () {
		if (!followTarget && !GameObject.Find ("PlayerShip") || GameStateManager.Instance.CurrentState.StateType != GameStateType.Gameplay) {
			return;
		} else {
			followTarget = GameObject.Find ("PlayerShip").transform;
		}

		transform.position = Camera.main.ScreenToWorldPoint (new Vector3(xPositionOffset, yPositionOffset, zPositionOffset));
	}

}
