using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpinEffect : MonoBehaviour {

	public float min = 0.75f;
	public float max = 2f;
	private float randNum;
	private float randNum2;
	private float randNum3;
	public bool constantRotation = true;

	private Rigidbody rB;

	void Start() {
		randNum = Random.Range (min, max);
		randNum2 = Random.Range (min, max);
		randNum3 = Random.Range (min, max);

		rB = GetComponent<Rigidbody>();

		rB.angularVelocity = new Vector3 (Random.value * randNum, Random.value * randNum2, Random.value * randNum3);
	}

//	void Update() {
//		if(constantRotation)
//			rB.angularVelocity = new Vector3 (Random.value * randNum, Random.value * randNum2, Random.value * randNum3);
//	}
}
