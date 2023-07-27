using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.ChallengeObject;
#if UNITY_EDITOR
using ChromaShift.Scripts.SaveGame;
using UnityEditor;
#endif

public static class LevelEditorManager {

	public static Vector2 tileSize = new Vector2 (8,8);
	public static Vector2 tilePadding = new Vector2 (2,2);
	public static float pixelToUnits = 0;
	public static Texture2D[] texture2D = new Texture2D[6];
	public static Vector2 textureTileSize = new Vector2 ();
	public const int NUMBEROFLANES = 13;
	private static Texture[] gridTexture = new Texture[5];
	private static Material gridMat;

	public static List<GameObject> enemyList = new List<GameObject>();
	public static List<GameObject> nebulaList = new List<GameObject>();
	public static List<GameObject> obstacleList = new List<GameObject>();
	public static List<GameObject> pickupList = new List<GameObject>();
	public static List<GameObject> managementList = new List<GameObject>();
	public static List<GameObject> backgroundList = new List<GameObject>();

	public static List<ZoneMapData> zoneMapList = new List<ZoneMapData>();
    public static List<MovementStack> movementStackList = new List<MovementStack>();
    public static List<GameObject> encounterMapList = new List<GameObject>();

	#if UNITY_EDITOR
	
	static LevelEditorManager() {
		texture2D[0] = (Texture2D) Resources.Load ("LP_Enemy");
		texture2D[1] = (Texture2D) Resources.Load ("LP_Obstacle");
		texture2D[2] = (Texture2D) Resources.Load ("LP_Nebula");
		texture2D[3] = (Texture2D) Resources.Load ("LP_Pickup");
		texture2D[4] = (Texture2D) Resources.Load ("LP_Management");
		texture2D[5] = (Texture2D) Resources.Load ("LP_Background");

		gridTexture[0]  = (Texture)   Resources.Load ("LP_GridTexture_1");
		gridTexture[1]  = (Texture)   Resources.Load ("LP_GridTexture_2");
		gridTexture[2]  = (Texture)   Resources.Load ("LP_GridTexture_3");
		gridTexture[3]  = (Texture)   Resources.Load ("LP_GridTexture_4");
		gridTexture[4]  = (Texture)   Resources.Load ("LP_GridTexture_5");
//		gridTexture[5]  = (Texture2D)   Resources.Load ("LP_GridTexture_6");
//		gridTexture[6]  = (Texture2D)   Resources.Load ("LP_GridTexture_7");
//		gridTexture[7]  = (Texture2D)   Resources.Load ("LP_GridTexture_8");
//		gridTexture[8]  = (Texture2D)   Resources.Load ("LP_GridTexture_9");
//		gridTexture[9]  = (Texture2D)   Resources.Load ("LP_GridTexture_10");

		gridMat  = (Material)   Resources.Load ("LP_GridMat");

		enemyList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/Enemy"));
		pickupList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/Pickup"));
		nebulaList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/Nebula"));
		obstacleList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/Obstacle"));
		managementList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/Management"));
		backgroundList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/Background"));
		//gridTexture[4]  = (Texture)   Resources.Load ("LP_GridTexture_5");

		zoneMapList = new List<ZoneMapData>(Resources.LoadAll<ZoneMapData>("LevelEditorObjects/ZoneMaps"));
		encounterMapList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/EncounterMaps"));
	}

    [MenuItem("Tools/ChromaShift/Create/Level Data Object")]
    public static void CreateLevelDataObject()
    {
        LevelData asset = ScriptableObject.CreateInstance<LevelData>();

        AssetDatabase.CreateAsset(asset, "Assets/ChromaShift/Scenes/Sectors/DataObjects/LDO - .asset");
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/ChromaShift/Create/Scene Data Object")]
    public static void CreateSceneDataObject()
    {
	    SceneData asset = ScriptableObject.CreateInstance<SceneData>();

	    AssetDatabase.CreateAsset(asset, "Assets/ChromaShift/Scenes/Sectors/DataObjects/SDO - .asset");
	    AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/ChromaShift/Create/Challenge Object")]
    public static void CreateChallengeObject()
    {
	    bool option = EditorUtility.DisplayDialog("New Challenge Object","Would you like to create a new Challenge Object?\nNote that after creating this object you will need to select it and give it a name.","OK","Cancel");
	    if (!option)
		    return;
	    ChallengeObject asset =  ScriptableObject.CreateInstance<ChallengeObject>();
	    
		
	    AssetDatabase.CreateAsset(asset, "Assets/ChromaShift/Resources/ChallengeObjects/Challenge_.asset");
	    AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tools/ChromaShift/Create/Save Data Object")]
    public static void CreateSaveDataObject()
    {
	    
	    SaveGameData asset =  ScriptableObject.CreateInstance<SaveGameData>();
	    
		
	    AssetDatabase.CreateAsset(asset, "Assets/ChromaShift/Resources/SaveData/00_SaveDataObject.asset");
	    AssetDatabase.SaveAssets();
    }
    

    /*[MenuItem("ChromaShift/Create/Zone")]
	public static void CreateSectorMap() {
		GameObject go = new GameObject ("Zone Map");
		go.AddComponent<ZoneMap> ();
		ZoneMap zM = go.GetComponent<ZoneMap> ();
	}

	[MenuItem("ChromaShift/Create/Encounter")]
	public static void CreateEncounter() {

		encounterMapList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/EncounterMaps"));

		int encounterCount = encounterMapList.Count;
		GameObject go;

		if (encounterCount < 10) {
			go = new GameObject ("0"+encounterCount+"_Encounter_");
		} else {
			go = new GameObject (encounterCount+"_Encounter_");
		}

		go.AddComponent<EncounterMap> ();
		go.transform.SetParent (GameObject.Find ("Zone Map").transform);

		EncounterMap encMap = go.GetComponent<EncounterMap>();

		encMap.uniqueEncounterID = encounterCount;

		//EnemyLayer
		GameObject el = new GameObject ("Enemy Layer");
		el.transform.parent = go.transform;
		el.AddComponent<EncounterLayer> ().parentEncounter = encMap;
		el.GetComponent<EncounterLayer> ().layerType = EncounterLayerType.Enemy;

		//ObstacleLayer
		GameObject ol = new GameObject ("Obstacle Layer");
		ol.transform.parent = go.transform;
		ol.AddComponent<EncounterLayer> ().parentEncounter = encMap;
		ol.GetComponent<EncounterLayer> ().layerType = EncounterLayerType.Obstacle;

		//NebulaLayer
		GameObject nl = new GameObject ("Nebula Layer");
		nl.transform.parent = go.transform;
		nl.AddComponent<EncounterLayer> ().parentEncounter = encMap;
		nl.GetComponent<EncounterLayer> ().layerType = EncounterLayerType.Nebula;

		//PickupLayer
		GameObject pl = new GameObject ("Pickup Layer");
		pl.transform.parent = go.transform;
		pl.AddComponent<EncounterLayer> ().parentEncounter = encMap;
		pl.GetComponent<EncounterLayer> ().layerType = EncounterLayerType.Pickup;

		//ManagementLayer
		GameObject ml = new GameObject ("Management Layer");
		ml.transform.parent = go.transform;
		ml.AddComponent<EncounterLayer> ().parentEncounter = encMap;
		ml.GetComponent<EncounterLayer> ().layerType = EncounterLayerType.Management;

		//BackgroundLayer
		GameObject bl = new GameObject ("Background Layer");
		bl.transform.parent = go.transform;
		bl.AddComponent<EncounterLayer> ().parentEncounter = encMap;
        bl.GetComponent<EncounterLayer>().layerType = EncounterLayerType.Background;

		GameObject newPrefab = PrefabUtility.CreatePrefab ("Assets/ChromaShift/Resources/LevelEditorObjects/EncounterMaps/" + go.name + ".prefab", go);
	}*/


    [MenuItem("Tools/ChromaShift/Create/Movement Stack")]
    public static void CreateNewMovementStack()
    {
        string assetName = GetAssetName();

        //GameObject go;

        //go = new GameObject(assetName);

        //go.AddComponent<MovementStack>();
        //go.GetComponent<MovementStack>().ID = assetName;

        MovementStack asset = ScriptableObject.CreateInstance<MovementStack>();

        
        AssetDatabase.CreateAsset(asset, "Assets/ChromaShift/Resources/MovementStacks/MS-"+assetName+".asset");
        asset.ID = assetName;
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        //GameObject newPrefab = PrefabUtility.CreatePrefab("Assets/ChromaShift/Resources/MovementStacks/MS-" + assetName + ".prefab", go);

        Selection.activeObject = asset;

        //Selection.activeObject = newPrefab;

        //Object.DestroyImmediate(go);
    }

    public static string GetAssetName()
    {
        movementStackList = new List<MovementStack>(Resources.LoadAll<MovementStack>("MovementStacks"));
        int count = movementStackList.Count + 1;

        string assetName = "";
        if (count < 10)
        {
            assetName = "0000" + count;
        }
        else if (count < 100)
        {
            assetName = "000" + count;
        }
        else if (count < 1000)
        {
            assetName = "00" + count;
        }
        else if (count < 10000)
        {
            assetName = "0" + count;
        }
        else
        {
            assetName = "0" + count;
        }

        return assetName;
    }
	
	#endif
}
