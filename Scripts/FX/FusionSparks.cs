using UnityEngine;
using System.Collections;

public class FusionSparks : MonoBehaviour {

	private ParticleSystem myParticleSystem;

	// Use this for initialization
	void Start () {
		myParticleSystem = gameObject.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (myParticleSystem.isStopped)
			gameObject.SetActive (false);
	}
}
