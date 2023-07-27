using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Player;
using MoreMountains.Feedbacks;
using Chronos;
using UnityEngine.Serialization;

public class AbilityPlayerLumenShift : MonoBehaviour, PlayerAbilityInterface
{

    [SerializeField] private float _batteryChargeRequired;
    
    public float BatteryChargeRequired 
    {
        get => _batteryChargeRequired;
        set => _batteryChargeRequired = value;
    }

    [SerializeField] private ILifeSystem _playerLifeSystem;
    
    public ILifeSystem PlayerLifeSystem 
    {
        get => _playerLifeSystem;
        set => _playerLifeSystem = value;
    }
    
    [SerializeField] private bool isArmorAvailable;
    
    
    [SerializeField] private PlayerShip _playerShip;
    
    [SerializeField]  private bool hasUsedArmor;
    [FormerlySerializedAs("ArmorActiveTimer")] [SerializeField]   private float LumenShiftActiveTimer;
    [SerializeField]   private bool isRechargingArmor;
    [FormerlySerializedAs("ArmorDelayTime")] [SerializeField]  public float LumenShiftDelayTime;
    
    [FormerlySerializedAs("_ArmorEffectData")] [SerializeField] private StatusEffectDataBlock _LumenShiftEffectData;
    
    public StatusEffectDataBlock LumenShiftEffectDataBlock
    {
        get => _LumenShiftEffectData;
        set => _LumenShiftEffectData = value;
    }

    [SerializeField] private int _ArmorId;
    
    [Header("FEEDBACKS")] 
    [SerializeField] private MMFeedbacks _mmfArmorActivate;
    [SerializeField] private MMFeedbacks _mmfArmorDeactivate;
    
    
    public delegate void LumenShiftAbilityHandler();
    public event LumenShiftAbilityHandler UsedLumenShift;
    
    
    void Start ()
    {
        InitArmorAbility();
    }


    public void InitArmorAbility()
    {
        _playerShip = GetComponent<PlayerShip>();
        _playerShip.LumenShift = this;
        isRechargingArmor = false;
        _playerShip.CoC.CSM.LumenMode = LumenMode.Light;
        isArmorAvailable = true;
        UsedLumenShift?.Invoke();
    }
 #region Armor

    #region ActivateArmor


    public void DoAbilityPrimary()
    {
        ActivateArmor(LumenShiftActiveTimer);
    }
    
    public void ExitAbilityPrimary()
    {
        ExitArmor();
    }
    
    //TODO: NO Secondary ability available. Use first again.
    public void DoAbilitySecondary()
    {
        ActivateArmor();
    }
    
    public void ExitAbilitySecondary()
    {
        ExitArmor();
    }
        
    
    public void ActivateArmor(float time = 0)
    {

        if ( _playerShip.CoC.CSM.LumenMode == LumenMode.Dark)
        {
            return;
        }

        if (!isArmorAvailable)
        {
            _playerShip.CantActivateAbility.PlayFeedbacks();
            return;
        }

        if (_playerShip.LS.CurrentHP < _batteryChargeRequired)
        {
            _playerShip.CantActivateAbility.PlayFeedbacks();
            return;
        }

        
        _playerShip.LS.SpendLife(_batteryChargeRequired);
        _mmfArmorActivate.PlayFeedbacks();
        _ArmorId = EffectRequestManager.Instance.AddStatusEffectToObject(gameObject, gameObject, _LumenShiftEffectData);
        hasUsedArmor = true;
        _playerShip.CoC.CSM.LumenMode = LumenMode.Dark;
        
        StartCoroutine(RechargeArmorAbility());
    }

    #endregion
    

    #region ExitArmor

    public void ExitArmor()
    {
        if (_playerShip.CoC.CSM.LumenMode == LumenMode.Light)
        {
            return;
        }
        
        if (isArmorAvailable)
        {
            _playerShip.CantActivateAbility.PlayFeedbacks();
            return;
        }
        
        if (_playerShip.LS.CurrentHP < _batteryChargeRequired)
        {
            _playerShip.CantActivateAbility.PlayFeedbacks();
            return;
        }
        
        _playerShip.LS.SpendLife(_batteryChargeRequired);
        _mmfArmorDeactivate.PlayFeedbacks();
        EffectRequestManager.Instance.KillEffectRequestByID(_playerShip.CoC.StatusEffectRequests, _ArmorId);
        _playerShip.CoC.CSM.LumenMode = LumenMode.Light;
        
        StartCoroutine(RechargeArmorAbility());
    }

    #endregion

    #region RechargeArmor

    public IEnumerator RechargeArmorAbility()
    {
        float ArmorRechargeTimer;

        if (isRechargingArmor)
        {
            yield break;
        }

        float timerDelaySeconds = LumenShiftDelayTime;
        ArmorRechargeTimer = Time.time + timerDelaySeconds;


        while (Time.time < ArmorRechargeTimer)
        {
            yield return null;
        }
        
        if(!isArmorAvailable)
            isArmorAvailable = true;
        else
            isArmorAvailable = false;
    }

    #endregion

    #endregion
}
