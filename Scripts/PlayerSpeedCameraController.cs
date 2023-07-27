using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.ProBuilder;
using Math = System.Math;

public class PlayerSpeedCameraController : MonoBehaviour
{

    [SerializeField] private float _acceleratedCameraOffsetModifier = 2f;
    [SerializeField] private float _velocityThreshold = 40f;
    [SerializeField] private bool _accelerated = false;

    [SerializeField]
    private GearMoveApp moveScript;

    /*private void Start()
    {
        moveScript = GameManager.Instance.playerShip.GetComponent<GearMoveApp>();
    }

    void FixedUpdate()
    {
        float shipVelocityX = moveScript.ShipCurrentVelocity.x;
        shipVelocityX = Math.Abs(shipVelocityX);
        if (shipVelocityX >= _velocityThreshold)
            ModifyCameraOffset();
        else
            _accelerated = false;

    }

    void ModifyCameraOffset()
    {
        if (_accelerated) return;
        GameManager.Instance.mainCamera.CameraTargets[0].TargetOffset.x *= _acceleratedCameraOffsetModifier;
        _accelerated = true;
    }*/
}
