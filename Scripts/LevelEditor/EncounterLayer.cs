using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EncounterLayer : EncounterMap {

	public EncounterLayerType layerType;
	public string layerReferenceID;
	public float zIndex; // zIndex spacially separates the layers so you can see them at a different angle. Like a cake. Hmmm...cake.
	public int tileMapReferenceID = 0;
	public Vector2 gridSize;
	public Vector2 gridInUnits = new Vector2 ();
	public GameObject tiles;
	public EncounterMap parentEncounter;

	#if UNITY_EDITOR
	public Sprite currentTileBrush {
		get { return parentEncounter.parentZone.spriteReferences [tileMapReferenceID] as Sprite;}
	}

	override public void OnDrawGizmos() {
		if (!Selection.activeObject)
			return;

		parentEncounter = transform.parent.GetComponent<EncounterMap> ();


		if (Selection.activeGameObject != parentEncounter.transform.gameObject) {
			return;
		}
		transform.position = new Vector3 (transform.position.x, transform.position.y , FindZIndex());

		 
		//Ecounter Area Fill

//		zIndex = FindZIndex();
//
//		zIndex = transform.position.z + zIndex;

		Gizmos.color = GetLayerColor();

//		if (parentEncounter) {
//			if(parentEncounter.encounterType == EncounterType.Untagged)
//				Gizmos.color = new Color(1f,0f,0f,0.7f);
//			else
//				Gizmos.color = GetLayerColor();
			//Gizmos.DrawCube (new Vector3 ((transform.position.x + (parentEncounter.mapLength * LevelEditorManager.tileSize.x) / 2), ((LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y) / -2), transform.position.z), new Vector3 (parentEncounter.mapLength * LevelEditorManager.tileSize.x, LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y ,0));
//
//			if(parentEncounter.encounterType == EncounterType.Untagged)
//				Gizmos.color = new Color(1f,0f,0f,0.7f);
//			else
//				Gizmos.color = GetLayerColor();
//
//			Gizmos.color = new Color(Gizmos.color.r,Gizmos.color.g,Gizmos.color.b,1.0f);
//			Gizmos.DrawWireCube (new Vector3 (((parentEncounter.parentZone.mapSize.y * LevelEditorManager.tileSize.x) / 2), transform.position.y, transform.position.z), new Vector3 (parentEncounter.parentZone.gridSize.y, 1, mapLength * LevelEditorManager.tileSize.x));
//		}
	}

	override public void OnDrawGizmosSelected() {

		if (Selection.activeGameObject != transform.gameObject) {
			return;
		}

		float padding = 0;

		for (var i = 0; i < parentEncounter.parentZone.encounters.Count; i++) {

			if (parentEncounter.parentZone.encounters [i] == this.parentEncounter) {
				break;
			} else {
				padding = padding + parentEncounter.parentZone.encounters [i].mapLength;
			}
		}


		//Layer Box Fill
		if (parentEncounter.encounterType == EncounterType.Untagged) {
			Gizmos.color = new Color (1f, 0f, 0f, 0.7f);
			Gizmos.DrawCube (new Vector3 ((transform.position.x + (parentEncounter.mapLength * LevelEditorManager.tileSize.x) / 2), ((LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y) / -2), transform.position.z), new Vector3 (parentEncounter.mapLength * LevelEditorManager.tileSize.x, LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y ,0));

		}

		//Layer Box Border
		if(parentEncounter.encounterType == EncounterType.Untagged)
			Gizmos.color = new Color(1f,0f,0f,0.7f);
		else
			Gizmos.color = GetLayerColor ();

		Gizmos.color = new Color (Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 1f);
		//Gizmos.DrawWireCube (new Vector3 ((transform.position.x + (parentEncounter.mapLength * LevelEditorManager.tileSize.x) / 2), ((LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y) / -2), transform.position.z), new Vector3 (parentEncounter.mapLength * LevelEditorManager.tileSize.x, LevelEditorManager.NUMBEROFLANES * LevelEditorManager.tileSize.y ,0));

		//DRAW ZONE TILES
		Gizmos.color = Color.gray;
		var row = 0;
		var maxColumns = parentEncounter.mapLength;
		var total = parentEncounter.mapLength * LevelEditorManager.NUMBEROFLANES;

		var tile = new Vector3 (LevelEditorManager.tileSize.x, LevelEditorManager.tileSize.y, 0);


		var offset = new Vector3 ((tile.x / 2),(tile.y / 2), 0);

		//Debug.Log ("Max Columns: " + maxColumns + " | Total Grid Boxes: " + total + " | Offset.x: " + offset.x + " | Offset.y :" + offset.y);
		for (int z = 0; z < total; z++) {

			var column = z % maxColumns;

			var newX = parentEncounter.transform.position.x + (column * tile.x) + offset.x;
			var newY = (-row * tile.y) - offset.y;

			var id = (int)((column * LevelEditorManager.NUMBEROFLANES) + row);
			GameObject tileObject = GameObject.Find (parentEncounter.parentZone.name + "/" + parentEncounter.name + "/" + transform.name + "/" + "Tiles/tile_" + id);
//			//Debug.Log (tileObject.name);

			//Debug.Log ("Row: " + row + " | Column: " + column + " | Max Column: " + maxColumns);
			if (row > 3 && row < 9) {
				
				if (column == 0 && row == 6) {
						
					Gizmos.color = new Color (1f, 1f, 1f, 0.5f);

					if (tileObject == null) {	
						Gizmos.DrawCube (new Vector3 (newX, newY, transform.position.z), tile);
					}

					Gizmos.color = parentEncounter.GetZoneColor ();
					Gizmos.DrawWireCube (new Vector3 (newX, newY, transform.position.z), tile);

				} else if (column == (maxColumns - 1) && row == 6) {
						
					Gizmos.color = new Color (0.8f, 0.6f, 0.0f, 0.5f);
					if (tileObject == null) {	
						Gizmos.DrawCube (new Vector3 (newX, newY, transform.position.z), tile);
					}
						
					Gizmos.color = parentEncounter.GetZoneColor ();
					Gizmos.DrawWireCube (new Vector3 (newX, newY, transform.position.z), tile);

				} else {
					Gizmos.color = parentEncounter.GetZoneColor ();
					if (tileObject == null) {	
						Gizmos.DrawCube (new Vector3 (newX, newY, transform.position.z), tile);
					}
					Gizmos.DrawWireCube (new Vector3 (newX, newY, transform.position.z), tile);
				}

			} else {
				Gizmos.color = GetLayerColor ();
				if (tileObject == null) {		
					Gizmos.DrawCube (new Vector3 (newX, newY, transform.position.z), tile);
				}
				Gizmos.color = parentEncounter.GetZoneColor ();
				Gizmos.DrawWireCube (new Vector3 (newX, newY, transform.position.z), tile);
						
			}

			if (column == maxColumns - 1) {
				row++;
			}
		}

	}

	public Color GetLayerColor() {

		Color zoneColor;

		switch (layerType) {
		case EncounterLayerType.Background:
			zoneColor = new Color (1f, 0.85f, 0.8f, 0.3f);
			break;
		case EncounterLayerType.Management:
			zoneColor = new Color (1f, 0.91f, 0.26f, 0.3f);
			break;
		case EncounterLayerType.Pickup:
			zoneColor = new Color (0.35f, 1.0f, 0.83f, 0.3f);
			break;
		case EncounterLayerType.Nebula:
			zoneColor = new Color (0.94f, 0.45f, 1f, 0.3f);
			break;
		case EncounterLayerType.Obstacle:
			zoneColor = new Color (1f, 0.28f, 0.28f, 0.3f);
			break;
		case EncounterLayerType.Enemy:
			zoneColor = new Color (1f, 0f, 0f, 0.3f);
			break;
		default:
			zoneColor = new Color (1f, 0.85f, 0.8f, 0.3f);
			break;
		}

		return zoneColor;
	}

	public Texture2D GetLayerTexture() {

		Texture2D layerTexture;

		switch (layerType) {
		case EncounterLayerType.Background:
			layerReferenceID = "back";
			layerTexture = LevelEditorManager.texture2D[5];
			break;
		case EncounterLayerType.Management:
			layerReferenceID = "mana";	
			layerTexture = LevelEditorManager.texture2D[4];
			break;
		case EncounterLayerType.Pickup:
			layerReferenceID = "pick";	
			layerTexture = LevelEditorManager.texture2D[3];
			break;
		case EncounterLayerType.Nebula:
			layerReferenceID = "nebu";
			layerTexture = LevelEditorManager.texture2D [2];
			break;
		case EncounterLayerType.Obstacle:
			layerReferenceID = "obst";
			layerTexture = LevelEditorManager.texture2D[1];
			break;
		case EncounterLayerType.Enemy:
			layerReferenceID = "enem";
			layerTexture = LevelEditorManager.texture2D[0];
			break;
		default:
			layerTexture = LevelEditorManager.texture2D[0];
			layerReferenceID = "null";
			break;
		}

		return layerTexture;
	}

	public float FindZIndex() {

		switch (layerType) {
			case EncounterLayerType.Background:
				return -5f;
			case EncounterLayerType.Management:
				return -10f;
			case EncounterLayerType.Pickup:
				return -15f;
			case EncounterLayerType.Nebula:
				return -20f;
			case EncounterLayerType.Obstacle:
				return -25f;
			case EncounterLayerType.Enemy:
				return -30f;
			default:
				//Debug.LogWarning ("FindYIndex(); LayerType not set for: " + gameObject.name );
				return 0f;
		}
	}
	#endif
}
