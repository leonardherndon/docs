using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using uGUIPanelManager;
using UnityEngine.EventSystems;
using Chronos;
using CS_Audio;
using EnergyBarToolkit;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameUIManager : MonoBehaviour {

	private static GameUIManager _instance;
	
    [Header ("[GENERAL]")]
	public PlayerShip playerShip;
	public PlayerInputController PIC;

	[Header ("[CANVAS]")]
	public GameObject ApplicationStartCanvas;
	public GameObject SplashScreenCanvas;
	public GameObject StartMenuCanvas;
	public GameObject GameSetupCanvas;
	public GameObject LoadingCanvas;
	public GameObject GameplayCanvas;
	public GameObject CutsceneCanvas;
	
	public GameObject PauseCanvas;
	public GameObject InsightsCanvas;
	public GameObject InventoryCanvas;
	public GameObject JournalCanvas;
	public GameObject QuestCanvas;
	
	public GameObject AreaCompleteCanvas;
	public GameObject EndGameplayCanvas;
	
	public GameObject SettingsMenuCanvas;
	public GameObject SaveLoadCanvas;
	
	
	
	public List<GameObject> UIElementsSplashScreen = new List<GameObject>();
    public List<GameObject> UIElementsSectorMap = new List<GameObject>();
    public GameObject UIButtonCacheSectorMap;
    public List<GameObject> UIElementsActiveMain = new List<GameObject>();

    public List<Text> GameStatsElements = new List<Text>();
	public List<TypeOutScript> SectorMapTextElements = new List<TypeOutScript>();
    public List<Button> SectorMapButtons = new List<Button> ();
    public List<Button> DemoSceneButtons = new List<Button> ();

    //ACTIVE MAIN VARS
	[Header ("[ACTIVE MAIN]")]
	[FormerlySerializedAs("endOfLevelTrigger")] public GameObject EndLevelMarker;
	[FormerlySerializedAs("beginningOfLevelTrigger")] public GameObject BeginLevelMarker;
	public TypeOutScript textBox;
	//public GameObject AM_textField;

	public AudioSource _audioSource;
	public AudioClip[] audioClips;
	//APPROACH THE GATE
	[Header ("[APPROACH THE GATE]")]
	public Text ATG_fusionCoreText;
    public EnergyBar ATG_SuccessRate;
    //public CameraFilterPack_Color_GrayScale camFilter;


    //ConversationTrigger Helper
    public List<ConversationTrigger> cTs;


	//Singleton pattern implementation
	public static GameUIManager Instance {
		get 
		{
			if (_instance == null) 
			{
				_instance = UnityEngine.Object.FindObjectOfType (typeof(GameUIManager)) as GameUIManager;


				if (_instance == null)
				{
					GameObject go = new GameObject("_gameuimanager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<GameUIManager>();
				}
			}
			return _instance;
		}
	}

	void Awake() {
		playerShip = GameManager.Instance.playerShip;
	}

	void Start() {
		//GUI Text
		//CSM = gameObject.AddComponent<ChromaShiftManager>();
		//InitTextBox();

		//for(int i=0;i < SectorMapTextElements.Count;i++) {
		//	SectorMapTextElements[i].FinalText = "";
		//	SectorMapTextElements[i].reset = true;
		//	SectorMapTextElements[i].On = false;
		//}

		
		//GameUIManager.Instance.UIElementsActiveMain[4].SetActive (false);

	}
	public void ResetSectorMap (bool fromSplash = true) {
        if(!fromSplash)
        {
            UIElementsSectorMap[2].SetActive(false);
        }
        
        GameManager.Instance.mainCamera.enabled = false;

        for (int i = 0; i < SectorMapButtons.Count; i++) {
            if (GameManager.Instance.isLevelCompleted[i] != 1)
            {
                SectorMapButtons[i].interactable = true;
            } else
            {
                SectorMapButtons[i].interactable = false;
            }
        }

        for (int i=0;i < SectorMapTextElements.Count;i++) {
			SectorMapTextElements[i].FinalText = "";
			SectorMapTextElements[i].reset = true;
			SectorMapTextElements[i].On = false;
		}
	}

	public void InitTextBox() {
		if (!textBox) {
			textBox = UIElementsActiveMain [2].GetComponent<TypeOutScript> ();
		}
		textBox.reset = true;
	}


	#region OnGUI
	void OnGUI() {
		
		//------------------------------------
		#region ACTIVE APPROACH THE GATE (ATG)
		//------------------------------------

		if (GameManager.Instance.gameState == GameStateType.ActiveATG) {
			//Debug.Log ("ATG");
			//ATG_fusionCoreText.text = "Fusion Cores: ";
			//for (int i = 0; i < playerShip.fusionCoreInventory.Count; i++) {
			//ATG_fusionCoreText.text = ATG_fusionCoreText.text + " | " + playerShip.fusionCoreInventory [i].ColorIndex;
			//}
            ATG_SuccessRate.SetValueCurrent(Mathf.RoundToInt(GameManager.Instance.playerShip.warpCharge));

            //camFilter._Fade = Math.Abs((GameManager.Instance.playerShip.warpCharge - 100) / 100f);
        }
		
		#endregion

	}
	#endregion


	public void OpenTextBox(bool requiredInput = false){
        textBox.FinalText = "";
        textBox.GetComponent<Text>().text = "";
        GameUIManager.Instance.UIElementsActiveMain[4].GetComponent<CanvasGroup>().alpha = 1;

        if (requiredInput) {
			PIC.CachedDisableControl ();
			Timekeeper.instance.Clock ("Actives").LerpTimeScale (0, 0.5f, false);
		}

	}

	public void ClearTextBox(){
        textBox.GetComponent<Text>().text = "";
        textBox.FinalText = "";
        textBox.reset = true;
        textBox.On = false;
        textBox.reset = true;
        GameUIManager.Instance.UIElementsActiveMain[4].GetComponent<CanvasGroup>().alpha = 0;
        
        //Uses the current Timescale instead of a hardcoded one
        //Timekeeper.instance.Clock ("Actives").LerpTimeScale(1,Timekeeper.instance.Clock("Actives").timeScale,false);
        //Timekeeper.instance.Clock ("Actives").LerpTimeScale(1,0.5f,false);

        PIC.RestoreControl();
        //Hide both Text Field options.
        //GameUIManager.Instance.UIElementsActiveMain[4].SetActive(false);
        GameUIManager.Instance.UIElementsActiveMain[4].GetComponent<CanvasGroup>().alpha = 0;
        //uGUIManager.SetPanelState("AM_TextBlock", PanelState.Hide, additional: true, queued: false, instant: true);
        

    }

	public void GatherGameStats () {

		//DISTANCE
		float distanceCalc =  GetDistancePercent() * 100;
		int distanceStat = (int) distanceCalc;
		GameStatsElements[0].text = distanceStat + " %";
		//GameStatsElements[1].text = ;

		//ENEMIES DESTORYED
		GameStatsElements[2].text = GameManager.Instance.currentLevelEnemiesDestroyed.ToString();
		//GameStatsElements[3].text = ;

		//CORES COLLECTED
		GameStatsElements[4].text = GameManager.Instance.currentLevelCoresCollected + " / " + GameManager.Instance.currentLevelCoresInLevel;
		//GameStatsElements[5].text = ;

		//CORES LOST
		GameStatsElements[6].text = GameManager.Instance.currentLevelCoresLost.ToString();
		//GameStatsElements[7].text = ;

		//TIME
		GameStatsElements[8].text = GameManager.Instance.currentLevelTimeInLevel.ToString();
		//GameStatsElements[9].text = ;

		//SCORE
		GameStatsElements[10].text = GameManager.Instance.currentLevelScore.ToString();
		//GameStatsElements[11].text = ;

		//GRADE
		GameStatsElements[12].text = GameManager.Instance.currentLevelGrade.ToString();
		//GameStatsElements[13].text = ;
	}

	public float GetDistancePercent() {
		if (BeginLevelMarker == null || EndLevelMarker == null)
		{
			return 0;
		}
		
		float distancePercent = 
				(playerShip.transform.position.x - BeginLevelMarker.transform.position.x) 
				/ EndLevelMarker.transform.position.x
			;
		if (distancePercent > 1)
			distancePercent = 1;
		if (distancePercent < 0)
			distancePercent = 0;
		return distancePercent;
	}

	public float GetDistanceScore() {
		float distancePercent = GetDistancePercent();
		float distanceScore =  (distancePercent) * GameManager.Instance.currentLevelDataObject.scorePotential;
		//Debug.Log ("Score Potential: " + GameManager.Instance.currentLevelDataObject.scorePotential + " | " +"Distance Percent: " + distancePercent + " | " + "Distance Score: " + distanceScore );
		return distanceScore;
	}

	
    public void OpenSectorDescriptionPanel()
    {
        ResetSectorMap();
        UIButtonCacheSectorMap =  GameManager.Instance.eventSystem.currentSelectedGameObject;
        uGUIManager.SetPanelState("Panel_SectorDescription", PanelState.Show, additional: true, queued: true, instant: false);
        GameObject.Find("UIB - SecMap_BackButton").GetComponentInChildren<Button>().interactable = false;
        
        foreach (Button button in GameObject.Find("Sector Panel Holder").GetComponentsInChildren<Button>())
        {
            button.interactable = false;
        }
        GameManager.Instance.eventSystem.SetSelectedGameObject(UIElementsSectorMap[5]);
        UIElementsSectorMap[6].GetComponent<Image>().sprite = GameManager.Instance.currentLevelDataObject.levelImage;
        AudioManager.Instance.StopPlayingClip(3);
        AudioManager.Instance.PlayClipWrap(3, 0, GameManager.Instance.currentLevelDataObject.descriptionAudio);
    }

    public void CloseSectorDescriptionPanel()
    {
        foreach (Button button in GameObject.Find("Sector Panel Holder").GetComponentsInChildren<Button>())
        {
            if(button.name == "Icon_Sector_2-A")
                button.interactable = true;
            if (button.name == "Icon_Sector_3-B")
                button.interactable = true;
            if (button.name == "Icon_Sector_3-C")
                button.interactable = true;
            if (button.name == "Icon_Sector_0-Leo")
                button.interactable = true;
            if (button.name == "Icon_Sector_0-Raymond")
                button.interactable = true;
        }
        GameObject.Find("UIB - SecMap_BackButton").GetComponentInChildren<Button>().interactable = true;
        GameManager.Instance.eventSystem.SetSelectedGameObject(UIButtonCacheSectorMap);
        UIButtonCacheSectorMap.GetComponent<Button>().Select();
        UIButtonCacheSectorMap.GetComponent<Button>().OnSelect(null);
        uGUIManager.SetPanelState("Panel_SectorDescription", PanelState.Hide, additional: true, queued: true, instant: false);
        AudioManager.Instance.StopPlayingClip(3);
    }

    public void LoadTriggerCollisions()
    {
	    if (!GameManager.Instance.currentZoneMap)
	    {
		    return;
	    }
        
	    Transform zoneMap = GameManager.Instance.currentZoneMap.transform;

        foreach (Transform child in zoneMap)
        {
            if(child.GetComponent<ConversationTrigger>())
                child.GetComponent<ConversationTrigger>().actor = GameManager.Instance.playerShip.transform;
        }

    }
    
    public void OpenDemoSceneSelectPanel()
    {
	    UIButtonCacheSectorMap =  GameManager.Instance.eventSystem.currentSelectedGameObject;
	    
	    for (int i = 0; i < DemoSceneButtons.Count; i++) {
		    SectorMapButtons[i].interactable = false;
	    }
	    
	    uGUIManager.SetPanelState("Panel_DemoSectorSelect", PanelState.Show, additional: true, queued: true, instant: false);

	    for (int i = 0; i < DemoSceneButtons.Count; i++) {
		    SectorMapButtons[i].interactable = true;
	    }

	    GameManager.Instance.eventSystem.SetSelectedGameObject(UIElementsSectorMap[9]);
	    AudioManager.Instance.StopPlayingClip(3);
	    //AudioManager.Instance.PlayClip(3, 0, GameManager.Instance.currentLevelDataObject.descriptionAudio);
    }
    
    
    //ONLY USE THIS METHOD FOR THE DEMO
    public void DemoSceneSelected()
    {
	    GameManager.Instance.mainCamera.enabled = false;

	    UIButtonCacheSectorMap =  GameManager.Instance.eventSystem.currentSelectedGameObject;
	    
	    for (int i = 0; i < DemoSceneButtons.Count; i++) {
		    SectorMapButtons[i].interactable = false;
	    }
	    
	    uGUIManager.SetPanelState("Panel_DemoStartMission", PanelState.Show, additional: false, queued: true, instant: true);

	    GameManager.Instance.eventSystem.SetSelectedGameObject(UIElementsSectorMap[0]);
	    //AudioManager.Instance.StopPlayingClip(3);
	    
    }
    
    public void CloseDemoStartMissionPanel()
    {
	    /*foreach (Button button in GameObject.Find("DemoSceneHolder").GetComponentsInChildren<Button>())
	    {
		    if(button.name == "Icon_Sector_3-B Scene 1")
			    button.interactable = true;
		    if (button.name == "Icon_Sector_3-B Scene 2")
			    button.interactable = true;
		    if (button.name == "Icon_Sector_3-B Scene 3")
			    button.interactable = true;
	    }*/
	    
	    GameManager.Instance.eventSystem.SetSelectedGameObject(UIButtonCacheSectorMap);
	    UIButtonCacheSectorMap.GetComponent<Button>().Select();
	    UIButtonCacheSectorMap.GetComponent<Button>().OnSelect(null);
	    //uGUIManager.SetPanelState("Panel_DemoStartMission ", PanelState.Hide, additional: true, queued: true, instant: false);
    }

    public void BackOutMenu()
    {
	    var demoStartMission = GameObject.Find("Panel_DemoStartMission");
	    var sectorMap = GameObject.Find("Panel_SectorMap");
	    var optionsPanel = GameObject.Find("Panel_Options");
	    var howToPlayPanel = GameObject.Find("Panel_HowToPlay");
	    
	    if (demoStartMission)
	    {
		    uGUIManager.SetPanelState("Panel_DemoStartMission", PanelState.Hide, additional: true, queued: true, instant: true);
		    uGUIManager.SetPanelState("Panel_SectorMap", PanelState.Show, additional: true, queued: true, instant: true);
		    CloseDemoStartMissionPanel();
	    }
	    else if (sectorMap || optionsPanel || howToPlayPanel)
		    GameManager.Instance.BackToSplashScreen();
	    
	    _audioSource.PlayOneShot(audioClips[0]);
    }
    
    public void OpenHueLightManager()
    {
	    Debug.Log("Attempting to Open Light Manager");
		    if (SceneManager.GetActiveScene().buildIndex == 0)
		    {
			    SceneManager.LoadScene(1);
		    }
    }

    public void ReturnToGameScene()
    {
	    if (SceneManager.GetActiveScene().buildIndex == 1)
	    {
		    SceneManager.LoadScene(0);
	    }
    }

    public string GetUIPanelFromGameObject(GameObject UIObject)
    {
	    if (!UIObject.TryGetComponent(out uGUIManagedPanel panel))
	    {
		    Debug.LogError("The GameObject does not have a uGUIManagedPanel component: " + UIObject.name);
		    return null;
	    }

	    return UIObject.name;
    }
    
}
