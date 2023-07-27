using UnityEngine;
using System.Collections;
using DG.Tweening;


/// <summary>
/// SCRIPT NEEDS A VISUAL AID COMPONENT TO SHOW PLAYER THAT THEY HAVE LOST CONTROL OF SHIP
/// </summary>
/// 
public enum controlFeature {shield,phase,collision,fullControl,coreDump,batteryCharge,nullControl}

public class PlayerControlTrigger : MonoBehaviour {
	
	public PlayerInputController PIC;

	// -- GAMEPLAY -- //
	
	[Header ("[DEBUG]")]
	[SerializeField] private bool adjustDebug;
	[SerializeField] private bool debugToggle;
	
	[Header ("[PLAY AREA]")]
	[SerializeField] private bool adjustPlayAreaToggle;
	public int[] newPlayArea = new int[2];

	[Header("[CAMERA ZOOM")] 
	[SerializeField] private bool adjustCameraZoomToggle;
	public float cameraZoomDistance = 65;
	public float cameraZoomSpeed = 1f;
	
	[Header("[SPEED]")] 
	[Tooltip("Z is used for Drag Factor. DO NOT set to 0")]
	[SerializeField] private bool adjustSpeedOverride;
	[SerializeField] private bool overrideSpeedToggle;
	[SerializeField] private Vector3 overrideSpeed;

	[Header ("[CORE DUMP]")]
	[SerializeField] private bool coreDumpToggle;
	[SerializeField] private bool dumpAllCores;
	public int dumpCount;

	[Header ("[CORE CAPACITY]")]
	[SerializeField] private bool coreCapacityToggle;
	public int coreCapacityCount = 3;

	[Header ("[BATTERY CHARGE]")]
	[SerializeField] private bool batteryChargeToggle;
	public float batteryLevel;
	
	[Header ("[INVINCIBLE]")]
	[SerializeField] private bool adjustInvincible;
	[SerializeField] private bool invincibleToggle;
	
	[Header ("[LANE]")]
	[SerializeField] private bool adjustLane;
	[SerializeField] private bool laneToggle;
	public int laneDestination = 6;

	[Header ("[COLOR SHIFT]")]
	[SerializeField] private bool adjustColorShift;
	[SerializeField] private bool colorShiftToggle;

	// -- CONTROLS -- //
	
	[Header ("[HORIZONTAL CONTROL]")]
	[SerializeField] private bool adjustHorizontalControl;
	[SerializeField] private bool horizontalControlToggle;
	
	[Header ("[VERTICAL CONTROL]")]
	[SerializeField] private bool adjustVerticalControl;
	[SerializeField] private bool verticalControlToggle;
	
	[Header ("[RIGHT STICK CONTROL]")]
	[SerializeField] private bool adjustRightStickControl;
	[SerializeField] private bool rightStickControlToggle;
	
	[Header ("[FACE BUTTONS CONTROL]")]
	[SerializeField] private bool adjustFaceButtons;
	[SerializeField] private bool faceButtonsToggle;
	
	[Header ("[D-PAD UP CONTROL]")]
	[SerializeField] private bool adjustDPadUp;
	[SerializeField] private bool dPadUpToggle;
	
	[Header ("[D-PAD DOWN CONTROL]")]
	[SerializeField] private bool adjustDPadDown;
	[SerializeField] private bool dPadDownToggle;
	
	[Header ("[D-PAD LEFT CONTROL]")]
	[SerializeField] private bool adjustDPadLeft;
	[SerializeField] private bool dPadLeftToggle;
	
	[Header ("[D-PAD RIGHT CONTROL]")]
	[SerializeField] private bool adjustDPadRight;
	[SerializeField] private bool dPadRightToggle;

	[Header ("[LEFT SHOULDER BUTTON CONTROL]")]
	[SerializeField] private bool adjustLeftShoulderButton;
	[SerializeField] private bool leftShoulderButtonToggle;
	
	[Header ("[RIGHT SHOULDER BUTTON CONTROL]")]
	[SerializeField] private bool adjustRightShoulderButton;
	[SerializeField] private bool rightShoulderButtonToggle;
	
	[Header ("[LEFT TRIGGER CONTROL]")]
	[SerializeField] private bool adjustLeftTrigger;
	[SerializeField] private bool leftTriggerToggle;
	
	[Header ("[RIGHT TRIGGER CONTROL]")]
	[SerializeField] private bool adjustRightTrigger;
	[SerializeField] private bool rightTriggerToggle;
	
	
	// -- MOVEMENT -- //
	
	/*[Header ("[BASE MOVEMENT]")]
	[SerializeField] private bool adjustBaseMovement;
	[SerializeField] private bool baseMovementToggle;
	
	[Header ("[BARREL ROLL]")]
	[SerializeField] private bool adjustBarrelRoll;
	[SerializeField] private bool barrelRollManeuverToggle;
	
	[Header ("[BOOST MANEUVER]")]
	[SerializeField] private bool adjustBoostManeuver;
	[SerializeField] private bool boostManeuverToggle;
	

	
	[Header ("[GRAVITY BRAKE]")]
	[SerializeField] private bool adjustGravBrakeManeuver;
	[SerializeField] private bool gravBrakeManeuverToggle;
	
	[Header ("[SHIELD]")]
	[SerializeField] private bool adjustShield;
	[SerializeField] private bool shieldToggle;
	
	[Header ("[TELEPORT]")]
	[SerializeField] private bool adjustTeleport;
	[SerializeField] private bool teleportToggle;*/


	
	void Awake() {
		
		if (!PIC) {
			PIC = GameObject.Find ("ScriptManager").GetComponent<PlayerInputController>();
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.name != "PlayerShip") return;
		Debug.Log ("Player Control Trigger | Collision Detected with " + other.name);
		
		// -- CONTROLS -- //
		
		if (adjustHorizontalControl) {
			if (horizontalControlToggle)
				PIC.EnableHorizontalControl();
			else
				PIC.DisableHorizontalControl();
		}
		
		if (adjustVerticalControl) {
			if (verticalControlToggle)
				PIC.EnableVerticalControl();
			else
				PIC.DisableVerticalControl();
		}
		
		if (adjustRightStickControl) {
			if (rightStickControlToggle)
				PIC.EnableRightStickControl();
			else
				PIC.DisableRightStickControl();
		}
		
		if (adjustLeftTrigger) {
			if (leftTriggerToggle)
				PIC.EnableLeftTriggerControl();
			else
				PIC.DisableLeftTriggerControl();
		}

		if (adjustRightTrigger) {
			if (rightTriggerToggle)
				PIC.EnableRightTriggerControl();
			else
				PIC.DisableRightTriggerControl();
		}
		
		if (adjustLeftShoulderButton) {
			if (leftShoulderButtonToggle)
				PIC.EnableLeftShoulderControl();
			else
				PIC.DisableLeftShoulderControl();
		}
		
		if (adjustRightShoulderButton) {
			if (rightShoulderButtonToggle)
				PIC.EnableRightShoulderControl();
			else
				PIC.DisableRightShoulderControl();
		}
		
		// -- DPAD -- //
		if (adjustDPadUp) {
			if (dPadUpToggle)
				PIC.EnableDPadUpControl();
			else
				PIC.DisableDPadUpControl();
		}
		
		if (adjustDPadDown) {
			if (dPadDownToggle)
				PIC.EnableDPadDownControl();
			else
				PIC.DisableDPadDownControl();
		}
		
		if (adjustDPadLeft) {
			if (dPadLeftToggle)
				PIC.EnableDPadLeftControl();
			else
				PIC.DisableDPadLeftControl();
		}
		
		if (adjustDPadRight) {
			if (dPadRightToggle)
				PIC.EnableDPadRightControl();
			else
				PIC.DisableDPadRightControl();
		}
		
		// -- FACE BUTTONS -- //
		
		if (adjustFaceButtons) {
			if (faceButtonsToggle)
				PIC.EnableFaceButtonsControl();
			else
				PIC.DisableFaceButtonsControl();
		}
		
		// -- MOVEMENT -- //

		if (adjustColorShift) {
			if (colorShiftToggle) {
				PIC.EnableColorShift(); 
			} else {
				PIC.DisableColorShift();
			}
		}

		/*if (adjustShield) {
			if (shieldToggle) {
				PIC.EnableShield ();
			} else {
				PIC.DisableShield ();
			}
		}
		
		if (adjustTeleport) {
			if (teleportToggle) {
				PIC.EnableTeleport ();
			} else {
				PIC.DisableTeleport ();
			}
		}

		if (adjustActiveAbility) {
			if (activeAbilityToggle) {
				PIC.EnableActiveAbility ();
			} else {
				PIC.DisableActiveAbility ();
			}
		}
		
		if (adjustDebug) {
			if (debugToggle)
				PIC.EnableDebug ();
			else
				PIC.DisableDebug ();
		}*/

		if (adjustInvincible)
		{
			if (invincibleToggle == true)
			{
				GameManager.Instance.playerShip.isInvincible = true;
			}
			else
			{
				GameManager.Instance.playerShip.isInvincible = false;
			}
		}

		if(batteryChargeToggle == true) {
			GameManager.Instance.playerShip.LS.CurrentHP = batteryLevel;
		}

		if (coreDumpToggle == true) {
			if (dumpAllCores)
				dumpCount = GameManager.Instance.playerShip.fusionCoreInventoryCapacity;
			for (int i = 0; i < dumpCount; i++) {
				if (GameManager.Instance.playerShip.fusionCoreInventory.Count > 0) {
					//Debug.Log ("Dumping Core");
					GameManager.Instance.playerShip.fusionCoreInventory.RemoveAt (0);
				}
			}
		}

		if (laneToggle == true) {
			//Debug.Log ("Player Control Trigger: Toggle to Lane [" + laneDestination + "]");
			//GameManager.Instance.playerShip.GearMovementApp.ForceMovementToggle(LaneManager.Instance.laneArray[laneDestination].y, false, 0, enableControl);
		}

		if (adjustSpeedOverride)
		{
			GameManager.Instance.playerShip.GearMovementApp.OverrideSpeed = overrideSpeedToggle;
			GameManager.Instance.playerShip.GearMovementApp.SetSpeedOverride(overrideSpeed.x, overrideSpeed.y, overrideSpeed.z);
		}
		
		if (adjustPlayAreaToggle)
		{
			LaneManager.Instance.currentAnchorLanes = newPlayArea;
		}
		
		if (adjustCameraZoomToggle)
		{
			GameManager.Instance.mainCamera.transform.DOMoveZ(cameraZoomDistance, cameraZoomSpeed);
		}
		
	}
}
