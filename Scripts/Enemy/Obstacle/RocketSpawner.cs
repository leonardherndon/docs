using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class RocketSpawner : ObjectSpawner {

	public GameColor rocketColorIndex;
	public int rocketStartLane;
	public bool detectionActive;
	public bool detectionEnabled = false;
	private int playerColorCache;
    public GameObject targetEffect;

    // Update is called once per frame
    void FixedUpdate () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "PlayerShip") {
            //if (detectionEnabled) {
            AudioManager.Instance.PlayClipWrap(5, 17);
            gameObject.GetComponent<RocketSpawner> ().detectionActive = true;
            GameObject TargetIcon = Instantiate(
	            targetEffect,
	            new Vector3(
		            GameManager.Instance.playerShip.transform.position.x, 
		            GameManager.Instance.playerShip.transform.position.y, 
		            GameManager.Instance.playerShip.transform.position.z - 3f
		        ), 
	            Quaternion.identity
            );
            TargetIcon.transform.parent = GameManager.Instance.playerShip.transform;
            //}
        } 
	}

	// Update is called once per frame
	public override void Spawn () 
	{
		//Debug.Log ("Testing Spawn Function");
		if (hasSpawned)
			return;
		if (detectionActive == false)
			return;
       
        hasSpawned = true;
		objectContainer [0] = FastPoolManager.GetPool (sourcedObject).FastInstantiate ();
		objectContainer [0].transform.parent = spawnedObjectsHolder.transform;
		GameManager.Instance.fastPoolObjects.Add (objectContainer [0]);	

		//Set the Objects Reference Holder

		objectContainer [0].name = objectContainer [0].name + Random.Range (100, 999);

		objectContainer [0].gameObject.GetComponent<Hostile> ().MoC.startLane = rocketStartLane;
		objectContainer [0].transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);


		//Sets the movableObject 
		objectContainer [0].gameObject.GetComponent<Hostile> ().MoC.movableObject = objectContainer [0];

		objectContainer [0].GetComponent<Hostile> ().uniqueTileReferenceID = uniqueObjectReferenceID;
		objectContainer [0].gameObject.name = sourcedObject.name + "|" + uniqueObjectReferenceID;

		objectContainer [0].GetComponent<Hostile> ().CoC.CSM.ChromaShift(rocketColorIndex);

		//Sets the enemy starting current/destination lane			
		objectContainer [0].GetComponent<Hostile> ().enemyLaneCurrent = rocketStartLane;
		objectContainer [0].GetComponent<Hostile> ().enemyLaneDestination = rocketStartLane;
		objectContainer [0].GetComponent<Hostile> ().CSM.ChromaShift(rocketColorIndex);


		//Debug.Log (objectContainer [0].name + "BEFORE Object Tag is: " + objectContainer [0].tag);
		//SET TAG AND COLLISION LAYER
		objectContainer [0].tag = objectTag.ToString ();
		//Debug.Log (objectContainer [0].name + "AFTER Object Tag is: " + objectContainer [0].tag);
		objectContainer [0].layer = GameManager.Instance.GetCollisionLayer (collisionLayer);

		//			objectContainer [0].transform.position = new Vector3(LaneManager.Instance.laneArray[objectContainer [0].GetComponent<ObjectReferenceHolder>().MOD.startLane].x, transform.position.y, transform.position.z);
		//Debug.Log (objectContainer [0].gameObject.name+" "+objectContainer[0].GetInstanceID()+": "+objectContainer [0].transform.position);

		
	}
}
