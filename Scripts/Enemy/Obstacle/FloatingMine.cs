using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class FloatingMine : Hostile, IKillable {

    public AudioSource activationSoundSource;
	public GameObject activationLight;
	public bool mineActivated;

    
    public override void OnTriggerEnter (Collider other) {

		if (other.gameObject.name != "PlayerShip")
			return;
		ArmMine();
	}

	public void OnTriggerExit (Collider other) {
		if (other.gameObject.name != "PlayerShip")
			return;
        
		activationLight.SetActive (false);
	}

	public void ArmMine() {
		
		if (mineActivated)
			return;
		activationLight.SetActive (true);

		//play activation sound
	}
}
