using System;
using UnityEngine;
using System.Collections.Generic;
using ChromaShift.Scripts.Enemy;
using Chronos;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class MovableObjectController : SerializedMonoBehaviour
{
    /*[BoxGroup("NEW MOVEMENT SYSTEM")] [GUIColor(0, 0.5f, 1f, 1)] [CustomValueDrawer("MovementStackButton")]
    public bool useNewMovementSystem;*/
    
    [BoxGroup("NEW MOVEMENT SYSTEM")] [GUIColor(0.45f, 0.45f, 1f, 1)] [CustomValueDrawer("BlockParamsButton")]
    public bool useBlockParams;
    
#if UNITY_EDITOR
    /*private bool MovementStackButton(bool value, GUIContent label)
    {
        return GUILayout.Toggle(this.useNewMovementSystem, label, GUI.skin.button, GUILayout.Height(50));
    }*/
    
    private bool BlockParamsButton(bool value, GUIContent label)
    {
        return GUILayout.Toggle(useBlockParams, label, GUI.skin.button, GUILayout.Height(45));
    }
#endif

    [BoxGroup("NEW MOVEMENT SYSTEM")]
    public MovementStack moveStackReference;

    [BoxGroup("NEW MOVEMENT SYSTEM")] [InlineEditor] [HideLabel] [ShowOnly][InlineButton("InitStack")]
    public MovementStack moveList;

    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)] public List<float> baseSpeeds;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<float> verticalSpeeds;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<float> hoverDurations;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<float> crossLaneAngles;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<bool> crossLaneRotates;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<float> magFactors;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<int> laneSwitchDestinationLanes;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<bool> playerLaneSwitches;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)] public List<Transform> triggerObjects;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)] public List<float> objectActivationRadii;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<Camera> cameraTriggerObjects;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<float> cameraActivationDistances;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<float> timeLimits;
    [BoxGroup("NEW MOVEMENT SYSTEM")][HideIf("useBlockParams", false)]  public List<bool> horizontalMoveRights;
    [BoxGroup("NEW MOVEMENT SYSTEM")] public List<bool> isBehindPlayer;

    //public MovementList movementList;
    public MovementActivationType activationType;

    public GameObject movableObject;
    public float hoverDuration;

    [ShowOnly] public int startLane;
    public int switchDestinationLane;
    public bool switchToPlayerlane;
    public float crossLaneAngle = 30f;
    public bool crossLaneRotate = true;
    public bool randomizeCrossLaneAngle = false;
    public float moveVertSpeed = 0.2f;
    public float boostModifier = 2f;
    public bool horizontalMoveRight = false;
    public float distanceToPlayerActivation;
    public float distanceToPlayer;
    public float TimedSwitchNumber;
    public bool isAttracted;
    public bool isRepelled;
    public float magStrength;
    public float magDistance;
    public float alignSpeed;
    public bool magXRestricted;
    public bool magYRestricted;
    public bool magZRestricted;
    public float magFactor;
    public bool moveToPlayerRotation = true;
    public Vector2 waypointGroup;

    [Header("[WAVE MOVEMENT]")] public float waveHeight;
    public float waveOffset;

    [Header("[MOVEMENT VARIABLES]")]
    //Base Speed should be assigned in the editor
    public float baseSpeed = 0;

    //Affect by Outside Objects such as Nebulae. This is a multiplier default should stay 1.
    [ShowOnly] public float speedMod = 1f;

    //This will set a speed that will not be affected by anything
    public float speedOverride = 0;
    [ShowOnly] public bool isSpeedOverwritten = false;

    [Header("[NEBULA]")] [ShowOnly] public bool isInsideNebula = false;
    [FormerlySerializedAs("statusEffect")] [FormerlySerializedAs("statusEffectGroup")] [FormerlySerializedAs("nebulaType")] [ShowOnly] public StatusEffectType statusEffectType;
    public bool isInventoryDampened = false;

    [Header("[LANE SWITCHING]")] public Vector3 destinationLanePosition;
    public float laneDistance;
    public float distanceFromEnemyToDestinationLane;
    [ShowOnly] public float fracJourney;

    [ShowOnly] public float objectTimeAlive;
    public Transform triggerCollisionObject;

    [Header("[Boss Movement Stacks]")] 
    public List<MovementStack> stackQueue;

    [Header("[Boss Health breakpoints]")]
    public List<int> healthBreakpoint;

    public delegate void HandleNewMovementStack(MovementStack movementStack);

    public void SetMovementStack(MovementStack movementStack)
    {
        moveList = movementStack;
    }

    public void SetMovementStackByReference(MovementStack reference)
    {
        MovementStack stack = (MovementStack) Instantiate(Resources.Load("MovementStacks/" + reference.ID,
                typeof(MovementStack)));
        moveList = stack;
    }
    
    public void Awake()
    {
        if (stackQueue.Count > 0)
        {
            useBlockParams = true;
            SetMovementStackByReference(stackQueue[0]);
        }
        else
            SetMovementStackByReference(moveStackReference);
    }

    public void Start()
    {
        var boss = GetComponent<Boss>();
        
        if (boss == null) return;
        boss.HealthChangeEvent += StackChangeByHealth;
    }

    public float FindSpeedModifiers()
    {
        return (isInsideNebula && statusEffectType == StatusEffectType.Slow) ? 0.5f : 1;
    }

    public void Update()
    {
        if (!triggerCollisionObject)
            triggerCollisionObject = GameObject.Find("HoverCollider").transform;
    }

#if UNITY_EDITOR
    private void InitStack()
    {
        if (!EditorUtility.DisplayDialog("Initialize Movement Stack on Object?",
            "Are you sure you want to reset the Movement Stack on " + name, "Initialize", "Cancel")) return;

        MovementStack currentStack =
            (MovementStack) Instantiate(Resources.Load("MovementStacks/" + moveStackReference.ID,
                typeof(MovementStack)));
        if (currentStack)
        {
            ClearStack();

            foreach (MoveBlock block in currentStack.MoveStack)
            {
                baseSpeeds.Add(block.baseSpeed);
                verticalSpeeds.Add(block.moveVertSpeed);
                hoverDurations.Add(block.hoverDuration);
                crossLaneAngles.Add(block.crossLaneAngle);
                crossLaneRotates.Add(block.crossLaneRotate);
                horizontalMoveRights.Add(block.horizontalMoveRight);
                magFactors.Add(block.magFactor);
                laneSwitchDestinationLanes.Add(block.destinationLane);
                playerLaneSwitches.Add(block.switchToPlayerlane);
                if (block.triggerObject != null)
                    triggerObjects.Add(block.triggerObject.transform);
                else
                    triggerObjects.Add(null);
                objectActivationRadii.Add(block.distanceToActivator);
                cameraTriggerObjects.Add(block.cameraTriggerObject);
                cameraActivationDistances.Add(block.cameraActivationDistance);
                timeLimits.Add(block.timeLimit);
                isBehindPlayer.Add(block.isBehindPlayer);
            }
        }
        else
        {
            throw new UnityException("Invalid movement stack. Is there even one?");
            Debug.LogError("Movement Stack is invalid.");
        }
    }

    private void ClearStack()
    {
        baseSpeeds.Clear();
        verticalSpeeds.Clear();
        hoverDurations.Clear();
        crossLaneAngles.Clear();
        crossLaneRotates.Clear();
        magFactors.Clear();
        laneSwitchDestinationLanes.Clear();
        playerLaneSwitches.Clear();
        triggerObjects.Clear();
        objectActivationRadii.Clear();
        cameraTriggerObjects.Clear();
        cameraActivationDistances.Clear();
        timeLimits.Clear();
        horizontalMoveRights.Clear();
        isBehindPlayer.Clear();
    }
#endif

    public void StackChangeByHealth(int currentHealth)
    {
        Debug.LogFormat("Checking from the Movable Object Controller on the Boss's new current health of {0}", currentHealth);

        if (currentHealth <= healthBreakpoint[0])
        {
            SetMovementStackByReference(stackQueue[0]);
        } else if (currentHealth <= healthBreakpoint[1])
        {
            SetMovementStackByReference(stackQueue[1]);
        } else if (currentHealth <= healthBreakpoint[2])
        {
            SetMovementStackByReference(stackQueue[2]);
        } else if (currentHealth <= healthBreakpoint[3])
        {
            SetMovementStackByReference(stackQueue[3]);
        }
    }
}