using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHider : MonoBehaviour
{
	private MeshRenderer mR;
	
	// Use this for initialization
	void Start ()
	{
		mR = gameObject.GetComponent<MeshRenderer>();
		mR.enabled = false;
	}
}
