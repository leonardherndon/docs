using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowYAxis : MonoBehaviour {

    Transform target;

	// Use this for initialization
	void Start () {
        if (!target)
            target = GameManager.Instance.playerShip.transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
	}
}
