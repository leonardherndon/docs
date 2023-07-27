using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionIdentifier : MonoBehaviour {
	
	private void OnCollisionEnter(Collision other)
	{
		Debug.Log("(" + gameObject.name + ") is [ENTER] colliding with (" + other.gameObject.name + ")");
	}
	
	// Update is called once per frame
	private void OnCollisionStay(Collision other)
	{
		Debug.Log("(" + gameObject.name + ") is [STAY] colliding with (" + other.gameObject.name + ")");
	}
	
	private void OnCollisionExit(Collision other)
	{
		Debug.Log("(" + gameObject.name + ") is [EXIT] colliding with (" + other.gameObject.name + ")");
	}
}
