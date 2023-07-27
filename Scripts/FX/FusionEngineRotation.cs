using System.Collections;
using System.Collections.Generic;
using Chronos;
using UnityEngine;

public class FusionEngineRotation : MonoBehaviour {

	public float localRotationSpeed = 90;
	private bool isRotating = false;
	private Clock _clock;

	// Use this for initialization
	void Start()
	{
		// StartCoroutine(RotateObject(localRotationSpeed, Vector3.right, 1));
		_clock = Timekeeper.instance.Clock("Enemy");
	}

	void Update() {
//		if (isRotating) {
//			return;
//		} else {
		RotateObject (localRotationSpeed, Vector3.right, 1);
//			StartCoroutine (RotateObject (localRotationSpeed, Vector3.right, 1));
//		}
	}

	void RotateObject(float angle, Vector3 axis, float inTime)
	{
//		isRotating = true;
		// calculate rotation speed
		float rotationSpeed = angle / inTime;

//		while (true)
//		{
			// save starting rotation position
			Quaternion startRotation = transform.rotation;

			float deltaAngle = 0;

			// rotate until reaching angle
//			while (deltaAngle < angle)
//			{
				deltaAngle += rotationSpeed * _clock.deltaTime;
				deltaAngle = Mathf.Min(deltaAngle, angle);

				transform.rotation = startRotation * Quaternion.AngleAxis(deltaAngle, axis);

//				yield return null;
//			}
				
//		}

//		isRotating = false; 

	}
}

