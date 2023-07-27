using UnityEngine;
using System.Collections;
using ChromaShift.Scripts.Managers;

public class StateChangeTrigger : MonoBehaviour {

	
	public BaseGameState triggeredState;


	void State() {
		
	}

	void OnTriggerEnter(Collider other) {
	//Debug.Log ("HIT STATE CHANGE TRIGGER");
		if (other.CompareTag("Player")) {
			GameStateManager.Instance.SwitchState (triggeredState);
		}
	}
}
