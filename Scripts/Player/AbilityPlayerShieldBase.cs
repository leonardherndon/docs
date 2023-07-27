using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Player;
using MoreMountains.Feedbacks;
using UnityEngine;
using Chronos;



    public class AbilityPlayerShieldBase : MonoBehaviour, PlayerAbilityInterface
    {
            [SerializeField] protected float _batteryChargeRequired;
    
    public float BatteryChargeRequired 
    {
        get => _batteryChargeRequired;
        set => _batteryChargeRequired = value;
    }

    [SerializeField] protected ILifeSystem _playerLifeSystem;
    
    public ILifeSystem PlayerLifeSystem 
    {
        get => _playerLifeSystem;
        set => _playerLifeSystem = value;
    }

    [SerializeField] protected bool isShieldAvailable;
    
    [SerializeField] protected PlayerShip _playerShip;
    
    [SerializeField]  protected bool hasUsedShield;
    [SerializeField] protected float shieldDrainFactor = 0.001f;
    [SerializeField]   protected bool isRechargingShield;
    [SerializeField]  public float shieldDelayTime;
    
    [SerializeField] protected StatusEffectDataBlock _shieldEffectData;
    
    public StatusEffectDataBlock ShieldEffectDataBlock
    {
        get => _shieldEffectData;
        set => _shieldEffectData = value;
    }

    [SerializeField] protected int _shieldId;
    
    [Header("FEEDBACKS")] 
    [SerializeField] protected MMFeedbacks _mmfShieldActivate;
    [SerializeField] protected MMFeedbacks _mmfShieldDeactivate;
    
    public delegate void AbilityShieldHandler();
    public event AbilityShieldHandler UsedShieldAbility;
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
 #region SHIELD

    #region ActivateShield


    public virtual void DoAbilityPrimary()
    {
        ActivateShield();
    }
    
    public virtual void ExitAbilityPrimary()
    {
        ExitShield();
    }
    
    //TODO: NO Secondary ability available. Use first again.
    public virtual void DoAbilitySecondary()
    {
        ActivateShield();
    }
        
    public virtual void ExitAbilitySecondary()
    {
        ExitShield();
    }
    
    public virtual void ActivateShield()
    {
       if ( _playerShip.isShieldActive)
        {
            return;
        }

        if (!isShieldAvailable || _playerShip.CoC.CSM.CurrentColor == GameColor.Grey)
        {
            _playerShip.CantActivateAbility.PlayFeedbacks();
            return;
        }

        if (_playerShip.LS.CurrentHP < _batteryChargeRequired)
        {
            _playerShip.CantActivateAbility.PlayFeedbacks();
            return;
        }
        
        _playerShip.isShieldActive = true;
        _shieldId = EffectRequestManager.Instance.AddStatusEffectToObject(gameObject, gameObject, _shieldEffectData);
        _playerShip.LS.SpendLife(_batteryChargeRequired);
        StartCoroutine(_playerShip.sFX.ShieldAction());
        _mmfShieldActivate.PlayFeedbacks();
        UsedShieldAbility?.Invoke();

    }

    #endregion

    #region ExitShield

    public void ExitShield()
    {
        _mmfShieldDeactivate.PlayFeedbacks();
        EffectRequestManager.Instance.KillEffectRequestByID(_playerShip.CoC.StatusEffectRequests, _shieldId);
        StartCoroutine(_playerShip.sFX.ShieldShutdown());
        _playerShip.isShieldActive = false;
        StartCoroutine(RechargeShieldAbility());
    }

    #endregion

    #region RechargeShield

    public IEnumerator RechargeShieldAbility()
    {
        float shieldRechargeTimer;

        if (isRechargingShield)
        {
            yield break;
        }
        
        shieldRechargeTimer = Time.time + shieldDelayTime;

        while (Time.time < shieldRechargeTimer)
        {
            yield return null;
        }
        
        isShieldAvailable = true;
    }

    #endregion
    
    public virtual void ActiveShieldDrainCalc() //TODO: Introduce Curve for drain by time in polish phase 
    {
        UsedShieldAbility?.Invoke();
    }
    

    #endregion
    }
