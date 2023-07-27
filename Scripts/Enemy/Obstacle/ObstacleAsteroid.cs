using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ObstacleAsteroid : Hostile {

	public bool isComet = false;
	public GameObject[] rocks;
	public GameObject sourceObject;
	private int randomRockGenerator;
	public int startLane;
	public GameObject spawnedObjectsHolder;
	public string uniqueObjectReferenceID;
	public ObjectTag objectTag;
	public CollisionLayerType collisionLayer;
	public bool leftSpawn = false;
	private bool hasSpawned = false;

	//array where we keep our active clones
	public GameObject[] asteroidContainer;

    protected override void Start() {
		base.Start ();

		if (!isComet) {
			//Debug.Log ("Rocks:" + rocks.Length);
			//Debug.Log ("Random Rock Generator:" + randomRockGenerator);
            if(rocks.Length > 0) {  
			randomRockGenerator = Random.Range (0, rocks.Length-1);
            if (randomRockGenerator < 0)
                randomRockGenerator = 0;
			asteroidContainer = new GameObject[1];
			sourceObject = rocks [randomRockGenerator];
			Spawn ();
            }
		}
    }

    
    public void OnTriggerExit(Collider other)
    {

    }

    public void Spawn()
	{
		if (!hasSpawned) {
			hasSpawned = true;

			asteroidContainer [0] = FastPoolManager.GetPool (sourceObject).FastInstantiate ();

			asteroidContainer [0].transform.parent = transform;
			GameManager.Instance.fastPoolObjects.Add (asteroidContainer [0]);	

			//Set the Objects Reference Holder

			asteroidContainer [0].name = asteroidContainer [0].name + Random.Range (100, 999);
			asteroidContainer [0].transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);

			//Debug.Log (asteroidContainer [0].name + "BEFORE Object Tag is: " + asteroidContainer [0].tag);
			//SET TAG AND COLLISION LAYER
			asteroidContainer [0].tag = objectTag.ToString ();
			//Debug.Log (asteroidContainer [0].name + "AFTER Object Tag is: " + asteroidContainer [0].tag);
			asteroidContainer [0].layer = GameManager.Instance.GetCollisionLayer (collisionLayer);
//			gameObject.GetComponent<Rigidbody>()
			//			objectContainer [0].transform.position = new Vector3(LaneManager.Instance.laneArray[objectContainer [0].GetComponent<ObjectReferenceHolder>().MOD.startLane].x, transform.position.y, transform.position.z);
			//Debug.Log (objectContainer [0].gameObject.name+" "+objectContainer[0].GetInstanceID()+": "+objectContainer [0].transform.position);

		}		
	}


	public void DestroyObjects()
	{
		//Get the pool for our source game object.
		//If it's not exists - it will be created automatically with default settings.
		//Note that you must always provide the SOURCE game object and NOT a clone!
		//FastPool fastPool = FastPoolManager.GetPool(spawnedObject);

		//Cache our clone.
		//fastPool.FastDestroy(objectContainer[0]);


		//Or you can do it all in one line:
		FastPoolManager.GetPool(rocks[randomRockGenerator]).FastDestroy(asteroidContainer[0]);
	}
}
