using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneMap : MonoBehaviour {

	public int mapID;
	public Vector2 mapSize = new Vector2(1,LevelEditorManager.NUMBEROFLANES);
	public Vector2 gridSize;
	[SerializeField]
	public List<EncounterMap> encounters = new List<EncounterMap>();
	public Object[] spriteReferences;
	public Vector2 gridInUnits = new Vector2 ();
	public int tileID = 0;
	public GameObject tiles;
	public float centerX;
	public float centerY;


	public Sprite currentTileBrush {
		get { return spriteReferences [tileID] as Sprite;}
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnGUI() {
		
	}

	void OnDrawGizmos() {

		var tempEncounters = gameObject.GetComponentsInChildren<EncounterMap> ();
		var tempID = 0;
		encounters.Clear ();

		foreach (EncounterMap temp in tempEncounters) {
			if (temp.GetComponent<EncounterLayer> () == null) {
				temp.encounterOrderID = tempID;
				encounters.Add (temp);
				//Debug.Log ("Temp Name: " + temp.name);
				tempID++;
			}
		}

		//Calculating Zone Map Size X Zone 
		//Zone Map Size Y will ALWAYS be LevelEditorManager.NUMBEROFLANES

		mapSize.x = 0;

		if (encounters.Count > 0) {
			if (encounters [0]) {
				for (int i = 0; i < encounters.Count; i++) {
					mapSize.x = mapSize.x + encounters [i].mapLength;
				}
			} else {
				mapSize.x = 1;
			}
		} else {
			mapSize.x = 1;
		}

		gridSize = new Vector2 (LevelEditorManager.tileSize.x * mapSize.x, LevelEditorManager.tileSize.y * mapSize.y);

		Gizmos.color = new Color (0.6f, 0.28f, 0.18f, 1f);
		centerX = ((mapSize.x * LevelEditorManager.tileSize.x) / 2);
		centerY = ((mapSize.y * LevelEditorManager.tileSize.y) / -2);

		Gizmos.DrawWireCube (new Vector3 (centerX, centerY, 0), new Vector3(gridSize.x, gridSize.y, 0));
	}

	void OnDrawGizmosSelected() {

	}
}
