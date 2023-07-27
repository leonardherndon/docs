using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class EndLevelTrigger : MonoBehaviour {
	void OnTriggerEnter(Collider other) {

		if (other.GetComponent<PlayerShip>()) {
			gameObject.GetComponent<AudioSource> ().Play ();
			AudioManager.Instance.PlayClipWrap (3, 5);

		}

	}
}
