using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	private Vector3 velocity = Vector3.zero;
	
	public float smoothTime = 0.15f;
	
	void FixedUpdate () {
		if (target) {
			Vector3 targetPosition = new Vector3(0f, 30f , target.position.z);
			
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		}
	}
}