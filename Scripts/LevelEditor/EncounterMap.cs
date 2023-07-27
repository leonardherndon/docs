using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum EncounterType{Untagged,Pregame,Normal,Cutscene,WarpGate,NullMovement}
public enum EncounterLayerType {Enemy, Nebula, Obstacle, Pickup, Management, Background}

public class EncounterMap : MonoBehaviour {

	public int uniqueEncounterID;
	public int encounterOrderID;
	public int mapLength = 10;
	public bool hasCheckPoint = false;
	public string encounterName;
	public string encounterDescription;
	public int encounterDifficulty; // 1(Easy) - 5(Hardest)
	public EncounterType encounterType;
	public ZoneMap parentZone;
	public Vector3 pos;
	public Vector2 gridBoundry;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	#if UNITY_EDITOR
	virtual public void OnDrawGizmos() {
		//Ecounter Area Fill

		//Sorting of Encounters
		switch (uniqueEncounterID) {
			case 1:
				transform.SetSiblingIndex (0);
				break;
//			case 2:
//				transform.SetSiblingIndex (1);
//				break;
			case 3:
				transform.SetSiblingIndex (transform.parent.childCount + 1);
				break;
		}

//		Gizmos.color = GetZoneColor ();
//		var trueZ = (transform.position.z / 2f);
		if (parentZone) {
			if (Selection.activeGameObject == transform.gameObject) {
				return;
			}
			//Gizmos.color = new Color(1f,1f,1f,0.5f);
			//Gizmos.DrawCube (new Vector3 ((transform.position.x + (mapLength * LevelEditorManager.tileSize.x) / 2), parentZone.centerY, 0), new Vector3 (mapLength * LevelEditorManager.tileSize.x, LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y ,0));


			Gizmos.color = GetZoneColor ();
			//Gizmos.DrawGUITexture(new Rect(transform.position.x, -parentZone.gridSize.y, mapLength * LevelEditorManager.tileSize.x, parentZone.gridSize.y), (Texture)LevelEditorManager.gridTexture[mapLength-1]);
			Gizmos.DrawWireCube (new Vector3 ((transform.position.x + (mapLength * LevelEditorManager.tileSize.x) / 2), parentZone.centerY, 0), new Vector3 (mapLength * LevelEditorManager.tileSize.x, LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y ,0));

			float padding = 0;

			for (var i = 0; i < parentZone.encounters.Count; i++) {

				if (parentZone.encounters [i] == this) {
					transform.position = new Vector3 ((padding * LevelEditorManager.tileSize.x), 0, 0);
//					transform.position = new Vector3 (0, 0, ((padding * LevelEditorManager.tileSize.x) + ((mapLength * LevelEditorManager.tileSize.x) / 2)));
					break;
				} else {
					padding = padding + parentZone.encounters [i].mapLength;
				}
			}
		}

		//DRAW ZONE TILES

		var row = 0;
		var maxColumns = mapLength;
		var total = mapLength * LevelEditorManager.NUMBEROFLANES;

		var tile = new Vector3 (LevelEditorManager.tileSize.x, LevelEditorManager.tileSize.y, 0);


		var offset = new Vector3 ((tile.x / 2),(tile.y / 2), 0);

		//Debug.Log ("Max Columns: " + maxColumns + " | Total Grid Boxes: " + total + " | Offset.x: " + offset.x + " | Offset.y :" + offset.y);
		for (int z = 0; z < total; z++) {

			var column = z % maxColumns;

			var newX = transform.position.x + (column * tile.x) + offset.x;
			var newY = (-row * tile.y) - offset.y;
			//Gizmos.color = new Color (0.3f, 0.3f, 0.3f, 0.0f);
			//Gizmos.DrawCube (new Vector3 (newX, newY, transform.position.z), tile);
			Gizmos.color = new Color (0.3f, 0.3f, 0.3f, 1f);
			Gizmos.DrawWireCube (new Vector3 (newX, newY, transform.position.z), tile);


			if (column == maxColumns - 1) {
				row++;
			}
		}
	}

	virtual public void OnDrawGizmosSelected() {
		
		parentZone = transform.parent.GetComponent<ZoneMap> ();
		float padding = 0;

		if (parentZone) {
			for (var i = 0; i < parentZone.encounters.Count; i++) {
			
				if (parentZone.encounters [i] == this) {
					break;
				} else {
					padding = padding + parentZone.encounters [i].mapLength;
				}
			}
				
			//Encounter Area Fill
			Gizmos.color = GetZoneColor ();
//			Gizmos.DrawCube (new Vector3 (((parentZone.mapSize.y * LevelEditorManager.tileSize.x) / 2), 15f, transform.position.z + ((mapLength * LevelEditorManager.tileSize.x) / 2)), new Vector3 (parentZone.gridSize.y, 32f, mapLength * LevelEditorManager.tileSize.x));
			//Gizmos.DrawGUITexture(new Rect(transform.position.x, -parentZone.gridSize.y, mapLength * LevelEditorManager.tileSize.x, parentZone.gridSize.y), (Texture)LevelEditorManager.gridTexture[mapLength-1]);
			Gizmos.DrawCube (new Vector3 ((transform.position.x + (mapLength * LevelEditorManager.tileSize.x) / 2), parentZone.centerY, -15f), new Vector3 (mapLength * LevelEditorManager.tileSize.x, LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y ,30));
			//Encounter Area Outline
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube (new Vector3 ((transform.position.x + (mapLength * LevelEditorManager.tileSize.x) / 2), parentZone.centerY, -15f), new Vector3 (mapLength * LevelEditorManager.tileSize.x, LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y ,30));

		}
			
	}

	public Color GetZoneColor() {

		Color zoneColor;

		switch (encounterType) {
			case EncounterType.Pregame:
				zoneColor = new Color (0f, 0.8f, 0f, 0.7f);
				break;
			case EncounterType.Cutscene:
				zoneColor = new Color (0.75f, 0.54f, 0.15f, 0.7f);
				break;
			case EncounterType.Untagged:
				zoneColor = new Color (1f, 0f, 0f, 0.7f);
				break;
			case EncounterType.WarpGate:
				zoneColor = new Color (0f, 0.4f, 0.8f, 0.7f);
				break;
			case EncounterType.NullMovement:
				zoneColor = new Color (0.6f, 0.1f, 0f, 0.7f);
				break;
			default:
				zoneColor = new Color (0.4f, 0f, 0.4f, 0.7f);
				break;
		}

		return zoneColor;
	}
	#endif
}
