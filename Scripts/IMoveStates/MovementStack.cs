using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
#endif
using System.IO;
using ChromaShift.Scripts.Enemy;
using UnityEngine.UIElements;

[System.Serializable]
public class MovementStack : SerializedScriptableObject
{
#if UNITY_EDITOR
    [FoldoutGroup("MOVEMENT STACK")]
    [BoxGroup("MOVEMENT STACK/BASE INFO")]
    [GUIColor(0, 1, 0, 1)]
    [Button(ButtonSizes.Gigantic)]
    private void SaveStack()
    {
        OnSaveClick();
    }
#endif
    [FoldoutGroup("MOVEMENT STACK")]
    [BoxGroup("MOVEMENT STACK/BASE INFO")]
    [DisplayAsString(false)]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public string ID;

    [BoxGroup("MOVEMENT STACK/BASE INFO")]
    [Title("Description: Please describe the movement below for better understanding", bold: true)]
    [HideLabel]
    [MultiLineProperty(3)]
    [Required]
    [GUIColor(0.6f, 0.5f, 0, 1f)]
    public string Description;

    [FoldoutGroup("MOVEMENT STACK")] [BoxGroup("MOVEMENT STACK/BASE INFO")]
    public SpawnPosition spawnReference;

    [BoxGroup("MOVEMENT STACK/BASE INFO")] [GUIColor(1, 0, 0.75f, 1)] [CustomValueDrawer("CustomBossButton")]
    public bool bossStack = false;
    
    [BoxGroup("MOVEMENT STACK/BASE INFO")] [GUIColor(0, 0.75f, 1, 1)] [CustomValueDrawer("CustomLoopButton")]
    public bool loopStack;
    
    

    [BoxGroup("MOVEMENT STACK/LOOP")] [ShowIf("loopStack")]
    public int loopLimit = -1;

    [BoxGroup("MOVEMENT STACK/LOOP")] [ShowIf("loopStack")] [ShowOnly]
    public int currentLoopIndex = 0;

    [FoldoutGroup("MOVEMENT STACK")] [ListDrawerSettings(NumberOfItemsPerPage = 10), LabelText("MOVEMENT BLOCKS")]
    public MoveBlock[] MoveStack;

    private Dictionary<MovementType, string> moveTypeCode = new Dictionary<MovementType, string>();
    private Dictionary<MovementExitType, string> exitTypeCode = new Dictionary<MovementExitType, string>();
    private Dictionary<SpawnPosition, string> spawnReferenceCode = new Dictionary<SpawnPosition, string>();
    public MovementStack()
    {
        //Spawn Position Dictionary

        spawnReferenceCode.Add(SpawnPosition.Left, "L");

        spawnReferenceCode.Add(SpawnPosition.Right, "R");

        spawnReferenceCode.Add(SpawnPosition.Top, "T");

        spawnReferenceCode.Add(SpawnPosition.Bottom, "B");

        spawnReferenceCode.Add(SpawnPosition.Background, "BG");

        spawnReferenceCode.Add(SpawnPosition.Foreground, "FG");


        //MovementType Dictionary
        moveTypeCode.Add(MovementType.Stationary, "ST");
        moveTypeCode.Add(MovementType.Hover, "HV");
        moveTypeCode.Add(MovementType.SwitchLane, "SL");

        moveTypeCode.Add(MovementType.MoveToPlayer, "MTP");
        moveTypeCode.Add(MovementType.Wave, "WV");

        moveTypeCode.Add(MovementType.MoveHorizontal, "MH");
        moveTypeCode.Add(MovementType.CrossLane, "CL");

        moveTypeCode.Add(MovementType.HoverSwitch, "HS");
        moveTypeCode.Add(MovementType.MoveSwitch, "MS");

        moveTypeCode.Add(MovementType.MagMove, "MM");
        moveTypeCode.Add(MovementType.PlayerAbsorb, "PA");
        moveTypeCode.Add(MovementType.Flip, "FP");
        moveTypeCode.Add(MovementType.HunterMine, "HUNT");
        moveTypeCode.Add(MovementType.Dash, "DH");
        moveTypeCode.Add(MovementType.Rotate, "RT");

        //Exit Type Dictionary
        exitTypeCode.Add(MovementExitType.DistanceToObject, "DTO");
        exitTypeCode.Add(MovementExitType.DistanceFromObject, "DFO");

        exitTypeCode.Add(MovementExitType.Time, "TM");
        exitTypeCode.Add(MovementExitType.CollisionWithObject, "CWO");

        exitTypeCode.Add(MovementExitType.CameraEnter, "CE");
        exitTypeCode.Add(MovementExitType.CameraExit, "CX");

        exitTypeCode.Add(MovementExitType.Null, "NL");
        exitTypeCode.Add(MovementExitType.PlayerTriggerCollision, "PTC");

        exitTypeCode.Add(MovementExitType.PlayerDistanceFromObject, "PDFO");
        exitTypeCode.Add(MovementExitType.PlayerDistanceToObject, "PDTO");

        exitTypeCode.Add(MovementExitType.PlayerDistanceFromSelf, "PDFS");
        exitTypeCode.Add(MovementExitType.PlayerDistanceToSelf, "PDTS");
        exitTypeCode.Add(MovementExitType.Special, "SPEC");
    }


#if UNITY_EDITOR
    private bool CustomLoopButton(bool value, GUIContent label)
    {
        return GUILayout.Toggle(this.loopStack, label, GUI.skin.button, GUILayout.Height(50));
    }
    
    private bool CustomBossButton(bool value, GUIContent label)
    {
        return GUILayout.Toggle(this.bossStack, label, GUI.skin.button, GUILayout.Height(50));
    }

    private void OnSaveClick()
    {
        string newStackName = "";

        if (bossStack)
            newStackName = "BOSS_";

        if (loopStack)
            newStackName += "Loop_";
            
        
//@TODO try to use thing to set the spawn collider, the spawnReference
        newStackName = newStackName + MoveStack.Length + "_" + spawnReferenceCode[spawnReference];

        foreach (MoveBlock block in MoveStack)
        {
            newStackName += "_[" + moveTypeCode[block.movementType] + "-" + exitTypeCode[block.exitType] +
                           "]";
            if (block.useAttack)
            {
                newStackName += "_{A}";
                if (block.attackDelay > 0)
                    newStackName += "{" + block.attackDelay + "}";
            }
        }

        //Debug.Log(Application.dataPath + "/ChromaShift/Resources/MovementStacks/" + name + ".asset" + "  ||||  " + Application.dataPath + "/ChromaShift/Resources/MovementStacks/" + newStackName + ".asset");

        AssetDatabase.RenameAsset("Assets/ChromaShift/Resources/MovementStacks/" + name + ".asset",
            newStackName); //"Assets/ChromaShift/Resources/MovementStacks/" + newStackName + ".asset");

        ID = newStackName;

        AssetDatabase.Refresh();
    }


#endif
}

[System.Serializable]
public class MoveBlock
{
    [FoldoutGroup("MOVE BLOCK")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    [BoxGroup("MOVE BLOCK/MOVEMENT TYPE", centerLabel: true)]
    [EnumToggleButtons, HideLabel]
    public MovementType movementType;

    public bool isMovementSet;

    [GUIColor(0.8f, 0.4f, 0.3f, 1f)]
    [BoxGroup("MOVE BLOCK/EXIT TYPE", centerLabel: true)]
    [EnumToggleButtons, HideLabel]
    public MovementExitType exitType;

    [BoxGroup("MOVE BLOCK/Speed", centerLabel: true)]
    public float baseSpeed = 0;

    [BoxGroup("MOVE BLOCK/Speed", centerLabel: true)]
    public float moveVertSpeed = 0.2f;
    
    [BoxGroup("MOVE BLOCK/Speed", centerLabel: true)]
    public float distance;
    

    /*[ShowIf("ShowExitTypeDistanceLabel")*/[BoxGroup("MOVE BLOCK/Influencers", centerLabel: true)]
    public Transform triggerObject;

    /*[ShowIf("ShowExitTypeCameraLabel")]*/ [BoxGroup("MOVE BLOCK/Influencers", centerLabel: true)]
    public Camera cameraTriggerObject;

    /*[ShowIf("ShowExitTypeDistanceLabel")]*/ [BoxGroup("MOVE BLOCK/Influencers", centerLabel: true)]
    public float objectActivationRadius;

    [ShowIf("ShowExitTypeCameraLabel")] [BoxGroup("MOVE BLOCK/Influencers", centerLabel: true)]
    public float cameraActivationDistance;

    [ShowIf("exitType", MovementExitType.Time)] [BoxGroup("MOVE BLOCK/Influencers", centerLabel: true)]
    public float timeLimit;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowInInspector, ShowOnly]
    public float StateTimer;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowInInspector, ShowOnly]
    public float timeLimitCalc = 0;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowInInspector, ShowOnly]
    public float distanceToActivator;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowInInspector, ShowOnly]
    public float playerDistanceToActivator;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowInInspector, ShowOnly]
    public float distanceFromCamera;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowInInspector, ShowOnly]
    public bool visibleToCamera;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowInInspector, ShowOnly]
    private bool isCurrentlyActive;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowOnly]
    public float speedMod = 1f;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowOnly]
    public bool isSpeedOverwritten = false;

    [BoxGroup("MOVE BLOCK/STATUS", centerLabel: true)] [ShowOnly]
    public float speedOverride = 0;

    [BoxGroup("MOVE BLOCK/MOVE HORIZONTAL", centerLabel: true)] [ShowIf("ShowHorizontalLabel")]
    public bool horizontalMoveRight = false;

    [BoxGroup("MOVE BLOCK/LANE SWITCH", centerLabel: true)] [ShowIf("ShowDestinationLabel")]
    public int destinationLane;

    [BoxGroup("MOVE BLOCK/LANE SWITCH", centerLabel: true)] [ShowIf("ShowDestinationLabel")]
    public bool switchToPlayerlane;

    [BoxGroup("MOVE BLOCK/CROSS LANE", centerLabel: true)]
    [ShowIf("movementType", MovementType.CrossLane)]
    [Wrap(0f, 360f)]
    public float crossLaneAngle = 30f;

    [BoxGroup("MOVE BLOCK/CROSS LANE", centerLabel: true)] [ShowIf("movementType", MovementType.CrossLane)]
    public bool crossLaneRotate = true;

    [BoxGroup("MOVE BLOCK/CROSS LANE", centerLabel: true)] [ShowIf("movementType", MovementType.CrossLane)]
    public bool randomizeCrossLaneAngle = false;

    [BoxGroup("MOVE BLOCK/HOVER", centerLabel: true)] [ShowIf("ShowHoverLabel")]
    public float hoverDuration;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public bool isAttracted;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public bool isRepelled;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public float magStrength;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public float magDistance;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)]
    public float alignSpeed;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public bool magXRestricted;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public bool magYRestricted;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public bool magZRestricted;

    [BoxGroup("MOVE BLOCK/MAG MOVE", centerLabel: true)] [ShowIf("movementType", MovementType.MagMove)]
    public float magFactor;

    [BoxGroup("MOVE BLOCK/WAVE", centerLabel: true)] [ShowIf("movementType", MovementType.Wave)]
    public float waveHeight;

    [BoxGroup("MOVE BLOCK/WAVE", centerLabel: true)] [ShowIf("movementType", MovementType.Wave)]
    public float waveOffset;

    [BoxGroup("MOVE BLOCK/HOVER", true)] [ShowIf("IsHoverType")]
    public bool isBehindPlayer;

    [BoxGroup("MOVE BLOCK/ATTACK", true)]
    public bool useAttack;
    
    [BoxGroup("MOVE BLOCK/ATTACK", true)]
    public int attackSequence;
    
    [BoxGroup("MOVE BLOCK/ATTACK", true)]
    public float attackDelay;
    
    [BoxGroup("MOVE BLOCK/ATTACK", true)]
    public float targetPrediction;

    [BoxGroup("MOVE BLOCK/ATTACK", true)]
    public float targetDetectRange;

    [BoxGroup("MOVE BLOCK/ATTACK", true)]
    public AnimationCurve attackEase;
    [BoxGroup("MOVE BLOCK/WAYPOINT", centerLabel: true)] [ShowIf("movementType", MovementType.Waypoint)]
    public Vector2 waypointGroup;
    
#if UNITY_EDITOR
    private bool IsHoverType()
    {
        return movementType == MovementType.Hover || movementType == MovementType.HoverSwitch;
    }
#endif

    private bool ShowDestinationLabel
    {
        get
        {
            return movementType == MovementType.MoveSwitch || movementType == MovementType.HoverSwitch ||
                   movementType == MovementType.SwitchLane;
        }
    }

    private bool ShowHoverLabel
    {
        get { return movementType == MovementType.Hover || movementType == MovementType.HoverSwitch; }
    }

    private bool ShowHorizontalLabel
    {
        get { return movementType == MovementType.MoveHorizontal || movementType == MovementType.MoveSwitch; }
    }

    private bool ShowExitTypeDistanceLabel
    {
        get { return exitType == MovementExitType.DistanceFromObject || exitType == MovementExitType.DistanceToObject; }
    }

    private bool ShowExitTypeCameraLabel
    {
        get { return exitType == MovementExitType.CameraEnter || exitType == MovementExitType.CameraExit; }
    }
}