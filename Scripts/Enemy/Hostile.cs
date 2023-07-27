using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Enemy.WeaponSystems;
using ChromaShift.Scripts.Enemy;
using Chronos;
using DG.Tweening;
using CS_Audio;
using Language.Lua;
using QuickEngine.Extensions;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class Hostile : BaseBehaviour
{
    public MovableObjectController MoC;
    public Rigidbody rB;
    public FusionCoreEjector FCE;
    public string uniqueTileReferenceID;

    public AudioSource _audioSource;
    //	public AudioSource engineAudioSource;

    protected PlayerShip playerShip;

    protected ImpactType ImpactType;

    [Header("[SPECIAL FX]")] public List<GameObject> deathFX;
    private WeaponSystem _weaponSystem;
    public List<AudioClip> deathAudio;
    public bool isDestructable = true;
    public bool dangerZoneActivated = false;
    public bool isVirtual = false;
    public bool ableToThreatenPlayer = false;

    public List<GameObject> fusionCores = new List<GameObject>();

    [Header("[LANE SWITCHING]")] [ShowOnly]
    public int enemyLaneCurrent;

    [ShowOnly] public int enemyLaneDestination;
    [ShowOnly] public Vector3 destinationLanePosition;
    [ShowOnly] public float laneDistance;
    [ShowOnly] public float distanceFromEnemyToDestinationLane;
    [ShowOnly] public float fracJourney;

    public float tiltRotateDegree = 60f;
    public Quaternion defaultRot;
    public Quaternion targetRot;

    [ShowOnly] public bool isHovering;

    public IMoveState currentState;
    public List<IMoveState> newMoveStates = new List<IMoveState>();
    public IMoveState newCurrentState;
    public int currentBlockIndex;
    public float levelTimeBuffer;
    public bool currentMovementCompleted;

    [Header("[TARGETING]")] 
    public GameObject targetObject;
    public Vector3 targetPosition;
    public bool _targetPositionLocked;
    public float targetPrediction = 20f;
    
    //Distance Vars
    public float playerDistanceToActivator;
    public float distanceToActivator;
    public float playerDistanceFromSelf;

    [HideInInspector] public StationaryState stationaryState;
    [HideInInspector] public HoverState hoverState;
    [HideInInspector] public SwitchLaneState switchLaneState;
    [HideInInspector] public MoveToPlayerState moveToPlayerState;
    [HideInInspector] public WaveState waveState;
    [HideInInspector] public MoveHorizontalState moveHorizontalState;
    [HideInInspector] public CrossLaneState crossLaneState;
    [HideInInspector] public HoverSwitchState hoverSwitchState;
    [HideInInspector] public MoveSwitchState moveSwitchState;
    [HideInInspector] public MagMoveState magMoveState;
    [HideInInspector] public HunterMineState hunterMineState;
    [HideInInspector] public WaypointState waypointState;

    public GameObject enemyShipTrail;
    public bool isDead = false;
    public bool ejectCore = false;

    public delegate void HostileDestroyed();

    public static event HostileDestroyed OnHostileDestroyed = delegate { };

    protected Bounds theBounds;

    private SpawnPosition _spawnPosition;

    private int storedColorIndex;
    private int[] storedCoreInventory;
    private int storedCoreAmount;

    private Queue<IResourceSet> _resourceSets;

    private Boss _boss;
    

    protected virtual void Awake()
    {
//	    Debug.Log("CALLED AWAKE ON HOSTILE");

        if (!MoC)
        {
            MoC = GetComponent<MovableObjectController>();
        }
        

        if (!playerShip)
            playerShip = GameManager.Instance.playerShip;

        GameManager.Instance.cameraPlanes =
            GeometryUtility.CalculateFrustumPlanes(GameManager.Instance.mainCamera.GetComponent<Camera>());
        theBounds = gameObject.GetComponentInChildren<Collider>().bounds;

        if (!FCE)
        {
            FCE = GetComponent<FusionCoreEjector>();
        }

        if (!_weaponSystem)
        {
            _weaponSystem = gameObject.GetComponent<WeaponSystem>();
        }

        _boss = GetComponent<Boss>();
    }

    // Use this for initialization
    protected virtual void Start()
    {
        //Connects the color for the collision to the color that is rendered for the ship
        if (CSM != null && CoC != null)
        {
            CSM.ChromaShift(CoC.CSM.CurrentColor);
        }

        stationaryState = new StationaryState(this);
        hoverState = new HoverState(this);
        switchLaneState = new SwitchLaneState(this);
        moveToPlayerState = new MoveToPlayerState(this);
        waveState = new WaveState(this);
        moveHorizontalState = new MoveHorizontalState(this);
        crossLaneState = new CrossLaneState(this);
        hoverSwitchState = new HoverSwitchState(this);
        moveSwitchState = new MoveSwitchState(this);
        magMoveState = new MagMoveState(this);
        hunterMineState = new HunterMineState(this);
        waypointState = new WaypointState(this);

        if (MoC == null)
            return;

        //NEW MOVEMENT SYSTEM - MOVEMENT STACK
        IMoveState tempState;

        for (int i = 0; i < MoC.moveList.MoveStack.Length; i++)
        {
            if (MoC.moveList.MoveStack.Length == 0)
            {
                tempState = stationaryState;
            }

            switch (MoC.moveList.MoveStack[i].movementType)
            {
                case MovementType.Stationary:
                    tempState = stationaryState;
                    break;
                case MovementType.Hover:
                    tempState = hoverState;
                    break;
                case MovementType.SwitchLane:
                    tempState = switchLaneState;
                    break;
                case MovementType.MoveToPlayer:
                    tempState = moveToPlayerState;
                    break;
                case MovementType.Wave:
                    tempState = waveState;
                    break;
                case MovementType.MoveHorizontal:
                    tempState = moveHorizontalState;
                    break;
                case MovementType.CrossLane:
                    tempState = crossLaneState;
                    break;
                case MovementType.HoverSwitch:
                    tempState = hoverSwitchState;
                    break;
                case MovementType.MoveSwitch:
                    tempState = moveSwitchState;
                    break;
                case MovementType.MagMove:
                    tempState = magMoveState;
                    break;
                case MovementType.Flip:
                    tempState = new FlipState(gameObject.transform);
                    break;
                case MovementType.HunterMine:
                    tempState = hunterMineState;
                    break;
                case MovementType.Waypoint:
                    tempState = waypointState;
                    break;
                /*case MovementType.Dash:
                    tempState = DashState;
                    break;*/
                default:
                    tempState = stationaryState;
                    break;
            }

            newMoveStates.Add(tempState);
        }

        levelTimeBuffer = 5f;
        InitMoveBlock(0);
    }

    public void InitMoveBlock(int blockIndex)
    {
//	    Debug.Log("new block index: " + newBlockIndex);
        if (currentBlockIndex > MoC.moveList.MoveStack.Length - 1 && MoC.moveList.loopStack &&
            (MoC.moveList.currentLoopIndex < MoC.moveList.loopLimit || MoC.moveList.loopLimit == -1))
        {
            currentBlockIndex = 0;
            MoC.moveList.currentLoopIndex++;
        }

        GatherParams();

        if (blockIndex != 0)
            ableToThreatenPlayer = true;
        // @TODO trouble shoot this. It's annoying me.
        if (newMoveStates.Count <= currentBlockIndex && MoC.moveList.currentLoopIndex >= MoC.moveList.loopLimit)
        {
            // Could be a good place to raise an event that it ran out of moves.
            currentState = null;
            return;
        }

        currentState = newMoveStates[currentBlockIndex];
        // The below should just be a method the object being held by the
        // MoveStack to update it's time by passing in the time

        MoC.moveList.MoveStack[currentBlockIndex].timeLimitCalc =
            Timekeeper.instance.Clock("Enemy").time + _timeLimits;
        _isMoveSet = false;
        levelTimeBuffer = 0;

        StartCoroutine(InitAttackSequence());
    }

    private IEnumerator InitAttackSequence()
    {
        if (MoC.moveList.MoveStack[currentBlockIndex].useAttack)
        {
            yield return new WaitForSeconds(MoC.moveList.MoveStack[currentBlockIndex].attackDelay);

            StartCoroutine(_weaponSystem.ProcessSequence(MoC.moveList.MoveStack[currentBlockIndex].attackSequence));
        }
    }


    //Block Params
    private Transform _triggerObject;
    private bool _isMoveSet;
    private float _objectActivationRadius;
    private float _timeLimits;

    private void GatherParams()
    {
        if (MoC.moveList.MoveStack.Length > currentBlockIndex)
            _isMoveSet = MoC.moveList.MoveStack[currentBlockIndex].isMovementSet;
        
            _objectActivationRadius = MoC.moveList.MoveStack[currentBlockIndex].objectActivationRadius;
            _triggerObject = MoC.moveList.MoveStack[currentBlockIndex].triggerObject;
            _timeLimits = MoC.moveList.MoveStack[currentBlockIndex].timeLimit;
        
    }

    private void Update()
    {
        currentState?.Update();
    }

    private int lastBlockIndex = -1;
    
    public virtual void FixedUpdate()
    {
        if (!targetObject)
            targetObject = GameManager.Instance.playerShip.gameObject;
        if(!_targetPositionLocked && (targetObject != null))
            targetPosition = targetObject.transform.position;
        
        destinationLanePosition = new Vector3(transform.position.x,
            LaneManager.Instance.laneArray[enemyLaneDestination].y, transform.position.z);
        laneDistance = Vector3.Distance(LaneManager.Instance.laneArray[enemyLaneDestination],
            LaneManager.Instance.laneArray[enemyLaneCurrent]);
        distanceFromEnemyToDestinationLane = Vector3.Distance(destinationLanePosition, transform.position);
        fracJourney = distanceFromEnemyToDestinationLane / laneDistance;

        //Debug.Log ("Lane Distance: " + distanceFromEnemyToDestinationLane);
        //DETERMINE WHICH LANE THE ENEMY IS CURRENTLY IN
        if (enemyLaneCurrent != enemyLaneDestination)
        {
            //Debug.Log (name + "is not in the intended lane");
            //SwitchLane ();
        }
        else
        {
            SetEnemyLane();
        }

        if (distanceFromEnemyToDestinationLane < 0.5f)
            SetCurrentLane();

        if (GameManager.Instance.gameState != GameStateType.Gameplay)
        {
            return;
        }

        UseNewMovementSystem();
    }

    private void UseNewMovementSystem()
    {
        if (currentState == null)
        {
            return;
        }

//		Debug.Log("current block index: " + currentBlockIndex);
//		Debug.Log("Move Stack length: " + MoC.moveList.MoveStack.Length);
        if (currentBlockIndex < MoC.triggerObjects.Count)
        {
            if (_triggerObject != null)
            {
            }
        }

        if (!MoC.moveList)
            return;

        if (currentBlockIndex < MoC.moveList.MoveStack.Length)
        {
//		        Debug.Log("exitType: " + MoC.moveList.MoveStack[currentBlockIndex].exitType);
            //NEW MOVEMENT SYSTEM - MOVEMENT STACK
            switch (MoC.moveList.MoveStack[currentBlockIndex].exitType)
            {
                case MovementExitType.Time:

                    var tmpTime = Timekeeper.instance.Clock("Enemy").time;
                    if (tmpTime >=
                        MoC.moveList.MoveStack[currentBlockIndex].timeLimitCalc)
                    {
                        //Debug.Log(name + " | ExitTypeIs: Time");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.CollisionWithObject:
//				        Debug.Log("ExitTypeIs: Collision With Object");
                    break;
                case MovementExitType.DistanceFromObject:
                    distanceToActivator = Vector3.Distance(transform.position,
                        _triggerObject.position);
                    if (distanceToActivator > _objectActivationRadius)
                    {
//					        Debug.Log("ExitTypeIs: Distance From Object");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.DistanceToObject:
                    distanceToActivator = Vector3.Distance(transform.position,
                        _triggerObject.position);
                    if (distanceToActivator < _objectActivationRadius)
                    {
//					        Debug.Log("ExitTypeIs: Distance To Object");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.CameraEnter:
                    //Debug.Log("Checking to see if object is visible to camera");
                    GameManager.Instance.cameraPlanes =
                        GeometryUtility.CalculateFrustumPlanes(GameManager.Instance.mainCamera.GameCamera);
                    MoC.moveList.MoveStack[currentBlockIndex].visibleToCamera =
                        GeometryUtility.TestPlanesAABB(GameManager.Instance.cameraPlanes, theBounds);
                    MoC.moveList.MoveStack[currentBlockIndex].distanceFromCamera = Vector3.Distance(transform.position,
                        GameManager.Instance.mainCamera.transform.position);
                    if (MoC.moveList.MoveStack[currentBlockIndex].visibleToCamera)
                    {
                        //Debug.Log("ExitTypeIs: Camera Enter");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.CameraExit:
                    //Debug.Log("Checking to see if object is no longer visible to camera");
                    GameManager.Instance.cameraPlanes =
                        GeometryUtility.CalculateFrustumPlanes(GameManager.Instance.mainCamera.GameCamera);
                    MoC.moveList.MoveStack[currentBlockIndex].visibleToCamera =
                        GeometryUtility.TestPlanesAABB(GameManager.Instance.cameraPlanes, theBounds);
                    MoC.moveList.MoveStack[currentBlockIndex].distanceFromCamera = Vector3.Distance(transform.position,
                        GameManager.Instance.mainCamera.transform.position);
                    if (!MoC.moveList.MoveStack[currentBlockIndex].visibleToCamera)
                    {
                        //Debug.Log("ExitTypeIs: Camera Exit");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.PlayerDistanceFromObject:
                    //Debug.Log("Checking to see if player is far enough away from activator object");
                    playerDistanceToActivator = Vector3.Distance(GameManager.Instance.playerShip.transform.position,
                        _triggerObject.position);
                    if (playerDistanceToActivator > _objectActivationRadius)
                    {
                        //Debug.Log("ExitTypeIs: Distance From Object");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.PlayerDistanceToObject:
                    var transformToCheck = (_triggerObject == null)
                        ? transform
                        : _triggerObject;

                    //Debug.Log("Checking to see if player is close enough to activator object");
                    playerDistanceToActivator = Vector3.Distance(GameManager.Instance.playerShip.transform.position,
                        transformToCheck.position);
                    if (playerDistanceToActivator < _objectActivationRadius)
                    {
                        // Debug.Log("ExitTypeIs: Distance To Object");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.PlayerDistanceFromSelf:
                    //Debug.Log("Checking to see if player is far enough away from me");
                    playerDistanceFromSelf = Vector3.Distance(GameManager.Instance.playerShip.transform.position,
                        transform.position);
                    if (playerDistanceFromSelf > _objectActivationRadius)
                    {
                        //Debug.Log("ExitTypeIs: Distance From Self");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.PlayerDistanceToSelf:
                    //Debug.Log("Checking to see if player is close enough to me");
                    playerDistanceFromSelf = Vector3.Distance(GameManager.Instance.playerShip.transform.position,
                        transform.position);
                    if (playerDistanceFromSelf < _objectActivationRadius)
                    {
                        //Debug.Log("ExitTypeIs: Distance To Self");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                case MovementExitType.Special:
                    if (currentMovementCompleted)
                    {
                        //Debug.Log("ExitTypeIs: Special");
                        currentBlockIndex++;
                        InitMoveBlock(currentBlockIndex);
                    }

                    break;
                    
            }
        }

        currentMovementCompleted = false;
        
        if (currentState == null)
        {
            lastBlockIndex = -1;
            return;
        }

        currentState.UpdateState();
    }

    private void SetEnemyLane()
    {
        if (transform.position.y < 40f)
            enemyLaneCurrent = 8;
        else if (transform.position.y >= 40f && transform.position.y < 48f)
            enemyLaneCurrent = 7;
        else if (transform.position.y >= 48f && transform.position.y < 56f)
            enemyLaneCurrent = 6;
        else if (transform.position.y >= 56f && transform.position.y < 64f)
            enemyLaneCurrent = 5;
        else if (transform.position.y >= 64f)
            enemyLaneCurrent = 4;
    }

    public void SetCurrentLane()
    {
        enemyLaneCurrent = enemyLaneDestination;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
       //REMOVED DANGERZONE FEATURE
    }
}