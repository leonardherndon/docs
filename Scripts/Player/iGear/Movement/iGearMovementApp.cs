using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Player;
using UnityEngine;

public interface IGearMovementApp {
    /// <summary>
    /// This is what actually moves the player ship
    /// </summary>
    /// <param name="shipMoveDown"></param>
    void Move();
    
    void ForceMovementToggle(float yPos, bool enableMovement = true);
    
    /// <summary>
    /// Set the gear to be the given gear is setGear is true, else increment the gear based on the given value for gear.
    /// This in turn calls SetSpeed directly.
    /// </summary>
    /// <param name="gear"></param>
    /// <param name="setGear"></param>
    void SetSpeedGear(int gear, bool setGear);
    
    /// <summary>
    /// Set the vertical and horizontal speed with the given values, unless overrideSpeed get set to true
    /// </summary>
    /// <param name="speedHoriOverride"></param>
    /// <param name="speedVertOverride"></param>
    void SetSpeed();

    void SetSpeedOverride(float hori, float vert, float drag = 0);
    
    bool OverrideSpeed { get; set; }

    bool ShipMoveDown { get; set; }
    
    bool ShipMoveUp { get; set; }
    
    bool ShipMoveLeft { get; set; }
    
    bool ShipMoveRight { get; set; }
    
    bool ShipMoveBrake { get; set; }

    Vector3 ShipCurrentVelocity { get; set; }
    void DoBoost();

    void DoBackDash();

    void DoDoublePressLeft();
    
    void DoDoublePressRight();
    
    void DoBarrelRoll(bool isUp);

    void ExitBarrelRoll();

    void DoGravBrake();

    void DoGravBrakeRelease();
    
    void DoFlip();
    
    void FinishFlip();
    
    void DoAccelerator();

    void DoAcceleratorRelease();
}
