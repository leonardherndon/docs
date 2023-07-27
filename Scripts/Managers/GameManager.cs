//I used Rusticode GameManager Tutorial for this

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using DG.Tweening;
using uGUIPanelManager;
using Com.LuisPedroFonseca.ProCamera2D;
using Chronos;
using CS_Audio;
using Huenity;
using PixelCrushers.DialogueSystem;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using ChromaShift.Scripts;
using ChromaShift.Scripts.ChallengeObject;
using ChromaShift.Scripts.Managers;
using ChromaShift.Scripts.Player;
using ChromaShift.Scripts.SaveGame;
using PixelCrushers.DialogueSystem.UnityGUI.Wrappers;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Rewired;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

public enum GameStateType{
    NullState,
    ApplicationStart,
    PlayerManagement,
    GameSettings,
    Loading,
    Gameplay,
    EndGameplay,
    //StartMenu,
    
    ApplicationEnd,
    
    ActiveATG,
    ZoneComplete,
    GameSequence,
    CutScene,
    GameOver,
    Paused,
    ZoneSelect,
    Pregame,
    Count
}

public enum GameSubStateType{
	NullState,
	SplashScreen,
	StartMenu,
	PMInventory,
	PMQuest,
	PMJournal,
	PMMap,
	ZoneSelect, // --
	ZoneComplete, // --
	GameOver,
	InteractiveSequence,
	CinematicSequence,
}
public enum GameColor {Red, Orange, Yellow, Green, Blue, Purple, White, Black, Grey, Pink, Cyan}

public enum PlayerState{Normal,TakingDamage,Dead}
public enum PowerUpName{Shield,Phase,QuantumKey,ColorBomb}
public enum ObjectTag{Untagged,Pickup,Field,Gate,Nebula,Enemy,Obstacle,Door,Trigger,Scene,Wall}
public enum HostileType {Nebula, Enemy, Obstacle, Door, Trigger, DangerZone}
public enum FriendlyType {Null, FusionCore, HealthPickup ,Field, Gate, Special}
public enum MovementType{Stationary, Hover, SwitchLane, MoveToPlayer, Wave, MoveHorizontal, CrossLane, HoverSwitch, MoveSwitch, MagMove, PlayerAbsorb, Flip, Dash, HunterMine, Rotate, Waypoint}
public enum MovementActivationType {DistanceToPlayer, TimeAlive, CollisionWithObject, Null}
public enum MovementExitType {DistanceToObject, DistanceFromObject, Time, CollisionWithObject, CameraEnter, CameraExit, Null, PlayerTriggerCollision, PlayerDistanceFromObject, PlayerDistanceToObject, PlayerDistanceFromSelf, PlayerDistanceToSelf, Special }
public enum TriggerEventListenerType {Location_X,Location_Y,ButtonPress}
public enum ImpactType {Null, Physical, Energy, Special}
public enum CollisionLayerType {Default,Player,Enemy,Obstacle,Nebula,Pickup,Trigger,pPlayer,pEnemy,pObstacle,pNebula,pPickup,pTrigger,Spawner,pSpawner,gTrigger,Scene}
public enum SpawnPosition {Right,Left,Top,Bottom,Background,Foreground}
public enum LaserType {StrongLaser,WeakLaser}
public enum DamageGroup {Null, Physical, Deflectable, Key, Death }
public enum LumenType {Null, Simple, Complex}
public enum LumenMode {Null, Light, Dark}
public enum ThreatGroup {Null, Neutral, Friendly, Hostile, Player, Special}
[Flags]
public enum StatusEffectType
{
	Astra = 1 << 1,
	Barrier = 1 << 2,
	Blackout = 1 << 3,
	Bravery = 1 << 4,
	Burn = 1 << 5,
	Chill = 1 << 6,
	Corrosion = 1 << 7,
	Corruption = 1 << 8,
	Crack = 1 << 9,
	Decay = 1 << 10,
	Disable = 1 << 11,
	Freeze = 1 << 12,
	Immolate = 1 << 13,
	Resist = 1 << 14,
	Shielded = 1 << 15,
	Shock = 1 << 16,
	Silence = 1 << 17,
	Slow  = 1 << 18,
	Stun = 1 << 19,
	Weaken = 1 << 20,
	All = 2097151 
}

public enum RequestOriginType {Null, Ability, Object, Environment, Equipment, Global}
public enum AttributeType {DataIntegrity, ThermalIntegrity, HullIntegrity, MentalIntegrity}
public enum EffectApplicationType {Null, DoT, Sustained, Velocity, Permanent}
public enum DamageApplicationType {Ignore, Instant, DoT, Sustained, Velocity, Delayed}
public enum ColorMatchType {Ignore, Normal, Negative, ExactMatch}
public  enum DamageSeverity {Dampened, Normal, Amplified}
[Flags]
public enum DamageImmunityType
{
	Null = 0,
	Physical = 1,
	Deflectable = 2,
	Key = 4,
	Death = 8,
	Velocity = 16,
	All = 127
}

[Flags]
public enum EnvironmentalTriggers
{
	PlayerDamage = 1 << 1,
	Velocity = 1 << 2,
	ShieldUse = 1 << 3,
	ColorShift = 1 << 4,
	LumenSwitch = 1 << 5,
	AbilityLeftUse = 1 << 6,
	AbilityRightUse = 1 << 7,
	ScannerUse = 1 << 8,
	PlayerSustainedTime = 1 << 9,
	EnemyDamage = 1 << 10,
	EnemyDeath = 1 << 11,
	EnemySustainedTime = 1 << 12
}

   
public delegate void OnStateChangeHandler();
public delegate void OnKillEffect(string sourceId);



	
public class GameManager : MonoBehaviour {
	private static GameManager _instance;
	public event OnStateChangeHandler OnStateChange;
	public event OnKillEffect OnKillEffectEvent = delegate { };
	public FusionCore[] EMPTYFC = new[] { new FusionCore(GameColor.Red, false, false,0), new FusionCore(GameColor.Green,false, false,0), new FusionCore(GameColor.Blue, false,false, 0)};
	
	
	public GameStateType gameState { get; private set; }
	[SerializeField]
	private bool lockFrameRate;
	[SerializeField]
	private int frameRateTarget;
    public int qualityLevel;
    
    [Header("[PLAYER INFO]")]
    public int currentSaveDataIndex = -1;
    public SaveGameData currentSaveDataObject;
    public int currentDeathCount;
    public int currentTimePlayed;
    public string currentLevelID;
    
    [Header("[GAMESTATE]")]
	public GameStateType prevGameState;

	private bool sceneIsLoading = false;
	public GameUIManager GUIM;
	public int sceneToBeLoaded;

	[Header("[DEVICE]")]
	public string currentDevice;

	[Header("[CONTROLS]")]
	public Player playerController;
    public EventSystem eventSystem;

	[Header ("[TIME CONTROL]")] 
	public Clock rootClock;
	public Clock uiClock;
	public Clock enemyClock;
	public Clock pickupClock;
	public Clock playerClock;

	public LevelTimer levelTimer;
	
	[Header("[LEVEL OBJECTS]")]
	public List<LevelData> levelDataList = new List<LevelData>();
	public GameObject warpGate;
	public List<EnemyShip> enemyShips;
	public LevelData currentLevelDataObject;
	public List<float> EncounterStartPositions = new List<float>();
	public GameObject currentProjectionObject;
    public GameObject cleanupCollider;
    public List<CheckPointTrigger> checkPointObject;
    public int checkPointIndex;
    public bool useCheckPoint;
    
    [Header("[SCENE OBJECTS")] 
    public int currentSceneIndex;
    public SceneData currentSceneDataObject;
    public GameObject spawnedObjectsHolder;

    [Header("[GLOBAL OBJECTS]")] public GameObject globalSpawnedObjectsHolder;

	[Header("[LEVEL STATS]")]
	public int currentLevelEnemiesDestroyed;
	public float currentLevelDistanceTravelled;
	public int currentLevelCoresInLevel;
	public int currentLevelEnemiesInLevel;
	public int currentLevelCoresCollected;
	public int currentLevelCoresLost;
	public float currentLevelTimeInLevel;
	public int currentLevelScore;
	public int currentLevelGrade;
    public int[] isLevelCompleted = new int[20];
    public Vector3 cameraSectorResetPosition = new Vector3(-1469f, 52f, -65f);
    public bool fromLevelCompleted = false;



    [Header("[DISPLAY/CAMERA]")]
	public ScreenFader screenFader;
	public ColorSuite colorSuite;
	public GameObject hangerPrefab;

	[Header("[PLAYER PROGRESSION]")]
	public PlayerShip playerShip;
	public int playerScore;
	public int earnedPoints;
	public int distanceScore;
	public Text playerScoreText;
	public Vector3 PLAYER_START_POSITION = new Vector3(-200,52,0);
    public GameObject[] fusionCores;

    [Header("[DIALOG MANAGAMENT]")]
    public bool USEDIALOGSYSTEM = false;
    public GameObject dialogueManager;

    [Header("[PLAYER MECHANIC TOGGLE]")]
	public bool LANESWITCHMECHANIC = true;

	[Header("[CAMERA SYSTEM]")]
	public ProCamera2D mainCamera;
	public float CameraStandardOffset = 30f;
	public Camera planetCamera;
    public Plane[] cameraPlanes;
    public SceneCamPathManager scenePath;

    [Header ("[LEVEL EDITOR]")]
	public GameObject currentZoneMap;
	public LevelData testRealmLDO;
	public bool isTesting = false;
	public bool removeTest = false;
	public List<ZoneMapData> zoneMapList = new List<ZoneMapData>();
	public List<GameObject> encounterMapList = new List<GameObject>();
	public Vector2 tileSize = new Vector2 (8, 8);
	public string cachedScenePath;
	public int cachedSelectedObject;
	public GameObject cachedHMP;

	[Header("[OBJECT POOLING]")]
	public FastPoolManager FPM;
	public List<GameObject> fastPoolObjects = new List<GameObject>();
	public GameObject SOH;
   
	protected GameManager() {}

	public static bool isActive {
		get {
			return _instance != null;
		}
	}

	//Singleton pattern implementation
	public static GameManager Instance {
		get 
		{
			if (_instance == null) 
			{
				_instance = Object.FindObjectOfType (typeof(GameManager)) as GameManager;

				if (_instance == null)
				{
					GameObject go = new GameObject("_gamemanager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<GameManager>();
				}
			}
			
			return _instance;
		}
	}

	//Init Application
	public void Awake()
	{
		if (GUIM)
			GUIM.gameObject.SetActive(true);
		
		//Debug.Log("Game Manager Awake called");
        qualityLevel = QualitySettings.GetQualityLevel();
        if (lockFrameRate)
        {
	        Application.targetFrameRate = frameRateTarget;
        }

        sceneToBeLoaded = 0;
		currentDevice = SystemInfo.deviceType.ToString ();
		//GameStateManager.Instance.SwitchState (GameState.NullState);

		DOTween.Init(true, true, LogBehaviour.ErrorsOnly).SetCapacity(200, 10);
//		fastPoolObjects = new List<GameObject> ();

		//SET CHRONOS CLOCKS
		rootClock = Timekeeper.instance.Clock ("Root");
		uiClock = Timekeeper.instance.Clock ("UI");
		enemyClock = Timekeeper.instance.Clock ("Enemy");
		pickupClock = Timekeeper.instance.Clock ("Pickup");
		playerClock = Timekeeper.instance.Clock ("Player");

        //SET EVENT SYSTEM FOR ACCESS
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }
		
		InitGame ();

        if (AudioManager.Instance.musicAudioSource.clip != AudioManager.Instance.musicClips[0])
        {
            AudioManager.Instance.musicAudioSource.Stop();
            AudioManager.Instance.musicAudioSource.clip = AudioManager.Instance.musicClips[0];
            AudioManager.Instance.musicAudioSource.loop = true;
            AudioManager.Instance.musicAudioSource.Play();
        }
    }

    public void InitGame()
    {
	    InitGameData();
        //playerShip.InitPlayerShip();
        InitGameCamera();
        InitControlSystem();
        InitLevelDataObjects();
    }

    public void InitGameData()
    {
	    //LOAD MASTER DATA TO FIGURE OUT WHICH DATA FILE TO WORK WITH
	    SaveGameManager.Instance.LoadMasterData();
	    LoadGameData();
    }

    public void LoadGameData()
    {
    //Load Game Data from Files
	    SaveGameManager.Instance.LoadGameData(currentSaveDataIndex);
	    
	    //FETCH LEVEL DATA OBJECTS TO LOAD INTO GAME MANAGER
	    var levelList = Resources.LoadAll("LevelData");
	    foreach (LevelData level in levelList)
	    {
		    if (currentLevelID == level.Code)
		    {
			    currentLevelDataObject = level;
			    
			    //Debug.Log("Scene Index: " + currentSceneIndex);
			    SetLevelToBeLoaded(null, currentLevelDataObject);
		    }
	    }

	    
	    

	    /*//LOAD IN CHECK POINT
	    checkPointIndex = currentSaveDataObject.currentCheckPointIndex;
	    
	    //LOAD IN DEATH COUNT
	    currentDeathCount = currentSaveDataObject.currentDeathCount;

	    //LOAD IN TIME PLAYED
	    currentTimePlayed = currentSaveDataObject.currentTimePlayed;*/
    }

    public void InitGameCamera() {
		//Init Main Camera
		mainCamera = GameObject.Find ("MainCamera").GetComponent<ProCamera2D>();
		mainCamera.enabled = false;
		//playerShip.proCamera = mainCamera;
		colorSuite = mainCamera.GetComponent<ColorSuite> ();
        //if (qualityLevel != 0)
           // mainCamera.GetComponent<PostProcessingBehaviour>().profile = null;
	}

	public void InitControlSystem () {
        //Debug.Log("initcontroller");
		playerController = ReInput.players.GetPlayer (0);
	}

	public void InitLevelDataObjects() {
		//Init Zone Maps
		zoneMapList = new List<ZoneMapData>(Resources.LoadAll<ZoneMapData>("LevelEditorObjects/ZoneMaps"));
		encounterMapList = new List<GameObject>(Resources.LoadAll<GameObject>("LevelEditorObjects/EncounterMaps"));

	}

	public void GameOver() {
		//Save Game STATS
		Debug.Log("Game Over Sequence Started");
		//ENABLE GAME UI 
		// - SHOW SCORE
		GameStateManager.Instance.SwitchState(GameStateManager.Instance.EndGameplayState);
	}

	public void StartGame() {
		//Restart Frame
		//Debug.Log ("Starting Game");
		GameStateManager.Instance.SwitchState (GameStateManager.Instance.LoadingState);
	}

	/// <summary>
	/// Menu Control to open the How To Play screen
	/// </summary>
	public void OpenHowToPlay() {
        uGUIManager.SetPanelState("Panel_HowToPlay", PanelState.Show, additional: true, queued: false, instant: true);
        GameUIManager.Instance.UIElementsSplashScreen[7].GetComponent<Button>().Select();
        GameUIManager.Instance.UIElementsSplashScreen[7].GetComponent<Button>().OnSelect(null);
    }

	/// <summary>
	/// Menu Control to close the How To Play screen
	/// </summary>
    public void BackOutHowToPlay()
    {
        uGUIManager.SetPanelState("Panel_HowToPlay", PanelState.Hide, additional: true, queued: false, instant: true);
        GameUIManager.Instance.UIElementsSplashScreen[6].GetComponent<Button>().Select();
        GameUIManager.Instance.UIElementsSplashScreen[6].GetComponent<Button>().OnSelect(null);
    }

	/// <summary>
	/// Menu Control to go back to the Main Screen / Splash Screen
	/// </summary>
    public void BackToSplashScreen() {
		GameStateManager.Instance.SwitchState (GameStateManager.Instance.ApplicationStartState);
    }

	public void ContinueGame()
	{
		Debug.Log("Continue Game");
		DestroyCurrentScene();
		GoToGameScene(currentSceneIndex);
	}

	public void DestroyCurrentScene()
	{
		foreach(GameObject spawnedObject in fastPoolObjects) {
			Destroy (spawnedObject);
		}

		
		unloadGamePlayScenes ();
		Resources.UnloadUnusedAssets ();

	}
	
	public void RestartGame()
	{

		DestroyCurrentScene();
		
		//uGUIManager.SetPanelState("Panel_LoadGame",PanelState.Hide,additional: false,queued: false, instant: true);
		GameStateManager.Instance.SwitchState (GameStateManager.Instance.ApplicationStartState);
		
		//playerShip.InitPlayerShip ();
		ResetGameManager ();
        GameManager.Instance.eventSystem.SetSelectedGameObject(GameUIManager.Instance.UIElementsSplashScreen[2]);
    }

	//GoToGameScene will go to the next scene in the LDO list unless a SceneNumber is given.
	public void GoToGameScene(int sceneNumber = -1)
	{
		foreach (GameObject spawnedObject in fastPoolObjects)
		{
			Destroy(spawnedObject);
		}

		unloadGamePlayScenes();
		Resources.UnloadUnusedAssets();
		GameStateManager.Instance.SwitchState(GameStateManager.Instance.LoadingState);
		if(sceneNumber > -1) {
			currentSceneIndex = sceneNumber;
			//playerShip.InitPlayerShip();
		} else {
			currentSceneIndex++;
			currentLevelDataObject.currentFusionCores = new GameColor[playerShip.fusionCoreInventory.Count];
			for(int i = 0; i < playerShip.fusionCoreInventory.Count; i++)
			{
				currentLevelDataObject.currentFusionCores[i] = playerShip.fusionCoreInventory[i].ColorIndex;
			}
			
			//playerShip.InitPlayerShip();
		}
		SetSceneToBeLoaded(currentLevelDataObject.levelSceneList[currentSceneIndex]);
		
		
		LoadSceneWrap();
	}

	private void ResetGameManager() {
		prevGameState = GameStateType.NullState;
		sceneIsLoading = false;
		sceneToBeLoaded = 0;

		warpGate = GameObject.Find ("WarpGate");
		enemyShips = new List<EnemyShip>();
		currentLevelDataObject = null;

		mainCamera = GameObject.Find ("MainCamera").GetComponent<ProCamera2D>();
		screenFader = null;
		colorSuite = mainCamera.GetComponent<ColorSuite>();
		levelTimer.InitTimer();
		//checkPointIndex = 0;
        
		//		GameObject playerShipObject = PrefabUtility.InstantiatePrefab (playerShip as GameObject) as GameObject;
		//		playerShip = playerShipObject.GetComponent<PlayerShip>();

		playerScore = 0;
		earnedPoints = 0;
		distanceScore = 0;

		LANESWITCHMECHANIC = false;
		currentZoneMap = null;
		isTesting = false;
		removeTest = false;
		DOTween.Clear ();
	}

	public IEnumerator DelayGame(float waitTime) {
		print("Begin wait() " + Time.time);
		yield return new WaitForSeconds(waitTime);
		print("End wait() " + Time.time);
	}

	public void UpdateScore(int newPoints) {
		//distanceScore = (int) GameUIManager.Instance.GetDistanceScore ();
		//if (distanceScore < 0)
		//	distanceScore = 0;
		earnedPoints = earnedPoints + newPoints; 
		currentLevelScore = distanceScore + earnedPoints;


	}

	public void ResetScore() {
		playerScore = 0;
	}

	public void SetLevelToBeLoaded(LevelDataHolder LDOH = null, LevelData LDO = null) {
		
		//GameUIManager.Instance.OpeneSectorDescriptionPanel();
        //GameUIManager.Instance.OpenDemoSceneSelectPanel();
		//currentSceneIndex = 0;
		if (LDOH)
		{
			currentLevelDataObject = LDOH.LDO;
			SetSceneToBeLoaded(LDOH.LDO.levelSceneList[currentSceneIndex]);
		}
		else if (LDO)
		{
			currentLevelDataObject = LDO;
			SetSceneToBeLoaded(LDO.levelSceneList[currentSceneIndex]);
		}
		else
		{
			Debug.LogError("No level data to be loaded. Please Fix");
		}
    }
	
	public void SetSceneToBeLoaded(SceneData SDO) {
        
		sceneToBeLoaded = SDO.buildSceneIndex;
		currentSceneDataObject = SDO;
	}

	public void SaveScore () {
		PlayerPrefs.SetInt ("PlayerScore", playerScore);
	}


	//LoadSceneWrap is also being called from the UI element [UIB - SecMap_Btn_Deploy]
	public void LoadSceneWrap(){
		
        if (sceneToBeLoaded == 0) {
			AudioManager.Instance.uiAudioSource.clip = AudioManager.Instance.deniedClip;
			AudioManager.Instance.uiAudioSource.Play();
			return;
		}
		if(sceneIsLoading == false){
			AudioManager.Instance.uiAudioSource.clip = AudioManager.Instance.startClip;
			AudioManager.Instance.uiAudioSource.Play();
			StartCoroutine(LoadGameScene());
		}
	}

	public IEnumerator LoadGameScene() {
        
        float loadTimer = Time.time;
        GameStateManager.Instance.SwitchState(GameStateManager.Instance.LoadingState);
        //Init Controller
		PlayerInputController.Instance.DisableAllControls();
		sceneIsLoading = true;
//		if(checkPointIndex == 0 && currentSceneDataObject.name == "SDO - 3B_A")
//			AudioManager.Instance.PlayClipWrap(2, 0, GameManager.Instance.currentLevelDataObject.descriptionAudio,false, 1);
		RenderSettings.skybox = currentSceneDataObject.skyBox;
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneToBeLoaded, LoadSceneMode.Additive);
		//mainCamera.transform.position = new Vector3 (0, 30f, -66f);
 
		while (!loadScene.isDone || Time.time < loadTimer + 5 || AudioManager.Instance.audioSources[2].isPlaying)
		{
			if (playerController.GetButtonDown("Confirm") && Time.time > loadTimer + 5)
			{	
				AudioManager.Instance.StopPlayingClip(2);
				break;
			}

			yield return null;
		}

		yield return new WaitForSeconds(0.25f);
		//Debug.Log ("Scene is Loaded");
		InitGameLevelStats();

//		LoadZoneMap();
		//LoadZoneMapIntoLevel(currentLevelDataObject.availableZoneMaps[0].ID);
		// @TODO Comment out the load zone map into level
		
		InitLevelObjects ();

		sceneIsLoading = false;
//		currentLevelDataObject = GameObject.Find ("LevelData").GetComponent<LevelDataHolder> ().LDO;
		GUIM.playerShip = playerShip;
		GUIM.LoadTriggerCollisions();
		
        playerShip.InitPlayerShip();

        if (useCheckPoint)
        {
	        PlayerInputController.Instance.MoveShipToCheckpoint(checkPointIndex);
        }
        
        //Debug.Log("Init Level Camera");
		InitLevelCamera ();
		InitCutsceneObjects();
//		Debug.Log("CutScene Object Initialized");
		mainCamera.enabled = true;
//		uGUIManager.SetPanelState("Panel_LoadGame",PanelState.Hide,additional: false,queued: false, instant: true);
        GameStateManager.Instance.SwitchState(GameStateManager.Instance.GameplayState);
        HueManager.Instance.SetSceneLights(currentSceneDataObject.sceneMainColors[0],currentSceneDataObject.sceneMainColors[1]);
        HueManager.Instance.RestoreAllLights();
        //PlayerInputController.Instance.EnableAllControls();
	}

	

	public void InitGameLevelStats() {
		currentLevelEnemiesDestroyed = GameObject.FindObjectsOfType<Hostile>().Length;
		currentLevelDistanceTravelled = 0;
		currentLevelCoresInLevel = GameObject.FindObjectsOfType<PickupFusionCore>().Length;
		currentLevelCoresCollected = 0;
		currentLevelCoresLost = 0;
		currentLevelTimeInLevel = 0;
		currentLevelScore = 0;
		currentLevelGrade = 0;
	}

	public void InitCutsceneObjects()
	{
		foreach (GameObject csGo in GameObject.FindGameObjectsWithTag("CutScene Objects"))
		{
			TimelineAsset tLS = (TimelineAsset)csGo.GetComponent<PlayableDirector>().playableAsset;
			csGo.GetComponent<PlayableDirector>()
				.SetGenericBinding(tLS.GetOutputTrack(0), dialogueManager);
		}
	}
	public void InitLevelCamera() {

		if (currentProjectionObject)
			DestroyImmediate (currentProjectionObject);
		mainCamera.transform.position = cameraSectorResetPosition;
		mainCamera.transform.rotation = Quaternion.identity;
		mainCamera.GetComponent<Camera>().enabled = true;
		mainCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
		
        
		//if (planetCamera.gameObject.activeSelf == false) {
			if (currentSceneDataObject.useProjection) {
                if(planetCamera)
				    planetCamera.gameObject.SetActive (true);
                //Debug.Log("Planet Camera should be on now!");
				if (currentSceneDataObject.projectionObject) {
					currentProjectionObject = Instantiate (currentSceneDataObject.projectionObject, new Vector3 (mainCamera.transform.position.x, mainCamera.transform.position.y + 4.5f, mainCamera.transform.position.z + 250f), Quaternion.identity);
                    //Debug.Log("Projection Object:" + currentProjectionObject);
                    currentProjectionObject.transform.SetParent (mainCamera.transform);
                    //Debug.Log("Projection Parent:" + currentProjectionObject.transform.parent.name);
                }      
			} else {
                planetCamera.gameObject.SetActive(false);
			}
		//}
	}

	public void InitLevelObjects()
	{
		List<GameObject> tileGroup = new List<GameObject>();
//		List<Transform> tileObjects = new List<Transform>();;

		GUIM.BeginLevelMarker = GameObject.Find("BeginSceneMarker");
		GUIM.EndLevelMarker = GameObject.Find("EndSceneMarker");
		EncounterStartPositions.Clear();

		foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
		{
			if (
				go.gameObject.GetComponent<EncounterMap>() 
				&& go.gameObject.GetComponent<EncounterMap>().transform.parent.gameObject.GetComponent<ZoneMap>()
			) {
				EncounterStartPositions.Add(go.gameObject.GetComponent<EncounterMap>().transform.position.x);
			}
			
			if (go.name == "Tiles")
			{
				tileGroup.Add(go);
			}
		}

		var LDOH = GameObject.Find("LevelData").GetComponent<LevelDataHolder>();
		if (LDOH)
		{
			checkPointObject = LDOH.checkpoints;
		}
		
		//int loopIndex = 0;
		foreach (GameObject tileObject in tileGroup)
		{
			Transform[] tileList = tileObject.GetComponentsInChildren<Transform>();
			foreach (Transform tile in tileList)
			{
				BeginLevelTrigger BLT = tile.GetComponent<BeginLevelTrigger>();
				EndLevelTrigger ELT = tile.GetComponent<EndLevelTrigger>();
				Transform TempObject = tile.GetComponentInChildren<Transform>();

				if (BLT)
				{
					GUIM.BeginLevelMarker = tile.gameObject;
				}
				else if (ELT)
				{
					GUIM.EndLevelMarker = tile.gameObject;
				}

				tile.position = new Vector3(tile.position.x, tile.position.y, 0);

				if (tile.name == "TileDesign")
					tile.gameObject.SetActive(false);
			}
		}

		EncounterStartPositions.Sort();
	}

	public void ExitApplication() {
		SaveGameManager.Instance.SaveGameData(currentSaveDataIndex);
        Application.Quit();
	}

	public int GetCollisionLayer(CollisionLayerType type) {
		switch (type) {
			case CollisionLayerType.Player:
				return 9;
			case CollisionLayerType.Enemy:
				return 12;
			case CollisionLayerType.Obstacle:
				return 13;
			case CollisionLayerType.Nebula:
				return 14;
			case CollisionLayerType.Pickup:
				return 15;
			case CollisionLayerType.Trigger:
				return 16;
			case CollisionLayerType.pPlayer:
				return 17;
			case CollisionLayerType.pEnemy:
				return 18;
			case CollisionLayerType.pObstacle:
				return 19;
			case CollisionLayerType.pNebula:
				return 20;
			case CollisionLayerType.pPickup:
				return 21;
			case CollisionLayerType.pTrigger:
				return 22;
			case CollisionLayerType.Spawner:
				return 23;
			case CollisionLayerType.pSpawner:
				return 24;
			case CollisionLayerType.gTrigger:
				return 25;
			case CollisionLayerType.Scene:
				return 28;
			default:
				return 0;
		}
	}

	public void unloadGamePlayScenes () {
		for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
			//Debug.Log ("UNLOAD SCENE " + i);
			Scene  tempScene = SceneManager.GetSceneByBuildIndex (i);
			if(SceneManager.GetSceneByBuildIndex(i) != null && tempScene.buildIndex != 0) {
				if(tempScene.isLoaded){
					//Debug.Log ("Unload Scene: " + i);
					SceneManager.UnloadSceneAsync (i);
				}
			}
		}
	}

	public void LoadIntro() {
	}
}
