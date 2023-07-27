using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Managers;
using UnityEngine;

public class PlayerAlarmSystem : MonoBehaviour {


	private AudioSource aS;
	private PlayerInputController PIC;

	// Use this for initialization
	void Start () {
		aS = gameObject.GetComponent<AudioSource> ();	
		PIC = GameManager.Instance.playerShip.PIC;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GameStateManager.Instance.CurrentState.StateType == GameStateType.Gameplay) {
			if (GameManager.Instance.playerShip.fusionCoreInventory.Count > 0 || aS.isPlaying == true) {
				return;
			}
			aS.Play ();
		}
	}
}
