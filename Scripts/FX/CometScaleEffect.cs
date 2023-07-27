using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometScaleEffect : MonoBehaviour {

	public float minScaleFactor = 0.5f;
	public float maxScaleFactor = 1.2f;

	void Start() {
		float randNum = Random.Range (minScaleFactor, maxScaleFactor);
		gameObject.transform.localScale = Vector3.one * randNum;
	}
}
