using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectsHolder : MonoBehaviour {

	// Use this for initialization
	void Awake ()
	{
		GameManager.Instance.spawnedObjectsHolder = this.gameObject;
	}
	
}
