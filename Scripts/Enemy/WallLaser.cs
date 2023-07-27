using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WallLaser : MonoBehaviour {

	private bool isSet = false;
	public float aliveTime;
	public CollisionController CoC;
    
	// Use this for initialization
	private void Start()
	{
		transform.DOScale(new Vector3(5, 5, transform.localScale.z), 4);
	}

	private void FixedUpdate()
	{
		if(!isSet)
			return;

		isSet = true;
		Destroy(this, aliveTime);
	}
}
