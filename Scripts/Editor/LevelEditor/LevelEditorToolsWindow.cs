using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using System;

public class LevelEditorToolsWindow : EditorWindow
{
    Texture2D headerSectionTexture;
    Texture2D zoneSectionTexture;
    LevelEditorToolsWindow window;
    EncounterMap encounterSelection;
    Rect headerSectionRect;
    Rect zoneSectionRect;
    int zoneToLoad = 0;
    int encounterToLoad = 0;
    private float m_LastEditorUpdateTime;
    GameObject testEncounterCache;
    readonly Color headerSectionColor = new Color(44f / 255f, 44f / 255f, 44f / 255f, 1f);
    readonly Color zoneSectionColor = new Color(104f / 255f, 88f / 255f, 153f / 255f, 1f);
    readonly Color encounterSectionColor = new Color(62f / 255f, 15f / 255f, 204f / 255f, 1f);

    [MenuItem("Tools/ChromaShift/Window/Level Editor Tools")]
    static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        LevelEditorToolsWindow window =
            (LevelEditorToolsWindow) EditorWindow.GetWindow(typeof(LevelEditorToolsWindow), false, "Level Dev Tools");
//		window.title = "Level Dev Tools";
        Debug.Log("Let\'s Open the Window.");
        //window.Show();
    }

    void OnEnable()
    {
        InitTextures();
        EditorApplication.playmodeStateChanged += OnPlaymodeChange;
    }

    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        zoneSectionTexture = new Texture2D(1, 1);
        zoneSectionTexture.SetPixel(0, 0, zoneSectionColor);
        zoneSectionTexture.Apply();
    }

    public void OnGUI()
    {
        DrawLayouts();
        DrawZoneArea();
    }

    void DrawLayouts()
    {
        headerSectionRect.x = 0;
        headerSectionRect.y = 0;
        headerSectionRect.width = Screen.width;
        headerSectionRect.height = 30f;

        zoneSectionRect.x = 0;
        zoneSectionRect.y = 0;
        zoneSectionRect.width = EditorGUIUtility.currentViewWidth;
        zoneSectionRect.height = 40;

        GUI.DrawTexture(headerSectionRect, headerSectionTexture);
        GUI.DrawTexture(zoneSectionRect, zoneSectionTexture);
    }

    void DrawZoneArea()
    {
        GUILayout.BeginArea(zoneSectionRect);
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("ZONE", GUILayout.Width(EditorGUIUtility.currentViewWidth / 8));
        if (GUILayout.Button("Test Zone", GUILayout.Height(30),
            GUILayout.MinWidth((EditorGUIUtility.currentViewWidth / 8)), GUILayout.ExpandWidth(false)))
        {
            LoadTestRealmZone();
        }

        if (GUILayout.Button("Save Zone", GUILayout.Height(30),
            GUILayout.MinWidth((EditorGUIUtility.currentViewWidth / 8)), GUILayout.ExpandWidth(false)))
        {
            SaveZoneData();
        }

        zoneToLoad = EditorGUILayout.IntField(zoneToLoad, GUILayout.Height(30),
            GUILayout.Width((EditorGUIUtility.currentViewWidth / 8) / 4));
        if (GUILayout.Button("Load Zone", GUILayout.Height(30),
            GUILayout.MinWidth((EditorGUIUtility.currentViewWidth / 8) / 4 * 3), GUILayout.ExpandWidth(false)))
        {
            LoadZoneData(zoneToLoad);
        }

        if (GUILayout.Button("New Zone", GUILayout.Height(30),
            GUILayout.MinWidth((EditorGUIUtility.currentViewWidth / 8)), GUILayout.ExpandWidth(false)))
        {
            CreateZoneMapData();
        }

        if (GUILayout.Button("Remove Zone", GUILayout.Height(30),
            GUILayout.MinWidth((EditorGUIUtility.currentViewWidth / 8)), GUILayout.ExpandWidth(false)))
        {
//            CreateZoneMapData();
            ClearZoneElements(GameObject.Find("YuME_MapData"));
        }

        if (GUILayout.Button("Import Zone", GUILayout.Height(30),
            GUILayout.MinWidth((EditorGUIUtility.currentViewWidth / 8)), GUILayout.ExpandWidth(false)))
        {
            LoadZoneData(zoneToLoad);
        }

        GUILayout.Space((EditorGUIUtility.currentViewWidth / 7));

        GUILayout.EndHorizontal();
        
        GUILayout.EndArea();
        
        GUILayout.BeginArea(new Rect(0, zoneSectionRect.height, Screen.width, Screen.height - zoneSectionRect.height));
        
        AddYumeButtons();
        
        GUILayout.EndArea();
    }

    private void AddYumeButtons()
    {
        GUILayout.BeginVertical();
        
        if (GUILayout.Button("Clear tiles"))
        {
            ClearTiles(GameObject.Find("YuME_MapData"));
        }

        if (GUILayout.Button("Save zone layout"))
        {
            // Save data
            SaveYumeMapData();
        }
        
        GUILayout.EndVertical();
    }

    void OnPlaymodeChange()
    {
        if (!EditorApplication.isPlaying && GameManager.Instance.isTesting)
        {
            GameManager.Instance.isTesting = false;
            EditorSceneManager.OpenScene("Assets/ChromaShift/Scenes/LevelEditor.unity", OpenSceneMode.Additive);
            //if(encounterSelection != null)
            //	Selection.activeGameObject = encounterSelection.gameObject;
        }
    }

    void LoadTestRealmZone()
    {
        SaveZoneData(true); //separate and rename
        EditorSceneManager.SaveScene(SceneManager.GetSceneByName("LevelEditor"));
        EditorSceneManager.CloseScene(SceneManager.GetSceneByName("LevelEditor"), false);
        EditorApplication.isPlaying = true;
        GameManager.Instance.isTesting = true;
    }

    void LoadTestRealmEncounter()
    {
        encounterSelection = Selection.activeGameObject.GetComponent<EncounterMap>();

        if (encounterSelection == null)
        {
            EditorUtility.DisplayDialog("No Test Encounter Selected",
                "Please select a Encounter before trying to test.", "OK");
            //SaveEncounter(true);
//			LoadEncounterIntoTestRealm (19);
        }
        else
        {
            SaveEncounter(true);
            LoadEncounterIntoTestRealm(encounterSelection.uniqueEncounterID);
        }

//		SaveEncounter(true);
//		LoadEncounterIntoTestRealm (encounterSelection.uniqueEncounterID);
        EditorSceneManager.SaveScene(SceneManager.GetSceneByName("LevelEditor"));
        EditorSceneManager.CloseScene(SceneManager.GetSceneByName("LevelEditor"), false);
        EditorApplication.isPlaying = true;
        GameManager.Instance.isTesting = true;
    }

    void LoadTestFlight()
    {
        LoadEncounterIntoTestRealm();
        //		SaveEncounter(true);
        //		LoadEncounterIntoTestRealm (encounterSelection.uniqueEncounterID);
        EditorSceneManager.SaveScene(SceneManager.GetSceneByName("LevelEditor"));
        EditorSceneManager.CloseScene(SceneManager.GetSceneByName("LevelEditor"), false);
        EditorApplication.isPlaying = true;
        GameManager.Instance.isTesting = true;
    }

    private static void CreateZoneMapData()
    {
        //Refreshes the Zone Map List Built from the Asset Files 
        LevelEditorManager.zoneMapList =
            new List<ZoneMapData>(Resources.LoadAll<ZoneMapData>("LevelEditorObjects/ZoneMaps"));

        ZoneMapData asset = ScriptableObject.CreateInstance<ZoneMapData>();
        asset.ID = LevelEditorManager.zoneMapList.Count + 1;

        if (asset.ID < 10)
        {
            AssetDatabase.CreateAsset(asset,
                "Assets/ChromaShift/Resources/LevelEditorObjects/ZoneMaps/0" + asset.ID + "_ZMDO.asset");
        }
        else
        {
            AssetDatabase.CreateAsset(asset,
                "Assets/ChromaShift/Resources/LevelEditorObjects/ZoneMaps/" + asset.ID + "_ZMDO.asset");
        }

        AssetDatabase.SaveAssets();
    }

    private void SaveYumeMapData()
    {
        // Find the Game Object that holds the data
        // Loop through each layer
        // Loop through each object.
    }
    
    void SaveZoneData(bool forTest = false)
    {
        ZoneMap currentZoneMapObject;

        try
        {
            //Find Current Zone Map Object
            currentZoneMapObject = GameObject.Find("Zone Map").GetComponent<ZoneMap>();

            bool isConfirmdToSave = EditorUtility.DisplayDialog("Save Zone " + currentZoneMapObject.mapID,
                "Do you want to save this zone?",
                "SAVE",
                "Do Not Save"
            );

            //Confirm that we in fact want to save the current zone.
            if (!isConfirmdToSave)
            {
                return;
            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("No Zone Map Found",
                "There is currently no Zone Map Loaded in the Editor Hierarchy" + e, "OK");
            return;
        }

        //Actual Saving 
        if (!forTest)
        {
            //Double Check if Zone Actually Exists
            if (currentZoneMapObject == null)
            {
                return;
            }

            //Loop through all ZoneMapDataObjects in the LevelEditor ZoneMap List
            foreach (ZoneMapData ZMDO in LevelEditorManager.zoneMapList)
            {
                //If Currently Selected Zone has a Matching Zone Map Data Object
                if (currentZoneMapObject.mapID == ZMDO.ID)
                {
                    //Clear out the Data Object to prepare new encounted to be slotted
                    ZMDO.eList.Clear();

                    //Loop through all Encounter Maps in the Currently Selected Zone's Encounter Map list
                    foreach (EncounterMap encounter in currentZoneMapObject.encounters)
                    {
                        // Would like to do a check to make sure it's only saving the changed prefabs. No need to
                        // resave something that hasn't been modified.
                        PrefabUtility.ReplacePrefab(
                            encounter.gameObject,
                            PrefabUtility.GetCorrespondingObjectFromSource(
                                encounter.gameObject
                            )
                        ); //Prefabs DO NOT turn blue after saving with this method

                        //Add Encounter ID to the current Zone Map Data Object's Encounter List
                        ZMDO.eList.Add(encounter.uniqueEncounterID);
                    }
                }
            }

            EditorUtility.DisplayDialog("Zone " + currentZoneMapObject.mapID + " Saved", "Save Completed!", "OK");

            //TEST REALM SAVING
        }
        else
        {
            //Double Check if Zone Actually Exists
            if (currentZoneMapObject != null)
            {
                LevelEditorManager.zoneMapList[0].eList.Clear();
                foreach (EncounterMap encounter in currentZoneMapObject.encounters)
                {
                    // Would like to do a check to make sure it's only saving the changed prefabs. No need to
                    // resave something that hasn't been modified.
                    PrefabUtility.ReplacePrefab(encounter.gameObject,
                        PrefabUtility
                            .GetCorrespondingObjectFromSource(encounter
                                .gameObject)); //Prefabs DO NOT turn blue after saving with this method
                    LevelEditorManager.zoneMapList[0].eList.Add(encounter.uniqueEncounterID);
                }
            }
        }
    }

    void LoadZoneData(int zoneToLoad = 0)
    {
        //Ask if you want to save the current zone before loading
        if (!EditorUtility.DisplayDialog("Load Zone " + zoneToLoad,
            "Are you sure you want to load zone " + zoneToLoad + "?", "LOAD", "Exit"))
        {
            return;
        }
        else
        {
            //Get Current ZoneMap
            GameObject zoneMapGameObject = GameObject.Find("Zone Map");

            //Init the Zone found indicactor
            bool zoneFound = false;

            //Try to save current zone before loading
            SaveZoneData();

            //Loop through all available zones
            foreach (ZoneMapData ZMDO in LevelEditorManager.zoneMapList)
            {
                //Make sure the current zone has a connecting ZoneMapDataObject
                if (zoneToLoad == ZMDO.ID)
                {
                    //The the ZoneMapDataObject matches the currently selected zone
                    zoneFound = true;

                    //Clear Out Zone Game Object
                    ClearZoneElements(zoneMapGameObject);

                    //Loop though all encounter ids in the ZoneMapDataObject Encounter List
                    foreach (int eID in ZMDO.eList)
                    {
                        //Loop through all Encounter Game Objects in the Level Editor Manager
                        foreach (GameObject encounterGameObject in LevelEditorManager.encounterMapList)
                        {
                            //If the current Encounter ID matches the Unique Ecounter ID of the current Encounter Game Object
                            if (eID == encounterGameObject.GetComponent<EncounterMap>().uniqueEncounterID)
                            {
                                // Create Instances of Ecounter and Set it under the Zone Map Game Object
                                GameObject encounterObject =
                                    PrefabUtility.InstantiatePrefab(encounterGameObject as GameObject) as GameObject;
                                encounterObject.transform.SetParent(zoneMapGameObject.transform);
                            }
                        }
                    }

                    //Sets the Zone Map Game Object to the Newly Loaded Zone
                    zoneMapGameObject.GetComponent<ZoneMap>().mapID = zoneToLoad;
                }
            }

            if (!zoneFound)
                EditorUtility.DisplayDialog("Zone " + zoneToLoad + " Not Found",
                    "No Zone matching ID (" + zoneToLoad + ") Found!", "OK");
            else
                EditorUtility.DisplayDialog(
                    "Zone Load " + zoneMapGameObject.GetComponent<ZoneMap>().mapID + " Complete",
                    "Zone Load Completed!", "OK");
        }
    }

    void AddEncounter()
    {
        //Debug.Log ("Add Encounter Function: ");
        bool encounterChecker = false;
        foreach (GameObject encounter in LevelEditorManager.encounterMapList)
        {
            if (encounterToLoad == encounter.GetComponent<EncounterMap>().uniqueEncounterID)
            {
                encounterChecker = true;
                GameObject encounterObject = PrefabUtility.InstantiatePrefab(encounter as GameObject) as GameObject;
                encounterObject.transform.SetParent(GameObject.Find("Zone Map").transform);
                //Debug.Log ("Prefab: " + PrefabUtility.GetPrefabType (encounterObject));
            }
        }

        if (!encounterChecker)
            EditorUtility.DisplayDialog("Encounter (" + encounterToLoad + ") Not Found",
                "No Encounter matching ID (" + encounterToLoad + ") Found!", "OK");
    }

    void LoadEncounterIntoTestRealm(int id = 19)
    {
        ZoneMapData testZMDO = LevelEditorManager.zoneMapList[1];
        testZMDO.eList.Clear();
        testZMDO.eList.Add(1);
        testZMDO.eList.Add(2);
        testZMDO.eList.Add(id);
        testZMDO.eList.Add(3);
    }

    private void SaveEncounter(bool forTest = false)
    {
        UnityEngine.Object testEncounterCache =
            Resources.Load("LevelEditorObjects/EncounterMaps/00_Encounter_TestCache");

        try
        {
            encounterSelection = Selection.activeGameObject.GetComponent<EncounterMap>();
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("No Encounter Selected",
                "Please select a Encounter before trying to save \n " + e, "OK");
            return;
        }

        if (encounterSelection != null)
        {
            PrefabType selectionType = PrefabUtility.GetPrefabType(encounterSelection.gameObject);

            if (selectionType == PrefabType.None)
            {
                GameObject newPrefab = PrefabUtility.CreatePrefab(
                    "Assets/ChromaShift/Resources/LevelEditorObjects/EncounterMaps/" +
                    encounterSelection.gameObject.name + ".prefab", encounterSelection.gameObject);
                PrefabUtility.ConnectGameObjectToPrefab(encounterSelection.gameObject, newPrefab);
            }
            else
            {
                if (forTest)
                {
                    int UEIDCache = encounterSelection.uniqueEncounterID;
                    encounterSelection.uniqueEncounterID = 0;
                    PrefabUtility.ReplacePrefab(encounterSelection.gameObject,
                        PrefabUtility.GetPrefabObject(
                            testEncounterCache)); //Prefabs DO NOT turn blue after saving with this method
                    encounterSelection.uniqueEncounterID = UEIDCache;
                }
                else
                {
                    PrefabUtility.ReplacePrefab(encounterSelection.gameObject,
                        PrefabUtility.GetCorrespondingObjectFromSource(encounterSelection
                            .gameObject)); //Prefabs DO NOT turn blue after saving with this method
                }
            }
        }
        else
        {
            EditorUtility.DisplayDialog("No Encounter Selected", "Please select a Encounter before trying to save \n ",
                "OK");
        }
    }

    void RemoveEncounter()
    {
        try
        {
            encounterSelection = Selection.activeGameObject.GetComponent<EncounterMap>();
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("No Encounter Selected",
                "Please select a Encounter before trying to save \n " + e, "OK");
            return;
        }

        if (encounterSelection != null)
        {
            if ((encounterSelection.uniqueEncounterID == 1) || (encounterSelection.uniqueEncounterID == 2) ||
                (encounterSelection.uniqueEncounterID == 3))
            {
                EditorUtility.DisplayDialog("Cannot Remove Encounter",
                    "This is a Default Encounter. This cannot be removed from zone.", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Remove Encounter " + encounterSelection.transform.name,
                "Are you sure you want to Remove Encounter: " + encounterSelection.uniqueEncounterID, "OK", "Exit"))
            {
                return;
            }
            else
            {
                //Debug.Log("Delete Encounter Now.");
                DestroyImmediate(encounterSelection.gameObject);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("No Encounter Selected",
                "Please select a Encounter before trying to remove \n ", "OK");
            return;
        }
    }

    private void ClearTiles(GameObject YuMeMapData)
    {
        if (YuMeMapData.transform.childCount <= 0)
        {
            return;
        }
        
        foreach (Transform child in YuMeMapData.transform)
        {
            int i = 0;
            while (child.childCount > 0)
            {
                DestroyImmediate(child.GetChild(0).gameObject);
                if (i++ > 10)
                {
                    break;
                }
            }
        }
    }
    
    public void ClearZoneElements(GameObject zone)
    {   
        if (!zone.GetComponent<ZoneMap>())
        {
            Debug.LogError ("No Zone Map was attached to current Game Object");
            return;
        }

        while (zone.transform.childCount > 0)
        {
            Transform child = zone.transform.GetChild(0);
            child.parent = null;
            DestroyImmediate(child.gameObject);
        }
    }
}