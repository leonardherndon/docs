using UnityEngine;
using System.Collections;

public class TestCamera : MonoBehaviour {

	public GameObject PS;


	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = new Vector3 (0, 30f, PS.transform.position.z);
	}
}
