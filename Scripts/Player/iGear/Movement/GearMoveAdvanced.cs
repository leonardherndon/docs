using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.ObjectAttributeSystem;
using Chronos;
using UnityEngine;
using CS_Audio;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;


public class GearMoveAdvanced : GearMoveApp
{
    private GearSystem _gearSystem;
    
    
    [SerializeField] private ILifeSystem _lifeSystem;
    [SerializeField] private AttributeController _attributeController;
    [FormerlySerializedAs("animator")] [SerializeField] private Animator leanAnimator;
    [SerializeField] private Animator flipAnimator;
    [Header("INPUT")]

    [Range(0.01f, 0.1f)]
    [SerializeField] private float deadZone = 0.08f;
    [SerializeField] private float horiInputCache = 0f;
    [SerializeField] private float vertInputCache = 0f;
    
    [Header("VELOCITY/DRAG")]
    [Range(-10.0f, 60.0f)]
    
    [SerializeField] private float[] velocityLimit = new float[] {-5f, 40f};
    [Range(0.0f, 10000.0f)]
    [SerializeField] private float[] horiSpeedAcceleration = new float[] {1000f, 0, 1000f, 3000f};
    [Range(-10.0f, 200.0f)]
    [SerializeField] private float[] horiDragTable = new float[] {4f, 3f, 6f, 20f};
    [SerializeField] private float horiDragOverride;
    [Range(0.0f, 200.0f)]
    [SerializeField] private float[] vertDragTable = new float[] {0f, 6f};
    [SerializeField] private float vertDragFactor;
    [SerializeField] private float horiDragFactor;
    

    
    [Header("TRAIL")]
    private float[] trailstrengthTable = new float[] { 0.25f, 0.4f, 0.65f, 1.2f, 1.2f, 1.2f };
    private GameObject[] _speedTrails;
    public GameObject[] speedTrails;
    
    
    [Header("BOOST")]
    [SerializeField] private float _boostBatteryCost = 10f;[Range(1.0f, 2.0f)]
    [SerializeField] private bool isBoosting;
    [SerializeField] private float horiImpulse = 5000f;
    [SerializeField] private bool isBoostManeuverActive = true;
    [Range(1, 5)]
    [SerializeField] private int frameThreshhold = 3;
    [SerializeField] private int frameCount = 0;
    
    [SerializeField] private float vertImpulse = 5000f;

    
    [Header("GRAVBRAKE")]
    [SerializeField] private bool isGravBrakeActive = false;
    [SerializeField] private Vector2 _gravBrakeModifier = new Vector2(0.02f,0.5f);
    public Vector2 GravBrakeModifier 
    {
        get => _gravBrakeModifier;
        set => _gravBrakeModifier = value;
    }

    [SerializeField] private float forwardBrakingStrength = 3f;


    [Header("ACCELERATOR")]
    [SerializeField] private bool isAcceleratorActive = false;
    [SerializeField] private Vector2 _accelModifier = new Vector2(2f,1.5f);
    public Vector2 AccelModifier 
    {
        get => _accelModifier;
        set => _accelModifier = value;
    }
    
    [SerializeField] private Vector2 _accelImpulseModifier = new Vector2(2f,1.5f);
    public Vector2 AccelImpulseModifier 
    {
        get => _accelImpulseModifier;
        set => _accelImpulseModifier = value;
    }
    
    
    [Header("BARRELROLL")]
    [SerializeField] private float _barrelRollBatteryCost = 5f;
    
    [Header("BACKDASH")]
    [SerializeField] private float backDashLength = 0.1f;
    [SerializeField] private float backDashFallOff = 10f;
    [SerializeField] private float backDashBatteryCost = 20f;
    [SerializeField] private float backDashVelocityThreshold = 20f;

    [Header("FEEDBACKS")] 
    [SerializeField] private MMFeedbacks _mmfBoost;
    [SerializeField] private MMFeedbacks _mmfBarrelRoll;
    [SerializeField] private MMFeedbacks _mmfGravBrake;
    [SerializeField] private MMFeedbacks _mmfAccel;

    public delegate void PlayerSpeedHandler(float hori,float vert);

    public event PlayerSpeedHandler SpeedSet;
    
    public float HoriDragOverride
    {
        get => horiDragOverride;
        set => horiDragOverride = value;
    }

    //TODO: IMPLEMENT VERTICAL AND HORIZONTAL DRAG. 
    protected override void Awake()
    {
        if(!leanAnimator)
            leanAnimator = gameObject.GetComponentInChildren<Animator>();
        
        vertFactorTable = new float[]   { 2000, 2000 }; //{ 1500, 2750, 3500, 5000, 8000, 8000 };

        _gearSystem = gameObject.GetComponent<GearSystem>();
        _gearSystem.moveSystem = this;
        _attributeController = playerShip.AttributeController;
        SetSpeed();
    }

    private void OnEnable()
    {
        _lifeSystem = gameObject.GetComponent<ILifeSystem>();
        isBoosting = false;
        isRolling = false;
    }

    public override void DoGravBrake()
    {
        if (shipCurrentVelocity.x >= 5f)
        {
            _mmfGravBrake.PlayFeedbacks();
        }

        isGravBrakeActive = true;
        
    }

    public override void DoGravBrakeRelease()
    {
        isGravBrakeActive = false;
    }
    
   
    
    public override void DoAccelerator()
    {
        // if (shipCurrentVelocity.x >= 5f)
        // {
        //     _mmfAccel.PlayFeedbacks();
        // }

        isAcceleratorActive = true;
        
    }
    
    public override void DoAcceleratorRelease()
    {
        isAcceleratorActive = false;
    }

    public void FixedUpdate()
    {

        //if(ShipMoveLeft || ShipMoveRight) 
        if (!isFlipping)
        {
            if (!isFacingLeft)
            {
                horizontalAxis = GameManager.Instance.playerController.GetAxis("MoveHorizontal");
            }
            
            else
            {
                horizontalAxis = GameManager.Instance.playerController.GetAxis("MoveHorizontal") * -1;
            }
        }
        
        shipCurrentVelocity = playerShip.rB.velocity;
        verticalAxis = GameManager.Instance.playerController.GetAxis("MoveVertical");
        SetSpeed();
        leanAnimator.SetFloat("Lean",verticalAxis);
        
        
        SetTrailFX(3);
        
    }

    public override void SetSpeed()
    {
        if (!_overrideSpeed)
        {
            velocityLimit[1] = _attributeController.CalculateSpeedAdjustment(velocityLimit[1]);

            if (!isFacingLeft)
            {
                if (shipCurrentVelocity.x <= velocityLimit[0] || velocityLimit[1] <= shipCurrentVelocity.x)
                {
                    horiDragFactor = horiDragTable[2];
                }
                else
                {
                    horiDragFactor = horiDragTable[1];
                }
            }
            else
            {
                if (-shipCurrentVelocity.x <= -velocityLimit[0] || -velocityLimit[1] <= -shipCurrentVelocity.x)
                {
                    horiDragFactor = horiDragTable[2];
                }
                else
                {
                    horiDragFactor = horiDragTable[1];
                }
            }

            if (!isFacingLeft)
            {
                if (horizontalAxis <= -deadZone && 0 < shipCurrentVelocity.x)
                {

                    horiSpeed = horizontalAxis * (horiSpeedAcceleration[0] * forwardBrakingStrength);
                    horiDragFactor = horiDragTable[0];
                    

                }
                else if (horizontalAxis <= -deadZone)
                {

                    horiSpeed = horizontalAxis * horiSpeedAcceleration[0];
                    horiDragFactor = horiDragTable[0];
                    

                }
                else if (-deadZone <= horizontalAxis && horizontalAxis <= deadZone)
                {
                    horiSpeed = horizontalAxis * horiSpeedAcceleration[1];
                }

                else if (horizontalAxis >= deadZone)
                {
                    horiSpeed = horizontalAxis * horiSpeedAcceleration[2];
                }
            }
            else
            {
                if (horizontalAxis <= deadZone && 0 < -shipCurrentVelocity.x )
                {

                    horiSpeed = horizontalAxis * (horiSpeedAcceleration[0] * forwardBrakingStrength);
                    horiDragFactor = horiDragTable[0];
                    

                }
                else if (horizontalAxis <= deadZone)
                {

                    horiSpeed = horizontalAxis * horiSpeedAcceleration[0];
                    horiDragFactor = horiDragTable[0];
                    

                }
                else if (deadZone <= horizontalAxis && horizontalAxis <= -deadZone)
                {
                    horiSpeed = horizontalAxis * horiSpeedAcceleration[1];
                }
            
                else if (horizontalAxis >= -deadZone)
                {
                    horiSpeed = horizontalAxis * horiSpeedAcceleration[2];
                }
            }

            if (horizontalAxis >= 0.85)
                horiDragFactor = horiDragFactor * 0.8f;
            vertFactor = vertFactorTable[0];
       


            if (frameCount <= frameThreshhold)
                frameCount++;
            else
            {
                frameCount = 0;
                vertInputCache = verticalAxis;
                horiInputCache = horizontalAxis;
            }
            
            horiSpeed = _attributeController.CalculateSpeedAdjustment(horiSpeed);
            vertFactor = _attributeController.CalculateSpeedAdjustment(vertFactor);

            CheckManeuvers();
        }
        else
        {
            
            horiSpeed = speedOverride[0];
            vertFactor = speedOverride[1];

            horiDragFactor = horiDragOverride;
        }
        
       

        SpeedSet?.Invoke(horiSpeed, vertFactor);
    }
    
    void CheckManeuvers()
    {
        if (isAcceleratorActive)
        {
            horiSpeed *= _accelModifier.x;
            vertFactor *= _accelModifier.y;
        }
            
        if (isGravBrakeActive)
        {
            horiSpeed *= _gravBrakeModifier.x;
            vertFactor *= _gravBrakeModifier.y;
            horiDragFactor = horiDragTable[3];
        }
    }
    
    public override void SetSpeedOverride(float hori, float vert, float drag = 0)
    {
        speedOverride[0] = hori;
        speedOverride[1] = vert;
        horiDragOverride = drag;
    }

    private float HoriDifference()
    {
        var horiDifference = horizontalAxis - horiInputCache;
        return horiDifference;
    }

    private float VertDifference()
    {
        var vertDifference = verticalAxis - vertInputCache;
        return vertDifference;
    }

    public override void DoBarrelRoll(bool isUp)
    {
        
        if (isRolling)
            return;
        
        if (_lifeSystem.CurrentHP < _barrelRollBatteryCost)
            return;
        playerShip.defaultRot = gameObject.transform.rotation;
        Debug.Log("Vertical Axis: " + verticalAxis);
        _mmfBarrelRoll.PlayFeedbacks();
        _lifeSystem.SpendLife(_barrelRollBatteryCost);
        isRolling = true;
        vertDragFactor = vertDragTable[1];
        leanAnimator.SetTrigger("startRoll");
        
        
        if (isUp)
        {
            //if (!isFacingLeft)
                leanAnimator.SetTrigger("rollLeft");
            //else
            //    leanAnimator.SetTrigger("rollRight");
            
            verticalForce =  vertImpulse;
        }
        else if(!isUp)
        {
            //if (!isFacingLeft)
                leanAnimator.SetTrigger("rollRight");
            //else
            //    leanAnimator.SetTrigger("rollLeft");
            
            verticalForce =  -vertImpulse;
        }

        //Calculate mods before sending through system
        var rollForce = _attributeController.CalculateSpeedAdjustment(verticalForce);
        if (isAcceleratorActive)
        {
            rollForce *= _accelImpulseModifier.y;
        }

        //FINALLY TIME TO MOVE THE SHIP
        playerShip.rB.AddForce(
            0,
            rollForce,
            0,
            ForceMode.Impulse
        );
        
    }
    
    public override void ExitBarrelRoll()
    {
        isRolling = false;
        frameCount = 0;
        vertInputCache = verticalAxis;
        vertDragFactor = vertDragTable[0];
        leanAnimator.SetTrigger("endRoll");
        //CleanAxis();
    }
    
    public override void DoFlip()
    {
        if (isFlipping) return;
        
        isFlipping = true;
        UnlockRotation();

        if (!isFacingLeft)
        {

            GameManager.Instance.mainCamera.CameraTargets[0].TargetOffset = new Vector2(-GameManager.Instance.CameraStandardOffset, 0);
            flipAnimator.SetTrigger("flipLeft");
            isFacingLeft = true;
        }
        else
        {
            
            GameManager.Instance.mainCamera.CameraTargets[0].TargetOffset = new Vector2(GameManager.Instance.CameraStandardOffset,0);
            flipAnimator.SetTrigger("flipRight");
            isFacingLeft = false;
        }

    }
    
    public override void FinishFlip()
    {
        flipAnimator.SetTrigger("endFlip");
        CleanAxis();
        LockRotation();
        isFlipping = false;
    }

    
    public override void DoBoost()
    {
        if(!isBoostManeuverActive)
            return;
        
        if (isBoosting)
            return;
        
        if (horizontalAxis < horiInputCache)
            return;
        
        if (_lifeSystem.CurrentHP < _boostBatteryCost)
            return;
        
        _mmfBoost.PlayFeedbacks();
        _lifeSystem.SpendLife(_boostBatteryCost);
        isBoosting = true;
        horiDragFactor = horiDragTable[2]/2f;

        //Calculate mods before sending through system
        var boostImpulse = _attributeController.CalculateSpeedAdjustment(horiImpulse);
        
        if (isAcceleratorActive)
        {
            boostImpulse *= _accelImpulseModifier.x;
        }

        if (isFacingLeft)
            boostImpulse *= -1;
        
        //FINALLY TIME TO MOVE THE SHIP
        playerShip.rB.AddForce(
            boostImpulse,
            0,
            0,
            ForceMode.Impulse
        );

        StartCoroutine(ExitBoost());
    }

    private IEnumerator ExitBoost()
    {
        yield return new WaitForSeconds(0.75f);
        
        isBoosting = false;
        frameCount = 0;
        horiInputCache = horizontalAxis;
    }

    public override void DoDoublePressLeft()
    {
        if (isFacingLeft)
            DoBoost();
        else
            DoBackDash();
    }
    
    public override void DoDoublePressRight()
    {
        if (isFacingLeft)
            DoBackDash();
        else
            DoBoost();
    }
    public override void DoBackDash()
    {
        if (isBoosting)
            return;
        
        if (horizontalAxis > horiInputCache)
            return;
        
        
        isBoosting = true;
        horiDragFactor = horiDragTable[2];
        if (shipCurrentVelocity.x > backDashVelocityThreshold)
            _lifeSystem.SpendLife(backDashBatteryCost*3f);
        else
            _lifeSystem.SpendLife(backDashBatteryCost);
        
        //Calculate mods before sending through system
        var boostImpulse = _attributeController.CalculateSpeedAdjustment(horiImpulse);
        
        if (isFacingLeft)
            boostImpulse *= -1;
        
        //FINALLY TIME TO MOVE THE SHIP
        playerShip.rB.AddForce(
            -boostImpulse,
            0,
            0,
            ForceMode.Impulse
        );

        StartCoroutine(ExitBackDash());
    }
    
    private IEnumerator ExitBackDash()
    {
        yield return new WaitForSeconds(backDashLength);
        
        isBoosting = false;
        frameCount = 0;
        horiInputCache = 0;

        playerShip.rB.velocity = new Vector3((playerShip.rB.velocity.x/backDashFallOff), playerShip.rB.velocity.y,playerShip.rB.velocity.z);
    }

   

    public override void Move()
    {
        base.Move();
        //Debug.Log("Move Advanced Move Update.");
        ActivateDrag();
    }

    private void ActivateDrag()
    {
        var vel = playerShip.rB.velocity;
        vel.x *= 1.0f-(horiDragFactor/100f); // reduce x component...
        vel.y *= 1.0f-(vertDragFactor/100f); // reduce x component...
        playerShip.rB.velocity = vel;
    }

    private void SetTrailFX(int gear)
    {
        return;
        
    }
}
