using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaSpawner : MonoBehaviour {

	
	public GameObject player;
	public float sectorDistanceTraveled;
	public float spawnIncrement = 28f;
	public int currentArea = 0;
	public List<GameObject> areaSetList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if (GameManager.Instance.gameState == GameStateType.Gameplay) {
			CheckDistance ();
		}
	}

	private void CheckDistance() {
		sectorDistanceTraveled = player.transform.position.z;
		if (currentArea == 0) {
			if (sectorDistanceTraveled >= 10f) {
				SpawnNewArea ();
			}
		} else {
			if (sectorDistanceTraveled >= (spawnIncrement * currentArea)) {
				SpawnNewArea ();
			}
		}

	
	}

	private void SpawnNewArea() {
		Instantiate (areaSetList [Random.Range (0, areaSetList.Count)], new Vector3 (0, 0, (spawnIncrement * (currentArea + 3))), Quaternion.identity);
		currentArea++;
	}
}
