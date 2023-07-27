using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Player;
using MoreMountains.Feedbacks;
using UnityEngine;
using Chronos;

public class AbilityPlayerShieldFixed : AbilityPlayerShieldBase
{

    [SerializeField] private float shieldDuration;
    
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
    
    public override void ExitAbilityPrimary()
    {
        
    }
    
    //TODO: NO Secondary ability available. Use first again.
    public void DoAbilitySecondary()
    {
        ActivateShield();
    }

    public void ExitAbilitySecondary()
    {
        
    }

    public override void ActivateShield()
    {
        base.ActivateShield();
        StartCoroutine(HoldShieldAbility());
    }
    #endregion

    #region HoldShield

    public IEnumerator HoldShieldAbility()
    {
        float shieldActiveTimer;

        if (!_playerShip.isShieldActive)
        {
            yield break;
        }
        
        shieldActiveTimer = Time.time + shieldDuration;

        while (Time.time < shieldActiveTimer)
        {
            yield return null;
        }
        
        ExitShield();
    }
    
    #endregion
    
    
}
