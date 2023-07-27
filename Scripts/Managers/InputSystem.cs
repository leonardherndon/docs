using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Managers;
using ChromaShift.Scripts.Player;
using ChromaShift.Settings;
using CS_Audio;
using Huenity;
using Rewired;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// Input system.
/// The sole purpose of this class is to take input from user's device and send events to the game. 
/// </summary>
public class InputSystem : MonoBehaviour
{
    private static InputSystem _instance;

    public delegate void OnInputMoveUp();

    public static event OnInputMoveUp OnMoveUp = delegate { };

    public delegate void OnInputMoveDown();

    public static event OnInputMoveDown OnMoveDown = delegate { };
    
    public delegate void OnInputDoubleUp(bool isUp);

    public static event OnInputDoubleUp OnDoublePressUp = delegate { };

    public delegate void OnInputDoubleDown(bool isUp);

    public static event OnInputDoubleDown OnDoublePressDown = delegate { };
    

    public delegate void OnInputMoveLeft();

    public static event OnInputMoveLeft OnMoveLeft = delegate { };

    public delegate void OnInputMoveRight();

    public static event OnInputMoveRight OnMoveRight = delegate { };

    public delegate void OnInputMoveLeftRelease();

    public static event OnInputMoveLeftRelease OnMoveLeftRelease = delegate { };

    public delegate void OnInputMoveRightRelease();

    public static event OnInputMoveRightRelease OnMoveRightRelease = delegate { };
    
    public delegate void OnInputDoubleLeft();

    public static event OnInputDoubleLeft OnDoublePressLeft = delegate { };

    public delegate void OnInputDoubleRight();

    public static event OnInputDoubleRight OnDoublePressRight = delegate { };


    //Active Ability

    public delegate void OnInputActiveAbilityLeft();

    public static event OnInputActiveAbilityLeft OnActiveAbilityLeft = delegate { };
    
    public delegate void OnInputActiveAbilityRight();

    public static event OnInputActiveAbilityRight OnActiveAbilityRight = delegate { };
    
    public delegate void OnInputGravBrake();

    public static event OnInputGravBrake OnGravBrake = delegate { };
    
    public delegate void OnInputTurnAround();

    public static event OnInputTurnAround OnTurnAround = delegate { };
    
    public delegate void OnInputGravBrakeRelease();

    public static event OnInputGravBrakeRelease OnGravBrakeRelease = delegate { };
    
    
    public delegate void OnInputAccelerator();

    public static event OnInputAccelerator OnAccelerator = delegate { };
    
    public delegate void OnInputAcceleratorRelease();

    public static event OnInputAcceleratorRelease OnAcceleratorRelease = delegate { };
    
    
    public delegate void OnInputShieldShortPress();

    public static event OnInputShieldShortPress OnShieldShortPress = delegate { };
    
    public delegate void OnInputShieldLongPress();

    public static event OnInputShieldLongPress OnShieldLongPress = delegate { };

    public delegate void OnInputShieldRelease();

    public static event OnInputShieldRelease OnShieldRelease = delegate { };
    
    public delegate void OnInputDeSaturate();

    public static event OnInputDeSaturate OnDeSaturate = delegate { };
    

    public delegate void OnInputContextButtonPress();

    public static event OnInputContextButtonPress OnContextButtonPress = delegate { };
    
    public delegate void OnInputLeftTriggerPull();

    public static event OnInputLeftTriggerPull OnLeftTriggerPull = delegate { };

    public static event OnInputLeftTriggerRelease OnLeftTriggerRelease = delegate { };

    public delegate void OnInputLeftTriggerRelease();
    
    public delegate void OnInputRightTriggerPull();

    public static event OnInputRightTriggerPull OnRightTriggerPull = delegate { };
    
    public static event OnInputRightTriggerRelease OnRightTriggerRelease = delegate { };

    public delegate void OnInputRightTriggerRelease();

    public delegate void OnInputGearShiftPositive();

    public static event OnInputGearShiftPositive OnGearShiftPositive = delegate { };

    public delegate void OnInputGearShiftNegative();

    public static event OnInputGearShiftNegative OnGearShiftNegative = delegate { };

    public bool IsRightStickEnabled = true;

    public delegate void OnInputColorShift(GameColor colorIndex);

    public static event OnInputColorShift OnColorShift = delegate { };
    
    // D-PAD
    
    public delegate void OnInputOnDPadUp();

    public static event OnInputOnDPadUp OnDPadUp = delegate { };
    
    public delegate void OnInputOnDPadDown();

    public static event OnInputOnDPadDown OnDPadDown = delegate { };
    
    public delegate void OnInputOnDPadLeft();

    public static event OnInputOnDPadLeft OnDPadLeft = delegate { };
    
    public delegate void OnInputOnDPadRight();

    public static event OnInputOnDPadRight OnDPadRight = delegate { };
    
    // FACE BUTTONS
    
    public delegate void OnInputFaceButtonX();

    public static event OnInputFaceButtonX OnFaceButtonX = delegate { };
    
    public delegate void OnInputFaceButtonCircle();

    public static event OnInputFaceButtonCircle OnFaceButtonCircle = delegate { };

    
    public delegate void OnInputFaceButtonSquare();

    public static event OnInputFaceButtonSquare OnFaceButtonSquare = delegate { };

    
    public delegate void OnInputFaceButtonTriangle();

    public static event OnInputFaceButtonTriangle OnFaceButtonTriangle = delegate { };

    
    

    // Debug Events
    public delegate void OnDebugPlayerToggleRedCore(FusionCore[] coreInventory);

    public static event OnDebugPlayerToggleRedCore OnToggleRedCore = delegate { };

    public delegate void OnDebugPlayerToggleGreenCore(FusionCore[] coreInventory);

    public static event OnDebugPlayerToggleGreenCore OnToggleGreenCore = delegate { };

    public delegate void OnDebugPlayerToggleBlueCore(FusionCore[] coreInventory);

    public static event OnDebugPlayerToggleBlueCore OnToggleBlueCore = delegate { };

    public delegate void OnDebugPlayerRemoveAllCores(FusionCore[] coreInventory);

    public static event OnDebugPlayerRemoveAllCores OnRemoveAllCores = delegate { };

    public delegate void OnDebugPlayerRemoveOneCore(FusionCore[] coreInventory);

    public static event OnDebugPlayerRemoveOneCore OnRemoveOneCore = delegate { };

    public delegate void OnDebugPlayerAddPhase();

    public static event OnDebugPlayerAddPhase OnAddPhase = delegate { };

    public delegate void OnDebugPlayerAddQuantumKey();

    public static event OnDebugPlayerAddQuantumKey OnAddQuantumKey = delegate { };

    public delegate void OnDebugEnemyAddRedCore();

    public static event OnDebugEnemyAddRedCore OnEnemyAddRedCore = delegate { };

    public delegate void OnDebugEnemyAddGreenCore();

    public static event OnDebugEnemyAddGreenCore OnEnemyAddGreenCore = delegate { };

    public delegate void OnDebugEnemyAddBlueCore();

    public static event OnDebugEnemyAddBlueCore OnEnemyAddBlueCore = delegate { };

    public delegate void OnDebugEnemyAddRandomCore();

    public static event OnDebugEnemyAddRandomCore OnEnemyAddRandomCore = delegate { };

    public delegate void OnInputPressPKey();

    public static event OnInputPressPKey OnPauseGame = delegate { };

    public delegate void OnInputPressZKey();

    public static event OnInputPressZKey OnShowControlPanel = delegate { };

    public delegate void OnInputPressOKey();

    public static event OnInputPressOKey OnATGStateSwitch = delegate { };

    public static event PlayerShip.Teleport AttemptToTeleport;


    private bool isShiftUpPressed = false;
    private bool isShiftDownPressed = false;

    private bool isActiveAbilityPressed = false;
    private bool _isplayerShipNull;

    //Singleton pattern implementation
    public static InputSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(InputSystem)) as InputSystem;

                if (_instance == null)
                {
                    GameObject go = new GameObject("_inputsystem");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<InputSystem>();
                }
            }

            return _instance;
        }
    }

    void FixedUpdate()
    {
        if (GameStateManager.Instance.CurrentState.StateType == GameStateType.Gameplay)
        {
            CheckForInput();
        }
    }
    
    #region CheckForInput()

    /// <summary>
    /// The legacy input control system.
    /// </summary>
    private void CheckForInput()
    {
        #region Game Controller

        if (GameManager.Instance.playerShip == null)
            return;
        
        //MOVE VERTICAL
        #region MoveVertical
        
        if (GameManager.Instance.playerController.GetAxis("MoveVertical") > 0.1f)
        {
            OnMoveUp();
        }
        else
        {
            if (!GameManager.Instance.playerShip.isBumperCollided &&
                !GameManager.Instance.playerShip.isMovementOverwritten)
                GameManager.Instance.playerShip.GearMovementApp.ShipMoveUp = false;
        }

        if (GameManager.Instance.playerController.GetAxis("MoveVertical") < -0.1f)
        {
            OnMoveDown();
        }
        else
        {
            if (!GameManager.Instance.playerShip.isBumperCollided &&
                !GameManager.Instance.playerShip.isMovementOverwritten)
                GameManager.Instance.playerShip.GearMovementApp.ShipMoveDown = false;
        }
        
        if (GameManager.Instance.playerController.GetButtonDoublePressDown("MoveVertical"))
        {
            OnDoublePressUp(true);
            Debug.Log("Double Press Up");
        }
        
        if (GameManager.Instance.playerController.GetNegativeButtonDoublePressDown("MoveVertical"))
        {
            OnDoublePressDown(false);
            Debug.Log("Double Press Down");
        }
        
        /*if (GameManager.Instance.playerController.GetAxis("MoveVertical") > 0.5f && GameManager.Instance.playerController.GetAxisTimeActive("MoveVertical") <= 0.4f && GameManager.Instance.playerController.GetButtonSinglePressHold("Accelerator"))
        {
            OnDoublePressUp(true);
            Debug.Log("Accelerated Barrel Up");
        }
        
        if (GameManager.Instance.playerController.GetAxis("MoveVertical") < -0.5f && GameManager.Instance.playerController.GetAxisTimeActive("MoveVertical") <= 0.4f && GameManager.Instance.playerController.GetButtonSinglePressHold("Accelerator"))
        {
            OnDoublePressDown(false);
            Debug.Log("Accelerated Barrel Down");
        }*/
        
        #endregion
        
        //MoveHorizontal
        #region MoveHorizontal

        if (GameManager.Instance.playerController.GetNegativeButtonSinglePressHold("MoveHorizontal"))
        {
           
            OnMoveLeft();
        }
        
        if (GameManager.Instance.playerController.GetNegativeButtonSinglePressUp("MoveHorizontal"))
        {
           
            OnMoveLeftRelease();
        }

        if (GameManager.Instance.playerController.GetButtonSinglePressHold("MoveHorizontal"))
        {
            OnMoveRight();
        }
        
        if (GameManager.Instance.playerController.GetButtonSinglePressUp("MoveHorizontal"))
        {
            OnMoveRightRelease();
        }
        
        if (GameManager.Instance.playerController.GetNegativeButtonDoublePressDown("MoveHorizontal"))
        {
            OnDoublePressLeft();
            //Debug.Log("Double Press Left");
        }
        
        if (GameManager.Instance.playerController.GetButtonDoublePressDown("MoveHorizontal"))
        {
            OnDoublePressRight();
            //Debug.Log("Double Press Right");
        }
        
        
        


        #endregion
        
        #region D-PAD
        
        if (GameManager.Instance.playerController.GetButtonDown("D-PadUp"))
        {
            OnDPadUp();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("D-PadDown"))
        {
            OnDPadDown();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("D-PadLeft"))
        {
            OnDPadLeft();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("D-PadRight"))
        {
            OnDPadRight();
        }
        
        #endregion
        
        #region FaceButtons
        
        if (GameManager.Instance.playerController.GetButtonDown("FaceButtonX"))
        {
            OnFaceButtonX();
        }
        
        if (GameManager.Instance.playerController.GetButtonRepeating("FaceButtonX"))
        {
            OnFaceButtonX();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("FaceButtonCircle"))
        {
            OnFaceButtonCircle();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("FaceButtonSquare"))
        {
            OnFaceButtonSquare();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("FaceButtonTriangle"))
        {
            OnFaceButtonTriangle();
        }
        
        #endregion
        
        
        #region ActiveAbility

        // The string "ActiveAbilityLeft" is also being used in the Scripts/SlingshotShield.cs
        if (GameManager.Instance.playerController.GetButtonDown("ActiveAbilityLeft"))
        {
            OnActiveAbilityLeft();
        }
        
        if (GameManager.Instance.playerController.GetButtonUp("ActiveAbilityRight"))
        {
            OnActiveAbilityRight();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("GravBrake"))
        {
            OnGravBrake();
        }
        if (GameManager.Instance.playerController.GetButtonDoublePressDown("TurnAround"))
        {
            OnTurnAround();
        }
        else if (GameManager.Instance.playerController.GetButtonUp("GravBrake"))
        {
            OnGravBrakeRelease();
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("Accelerator"))
        {
            Debug.Log("InputSystem: OnAccelerator");
            OnAccelerator();
        }
        else if (GameManager.Instance.playerController.GetButtonUp("Accelerator"))
        {
            Debug.Log("InputSystem: OnAccelerator Release");
            OnAcceleratorRelease();
        }
        

        if (GameManager.Instance.playerController.GetButtonTimedPressUp("Shield", 0, 0.7f))
        {
            OnShieldShortPress();
        }
        else if (GameManager.Instance.playerController.GetButtonTimedPressDown("Shield",  0.7f))
        {
            OnShieldLongPress();
        } 
        else if (GameManager.Instance.playerController.GetButtonTimedPressUp("Shield",  0.7f)) 
        {
            OnShieldRelease();
        }
        
        #endregion
        
        
        
        //COLOR SHIFT
        #region ColorShift
        
        if (GameManager.Instance.playerController.GetButtonDown("Color_RED"))
        {
            OnColorShift(GameColor.Red);
        }
        if (GameManager.Instance.playerController.GetButtonDown("Color_GREEN"))
        {
            OnColorShift(GameColor.Green);
        }
        if (GameManager.Instance.playerController.GetButtonDown("Color_BLUE"))
        {
            OnColorShift(GameColor.Blue);
        }
        if (GameManager.Instance.playerController.GetButtonDown("Color_YELLOW"))
        {
            OnColorShift(GameColor.Yellow);
        }
        if (GameManager.Instance.playerController.GetButtonDown("Color_PURPLE"))
        {
            OnColorShift(GameColor.Purple);
        }
        if (GameManager.Instance.playerController.GetButtonDown("Color_CYAN"))
        {
            OnColorShift(GameColor.Cyan);
        }
        if (GameManager.Instance.playerController.GetButtonLongPressDown("Color_GREY"))
        {
            OnColorShift(GameColor.Grey);
        }

        #endregion
        
        //GEAR SHIFT
        #region GearShift
        
        if (GameManager.Instance.playerController.GetAxis("GearShift") > 0.3f)
        {
            if (!isShiftUpPressed)
            {
                OnGearShiftPositive();
                isShiftUpPressed = true;
            }
        }
        else
        {
            isShiftUpPressed = false;
        }
        
        
        if (GameManager.Instance.playerController.GetAxis("GearShift") < -0.3f)
        {
            if (!isShiftDownPressed)
            {
                OnGearShiftNegative();
                isShiftDownPressed = true;
            }
        }
        else
        {
            isShiftDownPressed = false;
        }
        
        #endregion

        //WARP ZONE
        #region WarpZone

        if (GameManager.Instance.playerController.GetAxis("LeftTrigger") > 0.5f)
            OnLeftTriggerPull();
        else
            OnLeftTriggerRelease();

        if (GameManager.Instance.playerController.GetAxis("RightTrigger") > 0.5f)
            OnRightTriggerPull();
        else
            OnRightTriggerRelease();

        #endregion
        
        //SHIELD 
        #region Shield
        if (GameManager.Instance.playerController.GetButtonDown("Shield")) {
            OnShieldShortPress();
        }

        if (GameManager.Instance.playerController.GetButtonUp("Shield")) {
            OnShieldRelease();
        }
        #endregion

        //CONTEXT ACTION
        #region ContextAction
        
        if (GameManager.Instance.playerController.GetButtonDown("ContextAction"))
        {
            OnContextButtonPress();
        }

        #endregion
        
        //UI
        #region UI

        if (GameManager.Instance.playerController.GetButtonDown("PauseGame"))
        {
            OnPauseGame();
        }

        if (GameManager.Instance.playerController.GetButtonDown("Cancel"))
        {
            if (GameStateManager.Instance.CurrentState.StateType != GameStateType.Gameplay)
            {
                GameUIManager.Instance.BackOutMenu();
            }
        }
        
        if (GameManager.Instance.playerController.GetButtonDown("UIPopUp"))
        {
            OnShowControlPanel();
        }
        
        #endregion
        
        #endregion

        #region Desktop Keyboard

        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnShowControlPanel();
        }


        //Player Debug Keys
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            PlayerInputController.Instance.MoveShipToCheckpoint(GameManager.Instance.checkPointIndex);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.Instance.checkPointIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.Instance.checkPointIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.Instance.checkPointIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameManager.Instance.checkPointIndex = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GameManager.Instance.checkPointIndex = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GameManager.Instance.checkPointIndex = 5;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            GameManager.Instance.checkPointIndex = 6;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameManager.Instance.playerShip.CoC.GetWrecked();
            //HueManager.Instance.NullifyLights();
        }
        
        #endregion
    }

    #endregion CheckForInput()
    
    public Vector2 GetRightStickInput()
    {
        //Debug.Log("GetRightStickInput");
        if (!IsRightStickEnabled)
        {
            return Vector2.zero;
        }
        
        float rightStickX = GameManager.Instance.playerController.GetAxis("ColorX");
        float rightStickY = GameManager.Instance.playerController.GetAxis("ColorY");
        
        return new Vector2(rightStickX,rightStickY);
    }
    

}