using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Player;
using CS_Audio;
using Rewired.ControllerExtensions;
using UnityEngine.UI;

public class 
    PlayerInputController : MonoBehaviour
{
    private static PlayerInputController _instance;
    
    
    public bool colorShiftControlEnabled = false;
    public bool debugControlEnabled = true;
    public bool[] controlCache = new bool[7];
    public static PlayerInputController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = UnityEngine.Object.FindObjectOfType(typeof(InputSystem)) as PlayerInputController;

                if (_instance == null)
                {
                    GameObject go = new GameObject("_PlayerInputController");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<PlayerInputController>();
                }
            }

            return _instance;
        }
    }
    
    [SerializeField] private bool horizontalMovementControlEnabled;
    [SerializeField] private bool verticalMovementControlEnabled;
    [SerializeField] private bool leftTriggerControlEnabled;
    [SerializeField] private bool rightTriggerControlEnabled;
    [SerializeField] private bool leftShoulderControlEnabled;
    [SerializeField] private bool rightShoulderControlEnabled;
    [SerializeField] private bool dPadUpControlEnabled;
    [SerializeField] private bool dPadDownControlEnabled;
    [SerializeField] private bool dPadLeftControlEnabled;
    [SerializeField] private bool dPadRightControlEnabled;
    [SerializeField] private bool faceButtonControlEnabled;
    [SerializeField] private bool rightStickControlEnabled;


    private void Awake()
    {

        controlCache[0] = false;
        controlCache[1] = false;
        controlCache[2] = false;
        controlCache[3] = false;
        controlCache[4] = false;
        controlCache[5] = false;
        controlCache[6] = false;
    }
    
    public void DisableAllControls()
    {
        DisableHorizontalControl();
        DisableVerticalControl();
        DisableRightStickControl();
        DisableLeftTriggerControl();
        DisableLeftShoulderControl();
        DisableRightTriggerControl();
        DisableRightShoulderControl();
        DisableFaceButtonsControl();
        DisableDPadUpControl();
        DisableDPadDownControl();
        DisableDPadLeftControl();
        DisableDPadRightControl();
    }

    public void EnableAllControls()
    {
        EnableHorizontalControl();
        EnableVerticalControl();
        EnableRightStickControl();
        EnableLeftTriggerControl();
        EnableLeftShoulderControl();
        EnableRightTriggerControl();
        EnableRightShoulderControl();
        EnableFaceButtonsControl();
        EnableDPadUpControl();
        EnableDPadDownControl();
        EnableDPadLeftControl();
        EnableDPadRightControl();
        EnableDebug();
    }

    public void EnableHorizontalControl()
    {
        if (horizontalMovementControlEnabled)
        {
            return;
        }
        InputSystem.OnMoveLeft += GameManager.Instance.playerShip.DoMoveLeft;
        InputSystem.OnMoveLeftRelease += GameManager.Instance.playerShip.DoMoveLeftRelease;
        InputSystem.OnMoveRight += GameManager.Instance.playerShip.DoMoveRight;
        InputSystem.OnMoveRightRelease += GameManager.Instance.playerShip.DoMoveRightRelease;
        InputSystem.OnDoublePressLeft += GameManager.Instance.playerShip.GearMovementApp.DoDoublePressLeft;
        InputSystem.OnDoublePressRight += GameManager.Instance.playerShip.GearMovementApp.DoDoublePressRight;

        horizontalMovementControlEnabled = true;
    }
    
    public void DisableHorizontalControl()
    {
        if (!horizontalMovementControlEnabled)
        {
            return;
        }
        InputSystem.OnMoveLeft -= GameManager.Instance.playerShip.DoMoveLeft;
        InputSystem.OnMoveLeftRelease -= GameManager.Instance.playerShip.DoMoveLeftRelease;
        InputSystem.OnMoveRight -= GameManager.Instance.playerShip.DoMoveRight;
        InputSystem.OnMoveRightRelease -= GameManager.Instance.playerShip.DoMoveRightRelease;
        InputSystem.OnDoublePressLeft -= GameManager.Instance.playerShip.GearMovementApp.DoDoublePressLeft;
        InputSystem.OnDoublePressRight += GameManager.Instance.playerShip.GearMovementApp.DoDoublePressRight;

        horizontalMovementControlEnabled = false;
    }
    
    public void EnableVerticalControl()
    {
        if (verticalMovementControlEnabled)
        {
            return;
        }
        InputSystem.OnMoveUp += GameManager.Instance.playerShip.DoMoveUp;
        InputSystem.OnMoveDown += GameManager.Instance.playerShip.DoMoveDown;
        InputSystem.OnDoublePressUp += GameManager.Instance.playerShip.GearMovementApp.DoBarrelRoll;
        InputSystem.OnDoublePressDown += GameManager.Instance.playerShip.GearMovementApp.DoBarrelRoll;
        verticalMovementControlEnabled = true;
    }
    
    public void DisableVerticalControl()
    {
        if (!verticalMovementControlEnabled)
        {
            return;
        }
        InputSystem.OnMoveUp -= GameManager.Instance.playerShip.DoMoveUp;
        InputSystem.OnMoveDown -= GameManager.Instance.playerShip.DoMoveDown;
        InputSystem.OnDoublePressUp -= GameManager.Instance.playerShip.GearMovementApp.DoBarrelRoll;
        InputSystem.OnDoublePressDown -= GameManager.Instance.playerShip.GearMovementApp.DoBarrelRoll;
        verticalMovementControlEnabled = false;
    }
    
    public void EnableRightStickControl()
    {
        if (rightStickControlEnabled)
        {
            return;
        }
        if (!InputSystem.Instance.IsRightStickEnabled)
            InputSystem.Instance.IsRightStickEnabled = true;
        
        rightStickControlEnabled = true;
    }
    
    public void DisableRightStickControl()
    {
        if (!rightStickControlEnabled)
        {
            return;
        }
        if (InputSystem.Instance.IsRightStickEnabled)
            InputSystem.Instance.IsRightStickEnabled = false;
        
        rightStickControlEnabled = false;
    }
    
   
    //Trigger Controls
    public void EnableLeftTriggerControl()
    {
        if (leftTriggerControlEnabled)
        {
            return;
        }
            
        InputSystem.OnGravBrake += GameManager.Instance.playerShip.GearMovementApp.DoGravBrake;
        InputSystem.OnTurnAround += GameManager.Instance.playerShip.GearMovementApp.DoFlip;
        InputSystem.OnGravBrakeRelease += GameManager.Instance.playerShip.GearMovementApp.DoGravBrakeRelease;

        leftTriggerControlEnabled = true;
    }

    public void DisableLeftTriggerControl()
    {
        if (leftTriggerControlEnabled == false)
        {
            return;
        }


        InputSystem.OnGravBrake -= GameManager.Instance.playerShip.GearMovementApp.DoGravBrake;
        InputSystem.OnTurnAround -= GameManager.Instance.playerShip.GearMovementApp.DoFlip;
        InputSystem.OnGravBrakeRelease -= GameManager.Instance.playerShip.GearMovementApp.DoGravBrakeRelease;
        
        leftTriggerControlEnabled = false;
    }

    public void EnableRightTriggerControl()
    {
        if (rightTriggerControlEnabled)
        {
            return;
        }

        InputSystem.OnAccelerator += GameManager.Instance.playerShip.GearMovementApp.DoAccelerator;
        InputSystem.OnAcceleratorRelease += GameManager.Instance.playerShip.GearMovementApp.DoAcceleratorRelease;


        rightTriggerControlEnabled = true;
    }

    public void DisableRightTriggerControl()
    {
        if (rightTriggerControlEnabled == false)
        {
            return;
        }

        InputSystem.OnAccelerator -= GameManager.Instance.playerShip.GearMovementApp.DoAccelerator;
        InputSystem.OnAcceleratorRelease -= GameManager.Instance.playerShip.GearMovementApp.DoAcceleratorRelease;

        
        rightTriggerControlEnabled = false;
    }
    
    //Shoulder Controls
    public void EnableLeftShoulderControl()
    {
        if (leftShoulderControlEnabled)
        {
            return;
        }
            
        InputSystem.OnActiveAbilityLeft += GameManager.Instance.playerShip.DoActiveAbilityLeftPrimary;
        //InputSystem.OnActiveAbilityLeft += GameManager.Instance.playerShip.DoActiveAbilityLeftSecondary;
        
        leftShoulderControlEnabled = true;
    }

    public void DisableLeftShoulderControl()
    {
        if (leftShoulderControlEnabled == false)
        {
            return;
        }

        InputSystem.OnActiveAbilityLeft -= GameManager.Instance.playerShip.DoActiveAbilityLeftPrimary;
        //InputSystem.OnActiveAbilityLeft -= GameManager.Instance.playerShip.DoActiveAbilityLeftSecondary;
        
        leftShoulderControlEnabled = false;
    }
    
    public void EnableRightShoulderControl()
    {
        if (rightShoulderControlEnabled)
        {
            return;
        }
            
        //InputSystem.OnActiveAbilityRight += GameManager.Instance.playerShip.DoActiveAbilityRightPrimary;
        //InputSystem.OnActiveAbilityRight += GameManager.Instance.playerShip.DoActiveAbilityRightSecondary;
        InputSystem.OnShieldShortPress += GameManager.Instance.playerShip.DoShieldAbilityPrimary;
        InputSystem.OnShieldLongPress += GameManager.Instance.playerShip.DoShieldAbilitySecondary;
        InputSystem.OnShieldRelease += GameManager.Instance.playerShip.DoShieldAbilityRelease;
        
        rightShoulderControlEnabled = true;
    }

    public void DisableRightShoulderControl()
    {
        if (rightShoulderControlEnabled == false)
        {
            return;
        }

        //InputSystem.OnActiveAbilityRight -= GameManager.Instance.playerShip.DoActiveAbilityRightPrimary;
        //InputSystem.OnActiveAbilityRight -= GameManager.Instance.playerShip.DoActiveAbilityRightSecondary;
        
        InputSystem.OnShieldShortPress -= GameManager.Instance.playerShip.DoShieldAbilityPrimary;
        InputSystem.OnShieldLongPress -= GameManager.Instance.playerShip.DoShieldAbilitySecondary;
        InputSystem.OnShieldRelease -= GameManager.Instance.playerShip.DoShieldAbilityRelease;
        rightShoulderControlEnabled = false;
    }
    
    
    //DPad Controls
   
    public void EnableDPadUpControl()
    {
        if (dPadUpControlEnabled)
        {
            return;
        }
            
        InputSystem.OnDPadUp += GameManager.Instance.playerShip.DoDPadUp;
        
        
        dPadUpControlEnabled = true;
    }

    public void DisableDPadUpControl()
    {
        if (dPadUpControlEnabled == false)
        {
            return;
        }

        InputSystem.OnDPadUp -= GameManager.Instance.playerShip.DoDPadUp;

        
        dPadUpControlEnabled = false;
    }
    
    public void EnableDPadDownControl()
    {
        if (dPadDownControlEnabled)
        {
            return;
        }
            
        InputSystem.OnDPadDown += GameManager.Instance.playerShip.DoDPadDown;

        dPadDownControlEnabled = true;
    }

    public void DisableDPadDownControl()
    {
        if (dPadDownControlEnabled == false)
        {
            return;
        }

        InputSystem.OnDPadDown -= GameManager.Instance.playerShip.DoDPadDown;
        
        dPadDownControlEnabled = false;
    }
    
    public void EnableDPadLeftControl()
    {
        if (dPadLeftControlEnabled)
        {
            return;
        }
            
        InputSystem.OnDPadLeft += GameManager.Instance.playerShip.DoDPadLeft;

        dPadLeftControlEnabled = true;
    }

    public void DisableDPadLeftControl()
    {
        if (dPadLeftControlEnabled == false)
        {
            return;
        }

        InputSystem.OnDPadLeft -= GameManager.Instance.playerShip.DoDPadLeft;

        dPadLeftControlEnabled = false;
    }
    
    public void EnableDPadRightControl()
    {
        if (dPadRightControlEnabled)
        {
            return;
        }
            
        InputSystem.OnDPadRight += GameManager.Instance.playerShip.DoDPadRight;

        dPadRightControlEnabled = true;
    }

    public void DisableDPadRightControl()
    {
        if (dPadRightControlEnabled == false)
        {
            return;
        }

        InputSystem.OnDPadRight -= GameManager.Instance.playerShip.DoDPadRight;

        dPadRightControlEnabled = false;
    }
    
    
    public void EnableFaceButtonsControl()
    {
        if (faceButtonControlEnabled)
        {
            return;
        }
            
        InputSystem.OnFaceButtonX += GameManager.Instance.playerShip.DoFaceButtonX;
        InputSystem.OnFaceButtonCircle += GameManager.Instance.playerShip.DoFaceButtonCircle;
        InputSystem.OnFaceButtonSquare += GameManager.Instance.playerShip.DoFaceButtonSquare;
        InputSystem.OnActiveAbilityRight += GameManager.Instance.playerShip.DoActiveAbilityRightPrimary;

        faceButtonControlEnabled = true;
    }

    public void DisableFaceButtonsControl()
    {
        if (faceButtonControlEnabled == false)
        {
            return;
        }

        InputSystem.OnFaceButtonX -= GameManager.Instance.playerShip.DoFaceButtonX;
        InputSystem.OnFaceButtonCircle -= GameManager.Instance.playerShip.DoFaceButtonCircle;
        InputSystem.OnFaceButtonSquare -= GameManager.Instance.playerShip.DoFaceButtonSquare;
        InputSystem.OnActiveAbilityRight -= GameManager.Instance.playerShip.DoActiveAbilityRightPrimary;

        faceButtonControlEnabled = false;
    }
    
    
    
    
    // ABILITIES

    public void EnableShieldAbility()
    {
        if (rightTriggerControlEnabled == true)
        {
            return;
        }
        
        InputSystem.OnShieldRelease += GameManager.Instance.playerShip.DoShieldAbilityRelease;
        rightTriggerControlEnabled = true;

    }

    public void DisableShieldAbility()
    {
        if (rightTriggerControlEnabled == false)
        {
            return;
        }
        
        InputSystem.OnShieldRelease -= GameManager.Instance.playerShip.DoShieldAbilityRelease;
        rightTriggerControlEnabled = false;

    }

    public void EnableColorShift()
    {
        if (colorShiftControlEnabled)
        {
            return;
        }

        InputSystem.OnColorShift +=
            GameManager.Instance.playerShip.gearSystem.colorSystem.ChangeColor;

        colorShiftControlEnabled = true;
    }

    public void DisableColorShift()
    {
        if (colorShiftControlEnabled == false)
        {
            return;
        }

        InputSystem.OnColorShift -=
            GameManager.Instance.playerShip.gearSystem.colorSystem.ChangeColor;

        colorShiftControlEnabled = false;
    }

    public void EnableDebug()
    {
        if (debugControlEnabled)
        {
            return;
        }

        /// @TODO Remove these once the FusionCoreInventoryManager is removed. They also don't work due to pass by value.
        // InputSystem.OnToggleRedCore += FusionCoreInventoryManager.Instance.DoToggleRedCore;
        // InputSystem.OnToggleGreenCore += FusionCoreInventoryManager.Instance.DoToggleGreenCore;
        // InputSystem.OnToggleBlueCore += FusionCoreInventoryManager.Instance.DoToggleBlueCore;
        // InputSystem.OnRemoveAllCores += FusionCoreInventoryManager.Instance.DoRemoveAllCores;
        // InputSystem.OnRemoveOneCore += FusionCoreInventoryManager.Instance.DoRemoveOneCore;

        debugControlEnabled = true;
    }

    public void DisableDebug()
    {
        if (debugControlEnabled == false)
        {
            return;
        }

        /*InputSystem.OnToggleRedCore -= FusionCoreInventoryManager.Instance.DoToggleRedCore(null);
        InputSystem.OnToggleGreenCore -= FusionCoreInventoryManager.Instance.DoToggleGreenCore();
        InputSystem.OnToggleBlueCore -= FusionCoreInventoryManager.Instance.DoToggleBlueCore();
        InputSystem.OnRemoveAllCores -= FusionCoreInventoryManager.Instance.DoRemoveAllCores();
        InputSystem.OnRemoveOneCore -= FusionCoreInventoryManager.Instance.DoRemoveOneCore();*/

        debugControlEnabled = false;
    }

    public void CachedDisableControl()
    {

        
        if (colorShiftControlEnabled == true)
        {
            controlCache[1] = true;
            DisableColorShift();
        }
        

        if (rightTriggerControlEnabled == true)
        {
            controlCache[3] = true;
            DisableShieldAbility();
        }
        

    }

    public void RestoreControl()
    {
        EnableAllControls();
    }

    public void EnableGodMode()
    {
        if (GameManager.Instance.playerShip.isInvincible)
        {
            GameManager.Instance.playerShip.isInvincible = false;
            AudioManager.Instance.PlayClipWrap(3, 10);
        }
        else
        {
            GameManager.Instance.playerShip.isInvincible = true;
            AudioManager.Instance.PlayClipWrap(3, 9);
        }
    }

    public void UpdateCheckPoint(bool add = true)
    {
        if (add)
        {
            GameManager.Instance.checkPointIndex++;
            
            if (GameManager.Instance.checkPointIndex > GameManager.Instance.checkPointObject.Count-1)
            {
                GameManager.Instance.checkPointIndex = GameManager.Instance.checkPointObject.Count-1;
            }

        }
        else
        {
            GameManager.Instance.checkPointIndex--;
            
            if (GameManager.Instance.checkPointIndex < 0)
            {
                GameManager.Instance.checkPointIndex = 0;
            }

        }
    }


    public void MoveShipToCheckpoint(int index)
    {
        if (GameManager.Instance.checkPointObject.Count > 0 && index < GameManager.Instance.checkPointObject.Count && GameManager.Instance.checkPointObject[index] != null) {
            GameManager.Instance.playerShip.transform.position = new Vector3(
                GameManager.Instance.checkPointObject[index].transform.position.x,
                GameManager.Instance.checkPointObject[index].transform.position.y, 0);
        }
    }
    
    
    }