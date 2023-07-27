using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Player;
using UnityEngine;
using Sirenix.OdinInspector;
using Chronos;
using CS_Audio;
using Rewired.Utils.Classes.Data;

public class GearMoveApp : MonoBehaviour, IGearMovementApp
{
    
    [SerializeField] protected PlayerShip playerShip;
    public float horiSpeed;
    public float vertFactor;
    // public float vertSpeed;
    public float horizonatalForce;
    public float verticalForce;
    public float[] vertFactorTable;
    public int[] horiSpeedTable;
    [SerializeField] protected bool isFacingLeft = false;
    [SerializeField] protected bool isFlipping = false;
    [SerializeField] protected bool isRolling;
    public float horizontalAxis;
    public float verticalAxis;
    [ShowInInspector] private float _baseHorizontalSpeed = 1f;
    public Vector2 speedOverride;
    [SerializeField] protected Vector3 shipCurrentVelocity;

    public Vector3 ShipCurrentVelocity
    {
        get => shipCurrentVelocity;
        set => shipCurrentVelocity = value;
    }

    [Header("[MOVEMENT VARIABLES]")]
    
    [SerializeField] private bool shipMoveUp;
    public bool ShipMoveUp
    {
        get => shipMoveUp;
        set
        {
            shipMoveUp = value;
        }
    }
    
    [SerializeField] private bool shipMoveDown;
    public bool ShipMoveDown
    {
        get => shipMoveDown;
        set
        {
            shipMoveDown = value;
        }
    }

    [SerializeField] private bool shipMoveLeft;
    public bool ShipMoveLeft
    {
        get => shipMoveLeft;
        set
        {
            shipMoveLeft = value;
        }
    }
    
    [SerializeField] private bool shipMoveRight;
    public bool ShipMoveRight
    {
        get => shipMoveRight;
        set
        {
            shipMoveRight = value;
        }
    }

    [SerializeField] private bool shipMoveBrake;
    public bool ShipMoveBrake
    {
        get => shipMoveBrake;
        set
        {
            shipMoveBrake = value;
        }
    }
    
    public const float BASESPEED = 5;
    
    

    public float BaseHorizontalSpeed
    {
        get => _baseHorizontalSpeed;
        set
        {
            _baseHorizontalSpeed = value;
        }
    }



    [ShowInInspector] private float _baseVerticalSpeed = 1f;

    public float BaseVerticalSpeed
    {
        get => _baseVerticalSpeed;
        set => _baseVerticalSpeed = value;
    }

    [SerializeField] public bool _overrideSpeed;

    public bool OverrideSpeed
    {
        get => _overrideSpeed;
        set
        {
            _overrideSpeed = value;
            _overrideSpeed = value;
        }
    }

    protected virtual void Awake()
    {
        

    }

    public virtual void SetSpeedGear(int gear, bool setGearDirect)
    {
        //Debug.LogFormat("TODO: Implement GearMoveApp::SetSpeedGear({0}, {1}", gear, setGearDirect);
        //@TODO Implement this
    }                

    public virtual void SetSpeed()
    {
        if (!_overrideSpeed)
        {
            horiSpeed = horiSpeedTable[1];
            vertFactor = vertFactorTable[0];
        }
        else
        {
            horiSpeed = speedOverride[0];
            vertFactor = speedOverride[1];
        }
    }

    public virtual void SetSpeedOverride(float hori, float vert, float drag)
    {
        speedOverride[0] = hori;
        speedOverride[1] = vert;
    }

    
    public virtual void DoGravBrake()
    {
        //No Braking Here
    }
    
    public virtual void DoGravBrakeRelease()
    {
        //No Braking Here
    }
    
    public virtual void TurnAround()
    {
        //No Braking Here
    }

    public virtual void DoBackDash()
    {
        //No Backdash Here
    }
    
    public virtual void DoBoost()
    {
        //No Boost Here
    }
    
    public virtual void DoBarrelRoll(bool isUp)
    {
        //No Barrel Roll Here
    }

    public virtual void ExitBarrelRoll()
    {
        
    }

    public virtual void DoFlip()
    {
        //No Accelerator Here
        
    }
    
    public virtual void FinishFlip()
    {
        isFlipping = false;
    }
    public virtual void DoAccelerator()
    {
        //No Accelerator Here
        
    }
    
    public virtual void DoAcceleratorRelease()
    {
        //No Accelerator Here
    }

    public virtual void Move()
    {
        horizonatalForce = CalculateHorizontalForce();
        verticalForce = CalculateVerticalForce(verticalAxis);

        if (isFacingLeft)
        {
            horizonatalForce *= -1;
            //verticalForce *= -1;
        }
        
        //Debug.Log("Time to add force\n Hori:" + horizonatalForce + "Vert: " + verticalForce);
        //FINALLY TIME TO MOVE THE SHIP
        playerShip.rB.AddForce(
            horizonatalForce,
            verticalForce,
            0,
            ForceMode.Acceleration
        );
    }

    public float CalculateHorizontalForce()
    {
        //FIGURE OUT IF ANYTHING SHOULD BE MANIPULATING THE SHIP'S SPEED
        playerShip.speedMod = playerShip.FindSpeedModifiers();
        
        playerShip.speedFinal = horiSpeed * playerShip.speedMod * _baseHorizontalSpeed;

        //OVERRIDE FINAL SPEED
        if (playerShip.isSpeedOverwritten)
        {
            playerShip.speedFinal = speedOverride.x;
        }
        //Debug.Log("Calculator Hori Force Completed:" + playerShip.speedFinal);
        return playerShip.speedFinal * playerShip.BaseTimeLine.fixedDeltaTime;
    }

    public float CalculateVerticalForce(float playerAxis)
    {
        var  vertSpeed = 0.0f;
        
        if (!shipMoveDown && !shipMoveUp)
        {
            return 0.0f;
        }
        
        if (playerAxis > 0 && (playerShip.isBumperTop))
        {
            return 0.0f;
        }

        if (playerAxis < 0 && (playerShip.isBumperBottom))
        {
            return 0.0f;
        }
        
        if (playerShip.isSpeedOverwritten)
        {
            return playerAxis * speedOverride.y * playerShip.BaseTimeLine.fixedDeltaTime * _baseVerticalSpeed;
        }
        //Debug.Log("Vert Factor Compeleted \n Axis: " + playerAxis + "\nVert Factor: " + vertFactor + "\nBase Speed: " + _baseHorizontalSpeed + "\n Time: " + playerShip.BaseTimeLine.fixedDeltaTime);
        return playerAxis * vertFactor * playerShip.BaseTimeLine.fixedDeltaTime * _baseVerticalSpeed;
    }
    
    
    #region ForcedMovement()

    public void ForceMovementToggle(float yPos, bool enableMovement = true)
    {
        StartCoroutine(ForcedShipMove(yPos, enableMovement));
    }

    public IEnumerator ForcedShipMove(float yPos,
        bool enableMovement = true)
    {
        _overrideSpeed = true;
        playerShip.PIC.CachedDisableControl();
        float targetRange = 0.33f;

        if (yPos > transform.position.y)
        {
            shipMoveDown = false;
            shipMoveUp = true;
        }
        else if (yPos < transform.position.y)
        {
            shipMoveUp = false;
            shipMoveDown = true;
        }
        

        float targetCalc = yPos - Math.Abs(targetRange);
        float positionCalc = yPos - Math.Abs(transform.position.y);

        bool runWhileLoop = ShipOutofRange(yPos, targetRange);

        while (runWhileLoop)
        {
            positionCalc = yPos - Math.Abs(transform.position.y);
            runWhileLoop = ShipOutofRange(yPos, targetRange);

            yield return null;
        }

        if (enableMovement)
        {
            playerShip.PIC.RestoreControl();
        }

        _overrideSpeed = false;
    }


    public bool ShipOutofRange(float yPos, float targetRange)
    {
        if (transform.position.y < (yPos + targetRange) && transform.position.y > (yPos - targetRange))
            return false;
        else
            return true;
    }

    #endregion


    public virtual void DoDoublePressLeft()
    {
        
    }
    
    public virtual void DoDoublePressRight()
    {
        
    }
    
    public float GetBaseSpeed()
    {
        return _baseHorizontalSpeed;
    }

    public float GetAccelerationRate()
    {
        return playerShip.FindSpeedModifiers();
    }

    public float GetDecelerationRate()
    {
        throw new NotImplementedException();
    }

    public float GetMaxSpeed()
    {
        throw new NotImplementedException();
    }

    public float GetMinSpeed()
    {
        throw new NotImplementedException();
    }

    public float GetEnhancedAccelerationRate()
    {
        throw new NotImplementedException();
    }

    public float GetEnhancedDecelerationRate()
    {
        throw new NotImplementedException();
    }

    public float GetEnhancedMaxSpeed()
    {
        throw new NotImplementedException();
    }

    public float GetEnhancedMinSpeed()
    {
        throw new NotImplementedException();
    }

    public float GetMaximumReverseDistance()
    {
        throw new NotImplementedException();
    }

    public void CleanAxis()
    {
        // var cleanX = Quaternion.AngleAxis(0, transform.right);
        // var cleanZ = Quaternion.AngleAxis(0, transform.forward);
        // playerShip.shipRender.transform.rotation = cleanZ;
        // playerShip.shipRender.transform.rotation = cleanX;
    }

    public void LockRotation()
    {
        playerShip.rB.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void UnlockRotation()
    {
        playerShip.rB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }
}