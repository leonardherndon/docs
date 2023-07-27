using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class ObjectSpawner : MonoBehaviour {

	public GameObject sourcedObject;
    public LevelEntityManager LEM;
    [Wrap(0,12)]
    public int startLane;

    public int MovementStackID;
	public GameObject spawnedObjectsHolder;
	public string uniqueObjectReferenceID;
	public ObjectTag objectTag;
	public CollisionLayerType collisionLayer;
	public bool leftSpawn = false;
	public SpawnPosition spawnType = SpawnPosition.Right;
	protected bool hasSpawned = false;
    public bool isVirtual = false;

    //array where we keep our active clones
    public GameObject[] objectContainer;

	// Use this for initialization
	public void Start () {
//		Debug.Log(" ");
//		Debug.Log("Called Object Spawner Start()");
//		Debug.Log(" ");
		spawnedObjectsHolder = GameObject.Find ("Spawned Objects Holder");
		if (!gameObject.GetComponent<RocketSpawner>()) {
			if (gameObject.GetComponent<MeshRenderer> () != null)
				gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}
		objectContainer = new GameObject[1];
	}

	//TODO REMOVED STARTLANE IDs FROM SPAWN()
	public virtual void Spawn()
	{
		if (!hasSpawned) {
			hasSpawned = true;
			objectContainer [0] = FastPoolManager.GetPool (sourcedObject).FastInstantiate ();
			objectContainer [0].transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
			GameManager.Instance.fastPoolObjects.Add (objectContainer [0]);	

			//Set the Objects Reference Holder
		
			objectContainer [0].name = objectContainer [0].name + Random.Range (100, 999);


			//Sets the movableObject 
			gameObject.GetComponent<MovableObjectController> ().movableObject = objectContainer [0];
			if (objectContainer [0].GetComponent<Hostile> () != null) {

                if(gameObject.GetComponent<CollisionController>() != null)
                    objectContainer[0].GetComponent<Hostile>().CoC = gameObject.GetComponent<CollisionController>();

                objectContainer[0].GetComponent<Hostile>().MoC = gameObject.GetComponent<MovableObjectController>();
                objectContainer[0].GetComponent<Hostile>().CoC = gameObject.GetComponent<CollisionController>();

                if (objectTag == ObjectTag.Enemy || objectTag == ObjectTag.Obstacle || objectTag == ObjectTag.Door || objectTag == ObjectTag.Scene) {

                    objectContainer[0].GetComponent<Hostile>().isVirtual = isVirtual;
                    objectContainer [0].GetComponent<Hostile> ().uniqueTileReferenceID = uniqueObjectReferenceID;
					objectContainer [0].gameObject.name = sourcedObject.name + "|" + uniqueObjectReferenceID;
					

					//Sets the enemy starting current/destination lane			
					objectContainer [0].GetComponent<Hostile> ().enemyLaneCurrent = objectContainer [0].GetComponent<Hostile> ().MoC.startLane;
					objectContainer [0].GetComponent<Hostile> ().enemyLaneDestination = objectContainer [0].GetComponent<Hostile> ().MoC.startLane;

				}
			}

			if (objectContainer [0].GetComponent<RocketSpawner> () != null) {
				objectContainer [0].GetComponent<RocketSpawner> ().rocketColorIndex = gameObject.GetComponent<CollisionController> ().CSM.CurrentColor;
			}



			//SET TAG AND COLLISION LAYER
			objectContainer [0].tag = objectTag.ToString ();

			objectContainer [0].layer = GameManager.Instance.GetCollisionLayer (collisionLayer);
			
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
			FastPoolManager.GetPool(sourcedObject).FastDestroy(objectContainer[0]);
	}


}
