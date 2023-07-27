using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Player;
using MoreMountains.Feedbacks;
using UnityEngine;
using Chronos;

public class AbilityPlayerShieldBasic : AbilityPlayerShieldBase
{
        void Start ()
    {
        InitShieldAbility();
    }

    private void FixedUpdate()
    {
        if ( _playerShip.isShieldActive)
        {
            if (_playerShip.LS.CurrentHP <= 0.2f)
            {
                ExitShield();
            }
            else
            {
                ActiveShieldDrainCalc();
            }
        }

    }
    
    public void InitShieldAbility()
    {
        _playerShip = GetComponent<PlayerShip>();
        _playerShip.shieldFixedAbility = this;
        isRechargingShield = false;
        _playerShip.isShieldActive = false;
        isShieldAvailable = true;
        StartCoroutine(_playerShip.sFX.ShieldShutdown());
    }

    #region ActivateShield


    public void DoAbilityPrimary()
    {
        ActivateShield();
    }
    
    public void ExitAbilityPrimary()
    {
        ExitShield();
    }
    
    //TODO: NO Secondary ability available. Use first again.
    public void DoAbilitySecondary()
    {
        ActivateShield();
    }
        
    public void ExitAbilitySecondary()
    {
        ExitShield();
    }

    #endregion

    public override void ActiveShieldDrainCalc() //TODO: Introduce Curve for drain by time in polish phase 
    {
        base.ActiveShieldDrainCalc();
        _playerShip.LS.SpendLife(shieldDrainFactor);
    }
}