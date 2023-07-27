using UnityEngine;
using System.Collections;

public class PlayerDeathExplosion : MonoBehaviour {

	public GameObject spawnedObjectsHolder;
	public AudioClip soundFX;
	private AudioSource aS;

	// Use this for initialization
	void Start () {
		spawnedObjectsHolder = GameObject.Find ("PlayerShip");
		transform.parent = spawnedObjectsHolder.transform;
		aS = gameObject.GetComponent<AudioSource> ();
		aS.clip = soundFX;
		aS.PlayOneShot (soundFX);
	}

}
