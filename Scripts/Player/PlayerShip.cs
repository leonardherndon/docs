using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Managers;
using ChromaShift.Scripts.ObjectAttributeSystem;
using ChromaShift.Scripts.Player;
using DG.Tweening;
using Chronos;
using CS_Audio;
using MoreMountains.Feedbacks;
using QuickEngine.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = System.Random;
using SlingshotShield = ChromaShift.Scripts.Player.SlingshotShield;


/// <summary>
/// Player ship. Working on changing Collisions into Triggers.
/// </summary>

public class PlayerShip : BaseBehaviour
{
    

    public PlayerInputController PIC;
    public GameObject shipRender;
    public GameObject shipBody;
    public Rigidbody rB;
    private Clock _clock;
    [SerializeField] private GameObject PlayerManagementTools;
    public GameObject PMT;
    private bool _init = false;
    
    [FormerlySerializedAs("shieldAbility")]
    [Header("[ABILITIES]")]
    [SerializeField] public AbilityPlayerShieldBase shieldFixedAbility;
    
    
    public ShieldFX sFX;
    public float shieldDuration = 25f;
    public GameObject shieldModel;
    [FormerlySerializedAs("isAbsorbActive")] public bool isShieldActive;
    
    public SlingshotShield AbilityLeft;

    
    public PlayerTeleport AbilityRight;

    
    [FormerlySerializedAs("ArmoredMode")] public AbilityPlayerLumenShift LumenShift;

    
    public PlayerTeleport teleportAbility;
    public delegate void Teleport(TeleportDirectionEnum direction);
    public event Teleport UsedTeleport;
    
    public PlayerAbilityInterface AbilityScan;
    public delegate void AbilityScanHandler();
    public event AbilityScanHandler UsedScanAbility;
    
    public delegate void SpentHandler(float amount);
    public event SpentHandler SpentLife;
    
    
    public TeleportFX TFX;

    public bool teleportActive = false;

    public PlayerTeleportInterface playerTeleport;

    public GearSystem gearSystem;
    public IGearMovementApp GearMovementApp;
    
    [Header("[Anima Ability FX]")]
    
    public AnimaEngineFX animaFX;
    
    //FusionCore Variables
    public List<FusionCore> fusionCoreInventory = new List<FusionCore>();

    //ColorBombWave
    public GameObject colorBombWave;

    [Header("[CAMERA]")] 
    public float[] cameraOffsetDistance = new float[4] {1f, 0.8f, 0.56f, 100f};
    public float cameraOffsetOverride = 0;
    public Transform hoverCollider;

    //	[HideInInspector]
    public int fusionCoreInventoryCapacity;
    public int fusionDampenedCapacity;

    [Header("[MOVEMENT VARIABLES]")] 
    public float[] fusionCoreSpeedTable = new float[4] {5f, 10f, 15f, 30f};
    public const float BASESPEED = 5;
    public float speedFusionApplied = 0;
    public float speedMod = 0;
    public float speedOverride = 0;
    public bool isSpeedOverwritten = false;
    public bool isMovementOverwritten = false;

    [Header("[LANE SWITCHING]")]
    //NO LONGER NEEDED TODO: REMOVE WHEN RIPPING OUT OLD ENEMY STATE BASED MOVEMENT
    public int shipLaneCurrent;

    [Header("[FREE MOVEMENT]")]
    
    public float speedFinal;
    public bool isBumperCollided = false;
    public bool isBumperBottom = false;
    public bool isBumperTop = false;
    public bool isGrinding = false;
    public float grindSpeed = 0.25f;
    
    [Header("[HUE MANAGEMENT]")] public GameObject hTriggerTop;
    public GameObject hTriggerBottom;

    //Ship Rotation Vars
    [Header("[ROTATION]")] 
    public int shipRotationLaneCurrent;
    public int shipRotationLaneDestination;
    public float tiltRotateDegree = 60f;
    public Quaternion defaultRot;
    public Quaternion targetRot;
    public GameObject sidePanelLeft;
    public GameObject sidePanelRight;
    

    [Header("[LIFE SYSTEM]")] 
    [SerializeField] private float batteryWarningThreshold = 10f;

    [Header("[ATTRIBUTE SYSTEM]")] 
    [OdinSerialize] private AttributeController attributeController;
    [OdinSerialize] public AttributeController AttributeController
    {
        get => attributeController;
        set => attributeController = value;
    }
    
    //IBoostable Variables
    [Header("[BOOST]")] public bool forceBoost = true;

    public int boostDefenseColorIndex;

    public int[] boostDefenseCoresAtActivation = new int[3];
    

    [Header("[WARPGATE]")] public WarpFX WFX;
    public GameObject warpClones;
    public GameObject warpCloneTop;
    public GameObject warpCloneBottom;
    public GameObject warpCamera;
    public float warpAttuntementSpeed = 5f;
    public float warpFrictionLeft = 24f;
    public float warpFrictionRight = 24f;
    public float warpCharge = 0f;
    public float warpChargeSpeed = 1f;
    public float warpSuccess = 50f;
    public float positionOffset = 8f;
    
    
    private Vector3 velocityCache;

    [Header("[COLLISION]")] public bool isInvincible = false;
    private int tempPlayerCoreCount;
    private int tempHostileCoreAmount;


    public bool isInventoryDampened = false;
    
    [Header("FEEDBACKS")] 
    public MMFeedbacks CantActivateAbility;
    public MMFeedbacks LifeLow;
    
    #region INITIALIZATION

    private void Awake()
    {
        base.CommonInitialization();
        InitPlayerShip();
    }

    protected void Start()
    {
        
    }

    void OnEnable()
    {
        ResetPlayerShip();
        ResetPlayerCamera();
        ResetPlayerMovement();
    }
    public void InitPlayerShip()
    {
        //Assign Self to GameManager
        GameManager.Instance.playerShip = this;
        
        //Add LocalClock to TimeControl Script
        //GameManager.Instance.GetComponentInChildren<TimeControl>().playerClock = gameObject.GetComponent<LocalClock>();

        //CHECK DEPENDENCIES
        if (gearSystem == null)
        {
            Debug.LogError("No Gear System Attached to PlayerShip. Please attach to enable functionality");
            return;
        }
        
        if (GearMovementApp == null)
        {
            Debug.LogError("No Gear Movement App Attached to PlayerShip. Please attach to enable functionality");
            return;
        }

        if (_clock == null)
        {
            _clock = GameManager.Instance.playerClock;
        }

        if (PIC == null)
        {
            Debug.LogError("No Player Input Controller Attached to PlayerShip. Please attach to enable functionality");
            return;
        }
        
        if(attributeController == null) {
            Debug.LogError("No Attribute Controller Attached to PlayerShip. Please attach to enable functionality");
            return;
        } 
        
        if(rB == null) {
            Debug.LogError("No Rigidbody Attached to PlayerShip. Please attach to enable functionality");
            return;
        }  
        
        
        //Init PlayerManagementTools
        if (!PMT)
        {
            PMT = Instantiate(PlayerManagementTools, Vector3.zero, Quaternion.identity);
            PMT.transform.SetParent(GameManager.Instance.globalSpawnedObjectsHolder.transform, false);
            //Init Hover Collider
            //hoverCollider = GameObject.Find("HoverCollider").transform;
        }
        
        //Move These Checks Somewhere else TODO: Decouple These Ability FX scripts from the PlayerShip Component. 
        if (!sFX)
            sFX = gameObject.GetComponent<ShieldFX>();
        
        
        ResetPlayerShip();
        ResetPlayerCamera();
        ResetPlayerMovement();
        // //Init Player Ship Game Object
        // if (gameObject.activeSelf == false) {
        //     gameObject.SetActive(true);
        // }
    }

    private void ResetPlayerShip()
    {
        //Reset Attribute System
        attributeController.InitAttributeSystem();
        
        //Reset MESH RENDERS
        shipRender.SetActive (true);
 
        //Reset Shield
        shieldModel.gameObject.SetActive (true);
        shipBody.gameObject.SetActive(true);
        StartCoroutine(shieldFixedAbility.RechargeShieldAbility());

        //Reset Warp FX
        WFX.warpClones.SetActive(false);
        WFX.warpClones.SetActive(false);
        warpCharge = 0f;
        WFX.warpRip.SetActive(false);
        
        //Reset Color System
        //TODO DECIDE ON SOURCE OF COLOR INDEX
        CSM.ChromaShift(GameColor.Grey); //Grey
        CoC.CSM.ChromaShift(GameColor.Grey); //Grey

        //Reset Player Bumpers
        isBumperBottom = false;
        isBumperBottom = false;
        isBumperCollided = false;
        PMT.SetActive(true);
        
        //Reset Anima Engine
        animaFX.EndAnima();
        //animaFX.ui.GetComponentInChildren<EnergyBar>().valueCurrent = 0;
        
        //Reset Teleport
        teleportActive = false;
        GetComponent<PlayerTeleport>().isLockedOut = false;
        GetComponent<PlayerTeleport>().CoolDownTime = 0;
        //GetComponent<PlayerTeleport>().eBar.valueCurrent = 0;
        teleportAbility.isLockedOut = false;
        teleportAbility.CoolDownTime = 0;
        //teleportAbility.eBar.valueCurrent = 0;
        
        //Reset Player Movement
        GearMovementApp.OverrideSpeed = false;
        PIC.EnableAllControls();
    }

    public void ResetPlayerMovement()
    {
        Debug.Log("Reset Player Movement");
        //Init Player Movement
        rB.velocity = Vector3.zero;
        isSpeedOverwritten = false;
        speedOverride = 0;
        fusionCoreSpeedTable [0] = 24f;
        fusionCoreSpeedTable [1] = 28f;
        fusionCoreSpeedTable [2] = 28f;
        fusionCoreSpeedTable [3] = 32f;

        //Init Player Bumpers
        isBumperBottom = false;
        isBumperTop = false;
        isBumperCollided = false;
        GetComponent<Collider>().enabled = true;
    }

    public void ResetPlayerCamera()
    {
        GameManager.Instance.mainCamera.RemoveAllCameraTargets();
        GameManager.Instance.mainCamera.AddCameraTarget(transform,1f,1f,0, new Vector2(GameManager.Instance.CameraStandardOffset,0));
        GameManager.Instance.mainCamera.FollowHorizontal = true;
        GameManager.Instance.mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -66f);
    }
    
    private void InitStartingCores() {
        
        /*if (GameManager.Instance.currentLevelDataObject) {
            foreach (int coreID in GameManager.Instance.currentSceneDataObject.startingFusionCores) {
                CoC.CoreInventory.Insert (0, new FusionCore (coreID));
            }
        }*/
        CoC.CSM.CoreInventory[0].IsActive = true;
        CoC.CSM.CoreInventory[1].IsActive = true;
        CoC.CSM.CoreInventory[2].IsActive = true;
        //Debug.Log("Init Starting Cores.");
    }
    #endregion

    

    private void OnDisable()
    {
        PMT.SetActive(false);
        PIC.DisableAllControls();
    }

    #region UPDATE LOOPS
    

    protected void FixedUpdate()
    {
        
        if (
            GameStateManager.Instance.CurrentState.StateType == GameStateType.GameSequence
            || GameStateManager.Instance.CurrentState.StateType == GameStateType.Gameplay
            || GameStateManager.Instance.CurrentState.StateType == GameStateType.ZoneComplete
        )
        {

            GearMovementApp.Move();


            CleanYAxis();
            CleanZAxis();
            //ClampPlayerBoundry();
            CheckBatteryLevels();
        }

    }
    #endregion

    public void CheckBatteryLevels()
    {
        //Debug.Log("PlayerShip Battery Level: " + LS.CurrentHP);
        if (LS.CurrentHP <= batteryWarningThreshold)
        {
            if(LifeLow.IsPlaying == false)
                LifeLow.PlayFeedbacks();
        }
        else
        {
            if(LifeLow.IsPlaying == true)
                LifeLow.StopFeedbacks();
        }
    }


    public float FindSpeedModifiers()
    {
        float modSpeed = 1;

        if (isGrinding)
        {
            modSpeed *= grindSpeed;
        }

        return modSpeed;
    }

    #region DoMoveUp

    public void DoMoveUp()
    {
        
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                GearMovementApp.ShipMoveDown = false;
                GearMovementApp.ShipMoveUp = true;
                break;

            default:
                //Do Nothing
                break;
        }
    }

    #endregion

    #region DoMoveDown

    public void DoMoveDown()
    {
        
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                GearMovementApp.ShipMoveUp = false;
                GearMovementApp.ShipMoveDown = true;
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }

    #endregion


    #region DoMoveLeft

    public void DoMoveLeft()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                GearMovementApp.ShipMoveRight = false;
                GearMovementApp.ShipMoveLeft = true;
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoMoveLeftRelease()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                //Debug.Log("DoMoveLeftRelease");
                GearMovementApp.ShipMoveLeft = false;
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }

    #endregion

    #region DoMoveRight

    public void DoMoveRight()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                GearMovementApp.ShipMoveRight = true;
                GearMovementApp.ShipMoveLeft = false;
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoMoveRightRelease()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                //Debug.Log("DoMoveRightRelease");
                GearMovementApp.ShipMoveRight = false;
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }

    #endregion

    #region DoDPad

    public void DoDPadUp()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoDPadDown()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoDPadLeft()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                LumenShift.DoAbilityPrimary();
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoDPadRight()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                LumenShift.ExitAbilityPrimary();
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }

    #endregion
    
    #region DoDFaceButtons

    public void DoFaceButtonX()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoFaceButtonCircle()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoFaceButtonSquare()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoFaceButtonTriangle()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                
                break;

            //case GameState.ActiveATG:
            //		//WarpGate.instance.ChargeGate ();
            //	break;

            default:
                //Do Nothing
                break;
        }
    }

    #endregion
    
    #region ShieldAbilities
    
    public void DoShieldAbilityPrimary()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                Debug.Log("Shield Ability Primary Activated [PLEASE HANDLE]");
                shieldFixedAbility.DoAbilityPrimary();
                break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoShieldAbilitySecondary()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                Debug.Log("Shield Ability Secondary Activated [PLEASE HANDLE]");
                break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoShieldAbilityRelease()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                shieldFixedAbility.ExitAbilityPrimary();
                break;

            default:
                //Do Nothing
                break;
        }
    }
    #endregion
    
    #region DoActiveAbilities
    
    public void DoActiveAbilityLeftPrimary()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                Debug.Log("Active Ability Left Primary Activated [PLEASE HANDLE]");
                AbilityLeft.DoAbilityPrimary();
                
                break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoActiveAbilityLeftSecondary()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                Debug.Log("Active Ability Left Secondary Activated [PLEASE HANDLE]");
                AbilityLeft.DoAbilitySecondary();
                break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoActiveAbilityRightPrimary()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                Debug.Log("Active Ability Right Primary Activated [PLEASE HANDLE]"); 
                AbilityRight.DoAbilityPrimary();
                break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoActiveAbilityRightSecondary()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                Debug.Log("Active Ability Right Secondary Activated [PLEASE HANDLE]");
                AbilityRight.DoAbilitySecondary();
                break;

            default:
                //Do Nothing
                break;
        }
    }

    #endregion


    public void DoLeftTriggerPull()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                GearMovementApp.ShipMoveBrake = true;
                break;

            default:
                //Do Nothing
                break;
        }
    }
    
    public void DoLeftTriggerRelease()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            case GameStateType.Gameplay:
                GearMovementApp.ShipMoveBrake = false;
                break;

            default:
                //Do Nothing
                break;
        }
    }

    public void DoRightTriggerPull()
    {
        switch (GameStateManager.Instance.CurrentState.StateType)
        {
            default:
                //Do Nothing
                break;
        }
    }


    #region CleanZAxis

    public void CleanZAxis()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    #endregion

    public void CleanYAxis()
    {
        if (isBumperCollided)
        {
            if (isBumperTop)
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.02f, 0);
            if (isBumperBottom)
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, 0);
        }
    }

    public void CleanUp()
    {
        GearMovementApp.OverrideSpeed = true;
        GearMovementApp.SetSpeedOverride(0,0);
    }
}