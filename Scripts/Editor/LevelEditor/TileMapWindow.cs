using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class TileMapWindow : EditorWindow {

	public string[] spriteSheetScale = new string[] {"100%","90%","80%","70%","60%","50%"};

	int scaleLabel;
	EncounterLayer selection;
	public Vector2 scrollPosition = Vector2.zero;
	Vector2 currentSelection  = Vector2.zero;

	[MenuItem("Tools/ChromaShift/Window/Tile Map")]
	public static void OpenZoneMapWindow(){
		var window = EditorWindow.GetWindow (typeof(TileMapWindow));
		var title = new GUIContent ();
		title.text = "Tile Map Widow";
		window.titleContent = title;
	}
		

	void OnGUI() {

		//if (Selection.activeGameObject == null)
		//	return;

		//Check if Encounter Layer is Selected
		try
		{
			selection = Selection.activeGameObject.GetComponent<EncounterLayer> ();
		}
		catch(Exception e) {
			GUILayout.Box ("Please Select a Encounter Layer");
			return;
		}
	

		if (selection != null) {
			
			var texture2D = selection.GetLayerTexture ();
//			float newScale = 0f;
			if (texture2D != null) {
//				scale = (Scale)EditorGUILayout.EnumPopup ("Zoom", scale);
//				scaleLabel = EditorGUILayout.Popup(scaleLabel,spriteSheetScale);
//				//Debug.Log ("ScaleLabel: " + scaleLabel);
//				switch (scaleLabel) {
//					case 0:
//						newScale = 1f;
//						break;
//					case 1:
//						newScale = 0.9f;
//						break;
//					case 2:
//						newScale = 0.8f;
//						break;
//					case 3:
//						newScale = 0.7f;
//						break;
//					case 4:
//						newScale = 0.6f;
//						break;
//					case 5:
//						newScale = 0.5f;
//						break;
//				}
					
				var newTextureSize = new Vector2 (texture2D.width, texture2D.height); // * newScale;

				var offset = new Vector2 (10, 10);

				var veiwPort = new Rect (0, 0, position.width - 5, position.height - 5);
				var contentSize = new Rect (0, 0, newTextureSize.x + offset.x, newTextureSize.y + offset.y);
				scrollPosition = GUI.BeginScrollView (veiwPort, scrollPosition, contentSize);

				GUI.DrawTexture (new Rect (offset.x, offset.y, newTextureSize.x, newTextureSize.y), texture2D);

				var tile = LevelEditorManager.textureTileSize; // * newScale;

				tile.x += LevelEditorManager.tilePadding.x; // * newScale;
				tile.y += LevelEditorManager.tilePadding.y; // * newScale;

				var grid = new Vector2 (newTextureSize.x / tile.x, newTextureSize.y / tile.y);

				var selectionPos = new Vector2 (tile.x * currentSelection.x + offset.x,
					                   tile.y * currentSelection.y + offset.y);

				//Debug.Log ("SelectionPos: " + selectionPos.x + "x" + selectionPos.y);
				//Debug.Log ("Tile: " + tile.x + "x" + tile.y);
				//Debug.Log ("ScrollPosition: " + scrollPosition.x + "x" + scrollPosition.y);

				var boxTex = new Texture2D (1, 1);
				boxTex.SetPixel (0, 0, new Color (0, 0.5f, 1f, 0.2f));
				boxTex.Apply ();


				var style = new GUIStyle (GUI.skin.customStyles [0]);
				style.normal.background = boxTex;

				GUI.Box (new Rect (selectionPos.x, selectionPos.y, tile.x, tile.y), "", style);

				var cEvent = Event.current;
				Vector2 mousePos = new Vector2 (cEvent.mousePosition.x, cEvent.mousePosition.y);
				if (cEvent.type == EventType.MouseDown && cEvent.button == 0) {
					currentSelection.x = Mathf.Floor ((mousePos.x - offset.x) / tile.x);
					currentSelection.y = Mathf.Floor ((mousePos.y - offset.y) / tile.y);
					if (currentSelection.x > grid.x - 1) {
						currentSelection.x = grid.x - 1;
					}
					if (currentSelection.y > grid.y - 1) {
						currentSelection.y = grid.y - 1;
					}

					//Debug.Log ("CurrentSelection Position: " + currentSelection.x + "x" + currentSelection.y);

					selection.tileMapReferenceID = (int)(currentSelection.x + (currentSelection.y * grid.x) + 1);
					//Debug.Log ("Current Selection ID: " + selection.tileMapReferenceID);
					Repaint ();
				}

				GUI.EndScrollView ();
			}
		} else {
			GUILayout.Box ("Please Select a Encounter Layer");
		}

		//Debug.Log ("Zone Map Selected");
		
	}
}
