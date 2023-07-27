using UnityEngine;
using System.Collections;
using CS_Audio;

public class Pickup : MonoBehaviour {
	public GameObject pickupFX;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void KillObject(AudioClip clip) {
		//Debug.Log ("KillObject | Kill Me");
		AudioManager.Instance.pickUpAudioSource.clip = clip;
		AudioManager.Instance.pickUpAudioSource.Play();
		if (pickupFX)
		{
			GameObject currentPickupFX = Instantiate(pickupFX, transform.position, Quaternion.identity);
			currentPickupFX.transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
		}

		Destroy(this.gameObject);
	}
}
