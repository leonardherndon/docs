using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EncounterLayer))]
public class EncounterMapEditor : Editor {

	public EncounterMap encounter;
	public EncounterLayer encounterLayer;
	TileBrush brush;
	Vector3 mouseHitPos;
	int tileSize = 8;
	int delayNumber = 0;


	public override void OnInspectorGUI() {

		EditorGUILayout.BeginVertical ();
		encounter.uniqueEncounterID = EditorGUILayout.IntField ("Unique Encounter ID:", encounter.uniqueEncounterID);
		encounter.encounterOrderID = EditorGUILayout.IntField ("Encounter Order ID:", encounter.encounterOrderID);
		EditorGUILayout.LabelField ("Layer Reference ID: " + encounterLayer.layerReferenceID);
		EditorGUILayout.LabelField ("Encounter Type: " + encounter.encounterType);
		EditorGUILayout.ObjectField("Parent Encounter", encounterLayer.parentEncounter.transform, typeof (Transform), false);
		EditorGUILayout.ObjectField("Parent Zone", encounterLayer.parentEncounter.parentZone.transform, typeof (Transform), false);
		EditorGUILayout.LabelField ("Layer Type: " + encounterLayer.layerType);
		EditorGUILayout.ObjectField("Image", encounterLayer.currentTileBrush, typeof (Texture2D), false);
		UpdateBrush (encounterLayer.currentTileBrush);

//		//Debug.Log ("Is This Running??");
		UpdateCalculations ();
		encounter.parentZone.tileID = 1;
		CreateBrush ();

		EditorGUILayout.Vector2Field ("Grid Broundy:", encounter.gridBoundry);

		EditorGUILayout.LabelField ("Pixels To Units:", LevelEditorManager.pixelToUnits.ToString ());
		EditorGUILayout.Vector2Field ("Tile Size:", LevelEditorManager.tileSize);
//		EditorGUILayout.LabelField ("Tile Size:", LevelEditorManager.textureTileSize.x + "x" + LevelEditorManager.textureTileSize.y);
		LevelEditorManager.tilePadding = EditorGUILayout.Vector2Field ("Tile Padding", LevelEditorManager.tilePadding);
		UpdateBrush (encounterLayer.currentTileBrush);

		if (GUILayout.Button ("Clear Tiles")) {
			if(EditorUtility.DisplayDialog("Clear zone's tiles?", "Are you sure?", "Clear", "Decline")) {
				ClearZone();
			}
		}


		//Encounter List
		var list = encounterLayer.parentEncounter.parentZone.encounters;
		int newCount = Mathf.Max (0, EditorGUILayout.IntField ("Encounters", list.Count));
		while (newCount < list.Count)
			list.RemoveAt (list.Count - 1);
		while (newCount > list.Count)
			list.Add (null);

		for (int i = 0; i < list.Count; i++) {
			list [i] = (EncounterMap)EditorGUILayout.ObjectField (list [i], typeof(EncounterMap));
		}
		if (list.Count > 0) {
			if (list [0]) {
				if (list [0].encounterType != EncounterType.Pregame) {
					EditorGUILayout.HelpBox ("No Pregame Encounter Set", MessageType.Warning);
				}
			}
		} else {
				EditorGUILayout.HelpBox ("No Encounter attached to this Zone", MessageType.Warning);
		}
		EditorGUILayout.EndVertical ();
	}

	void OnEnable() {
//		zone = target as ZoneMap;
		encounterLayer = target as EncounterLayer;
		encounter = encounterLayer.parentEncounter;
		Tools.current = Tool.View;

//		zone.gridSize = new Vector2 (LevelEditorManager.tileSize.x * zone.mapSiztileSizeeSize.y * zone.mapSize.y);
		encounterLayer.gridSize = new Vector2 (LevelEditorManager.tileSize.x * encounter.parentZone.mapSize.x, LevelEditorManager.tileSize.y * encounter.parentZone.mapSize.y);

		if(encounterLayer.tiles == null) {
			var go = new GameObject ("Tiles");
			go.transform.SetParent (encounterLayer.transform);
			go.transform.position = Vector3.zero;

			encounterLayer.tiles = go;
		}
			
		UpdateCalculations ();
		NewBrush ();

	}

	void OnDisable() {
		DestroyBrush ();
	}

	void OnSceneGUI(){


		if (brush != null) {
			if (delayNumber < 100) {
				delayNumber++;
//				return;
			} else {
				delayNumber = 0;
				UpdateHitPosition ();
				MoveBrush ();
			}

			if (LevelEditorManager.texture2D != null) {
				Event current = Event.current;
				if (current.shift) {
					DrawTile ();
				}
				if (current.alt) {
					RemoveTile ();
				}
			}
		}	
	}

	void UpdateCalculations() {
		var path = AssetDatabase.GetAssetPath(encounterLayer.GetLayerTexture());
		encounter.parentZone.spriteReferences = AssetDatabase.LoadAllAssetsAtPath(path);

		var sprite = (Sprite) encounter.parentZone.spriteReferences[1];

		var width = sprite.textureRect.width;
		var height = sprite.textureRect.height;

		var w = encounterLayer.GetLayerTexture().width;
		var h = encounterLayer.GetLayerTexture().height;

		LevelEditorManager.pixelToUnits = (float)(sprite.rect.width / sprite.bounds.size.x);
//
		LevelEditorManager.textureTileSize = new Vector2 (width, height);
	}

	void CreateBrush() {
		if (brush != null) {
			return;
		}
		var sprite = encounterLayer.currentTileBrush;
		if (sprite != null) {
			GameObject go = new GameObject ("Brush");

//			go.transform.Rotate(new Vector3 (0, -90, 0));

			go.transform.SetParent (encounterLayer.transform);
			go.transform.localScale = new Vector3 (0.125f, 0.125f, 0.125f);

			//Debug.Log ("Assigning Brush to Var");
			brush = go.AddComponent<TileBrush> ();
			brush.renderer2D = go.AddComponent<SpriteRenderer> ();
			brush.renderer2D.sortingOrder = 00;
			//Debug.Log ("Confirming Brush Name: " + brush.transform.name);

			var pixelsToUnits = LevelEditorManager.pixelToUnits;

			brush.brushSize = new Vector3 (8, 8, 0);

			brush.UpdateBrush (sprite);

		} else {
			//Debug.Log ("Create Brush: Sprite is Null");
		}
	}

	void NewBrush() {
		if (brush == null) {
			//Debug.Log ("New Brush: No Brush. Let's Create Brush");
			CreateBrush ();
		}
	}

	void DestroyBrush(){
		if (brush != null)
			DestroyImmediate (brush.gameObject);
	}

	public void UpdateBrush(Sprite sprite) {
		if (brush != null) {
//			//Debug.Log ("Update Brush");
			brush.UpdateBrush (sprite);
		}
	}

	void UpdateHitPosition() {

		var p = new Plane (encounterLayer.transform.TransformDirection(Vector3.back), Vector3.zero);
		var ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		var hit = Vector3.zero;
		var dist = 0f;

		if (p.Raycast (ray, out dist))
			hit = ray.origin + ray.direction.normalized * dist;

		mouseHitPos = encounter.transform.TransformPoint (hit);
		//Debug.Log ("Mouse Hit Pos X [Before]: " + mouseHitPos.x);
		//I don't know why I have to multiply the xPosition by 2 but it works....let's look at this later
		mouseHitPos.x = mouseHitPos.x - encounter.transform.position.x * 2;
		//Debug.Log ("Mouse Hit Pos X [After]: " + mouseHitPos.x);

		//Debug.Log ("Mouse Hit Pos: " + mouseHitPos);
	}

	void MoveBrush(){

		var x = Mathf.Floor (mouseHitPos.x / LevelEditorManager.tileSize.x) * LevelEditorManager.tileSize.x;

		var y = Mathf.CeilToInt (mouseHitPos.y / LevelEditorManager.tileSize.y) * LevelEditorManager.tileSize.y;

		var row = Mathf.Abs(y / LevelEditorManager.tileSize.x);
		var rowBound = y / LevelEditorManager.tileSize.x;
		var column = x / LevelEditorManager.tileSize.y;

		//Debug.Log ("Row: " + row + " | RowBound: " + rowBound + " | Column: " + column);
		encounter.gridBoundry = new Vector2 (encounter.mapLength - 1, encounterLayer.gridSize.y / LevelEditorManager.tileSize.y - 1);

		var id = (int)((column * LevelEditorManager.NUMBEROFLANES) + row);


		//CURSOR IS OUTSIDE OF GRID BOUNDRY
		if (rowBound > 0 || column < 0 || row > encounter.gridBoundry.y || column > encounter.gridBoundry.x ) {
			row = -1;
			column = -1;
			id = -1;
		}

		//Debug.Log("Grid Boundry | x: " + encounter.gridBoundry.x + " z: " + encounter.gridBoundry.y);

		brush.tileID = id;

		//Debug.Log ("Tile ID: " + brush.tileID);
//		//Debug.Log ("Row: " + row + " | Column: " + column);
		//Debug.Log ("x: " + x + " | y: " + y);

		if (x < 0) {
		//	//Debug.Log("OUTSIDE of Grid Boundry X");
			x = 0;
		}
		if (x > encounter.gridBoundry.x * LevelEditorManager.tileSize.x) {
		//	//Debug.Log("OUTSIDE of Grid Boundry X");
			x = encounter.gridBoundry.x * LevelEditorManager.tileSize.x;
		}

		if (y > 0) {
		//	//Debug.Log("OUTSIDE of Grid Boundry Y");	
			y = 0;
		}
		if (y < (encounter.gridBoundry.y * LevelEditorManager.tileSize.y) * -1) {
		//	//Debug.Log("OUTSIDE of Grid Boundry Y");	
			y = (encounter.gridBoundry.y * LevelEditorManager.tileSize.y) * -1;
		}

		x += encounterLayer.transform.position.x;
		y += encounterLayer.transform.position.y;

		x = x * encounterLayer.transform.localScale.x + (LevelEditorManager.tileSize.x /2);
		y = (y * encounterLayer.transform.localScale.y + (LevelEditorManager.tileSize.y /2) * -1);

		//Debug.Log ("Brush Position [x: " + x + ", y: " + y + "]");
		//Debug.Log ("-------------------------------------------");
		brush.transform.position = new Vector3 (x, y, encounterLayer.transform.position.z);
	}

	void DrawTile() {
		if (delayNumber < 10) {
			delayNumber++;
			return;
		} else {
			delayNumber = 0;
		}
		var id = "";
		var posX = brush.transform.position.x;
		var posY = brush.transform.position.y;

		id = brush.tileID.ToString ();

		//Debug.Log ("Before Placing Tile | ID: " + id + " x: " + posX + " y: " + posY);

		if (id != null && id != "-1") {

			string tileName = encounterLayer.layerType.ToString() + "_" + id;
			GameObject tile = GameObject.Find (encounterLayer.parentEncounter.parentZone.name + "/" + encounterLayer.parentEncounter.name + "/" + encounterLayer.name + "/" + "Tiles/" + tileName);
			int assignedLane = (int.Parse(id)) % 13;
			//Debug.Log ("TileMapReferenceID: " + encounterLayer.tileMapReferenceID);
			if (tile == null) {
				EncounterLayerType currentLayerType = encounterLayer.layerType;

				switch(currentLayerType) {
					case EncounterLayerType.Enemy:
						tile = (GameObject)Instantiate (LevelEditorManager.enemyList [encounterLayer.tileMapReferenceID-1], encounterLayer.tiles.transform);                                                                                                                                                                                                 
						break;
					case EncounterLayerType.Obstacle:
						tile = (GameObject)Instantiate (LevelEditorManager.obstacleList [encounterLayer.tileMapReferenceID-1], encounterLayer.tiles.transform);                                                                                                                                                                                                
						break;
					case EncounterLayerType.Nebula:
						tile = (GameObject)Instantiate (LevelEditorManager.nebulaList [encounterLayer.tileMapReferenceID-1], encounterLayer.tiles.transform);                                                                                                                                                                                                 
						break;
					case EncounterLayerType.Pickup:
						tile = (GameObject)Instantiate (LevelEditorManager.pickupList [encounterLayer.tileMapReferenceID-1], encounterLayer.tiles.transform);                                                                                                                                                                                                  
						break;
					case EncounterLayerType.Management:
						tile = (GameObject)Instantiate (LevelEditorManager.managementList [encounterLayer.tileMapReferenceID-1], encounterLayer.tiles.transform);                                                                                                                                                                                                  
						break;
					case EncounterLayerType.Background:
						tile = (GameObject)Instantiate (LevelEditorManager.backgroundList [encounterLayer.tileMapReferenceID-1], encounterLayer.tiles.transform);                                                                                                                                                                                                 
						break;
				}
				if (tile.gameObject.GetComponent<ObjectSpawner> ().sourcedObject.gameObject.name != null)
					tile.name = tile.gameObject.GetComponent<ObjectSpawner> ().sourcedObject.gameObject.name + "-" + tileName;
				else
					tile.name = tileName;

//				tile = new GameObject (encounterLayer.layerType + "_" + id);
				//tile.transform.Rotate (new Vector3 (90, -90, 0));
				tile.transform.SetParent (encounterLayer.tiles.transform);
//				tile.transform.localScale = new Vector3 (0.125f, 0.125f, 0.125f);
				tile.transform.position = new Vector3 (posX, posY, brush.transform.position.z);
				//Debug.Log ("Placed Tile | x: " + posX + " z: " + posZ);
//				tile.AddComponent<SpriteRenderer> ();


				//Initializing TileObject when tile is created
				if (encounterLayer.layerType != EncounterLayerType.Management) {
					var objectSpawner = tile.GetComponent<ObjectSpawner>();
					objectSpawner.startLane = assignedLane;
					objectSpawner.uniqueObjectReferenceID = ("[Map: " + encounter.parentZone.mapID + "]  |  [Uencounter: " + encounter.uniqueEncounterID + "]  |  [" + encounterLayer.layerType + ": "+ encounterLayer.layerReferenceID + "]  |  [Tile: " + id + "]");
				}
			
				//Debug.Log ("Tile ID: " + id + "\n Assigned Lane: " + assignedLane);

			} else {
				tile.transform.position = new Vector3 (posX, posY, brush.transform.position.z);
				//Debug.Log ("Moving Tile | x: " + posX + " z: " + posZ);
			}

			//SET TILE TO SPRITE IMAGE
//			tile.GetComponent<SpriteRenderer> ().sprite = brush.renderer2D.sprite;
		
		} else {
			//Debug.Log("NO TILE CREATED: Brush is outside bounds of the play area.");
		}
	}

	void RemoveTile() {

		if (delayNumber < 10) {
			delayNumber++;
			return;
		} else {
			delayNumber = 0;
		}

		//Debug.Log ("Removing Tiles");
		var id = brush.tileID.ToString ();

		GameObject tile = GameObject.Find (encounterLayer.parentEncounter.parentZone.name + "/" + encounterLayer.parentEncounter.name + "/" + encounterLayer.name + "/" + "Tiles/" + encounterLayer.layerType + "_" + id);
		//Debug.Log ("Removing Tile ID: " + id);
		if (tile != null) {
			DestroyImmediate (tile);
		}
	}

	void ClearZone(){
//	Zone (var i = 0; i < zone.tiles.transform.childCount; i++) {
//			Transform t = zone.tiles.transform.GetChild (i);
//			DestroyImmediate (t.gameObject);
//			i--;
//		}
	}
}
