using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using Language.Lua;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageRequest : MonoBehaviour, IEffectRequest
{
    [SerializeField] private string _sourceName;
    [SerializeField] private int requestId;
    public int RequestId
    {
        get => requestId;
        set => requestId = value;
    }
    
    [SerializeField] private int sourceObjectId;
    public int SourceObjectId
    {
        get => sourceObjectId;
        set => sourceObjectId = value;
    }
    
    [FormerlySerializedAs("effectApplicationType")] [FormerlySerializedAs("_damageStatusApplicationType")] [SerializeField] private DamageApplicationType damageApplicationType = DamageApplicationType.Ignore;
    [SerializeField] private DamageSeverity _severity = DamageSeverity.Normal;
    [SerializeField] private ImpactType _impactType = ImpactType.Physical;
    [SerializeField] private float[] _damageAmount = new float[3] {1f,50f,90f};
    [SerializeField] private int _limit = 0;
    [SerializeField] private float _tickRate = 0;
    [SerializeField] private float _startDelay = 0;
    [SerializeField] private SimpleTimer _timer;
    [SerializeField] private ICollidible _CoC;
    [SerializeField] private ICollidible _sourceCoC;
    [SerializeField] private float _velMultiplier;

    public void CreateDamageRequest(DamageApplicationType type, float[] dAmount, int lmt, float tRate,
        float sDelay, DamageSeverity _srv, ImpactType _impt, ICollidible _other, string _sName,
        float velMultiplier = 0f)
    {
        SourceObjectId = _other.CollisionObjectId;
        damageApplicationType = type;
        _damageAmount = dAmount;
        _limit = lmt;
        _tickRate = tRate;
        _startDelay = sDelay;
        _severity = _srv;
        _impactType = _impt;
        _sourceCoC = _other;
        _sourceName = _sName;
        _velMultiplier = velMultiplier;
    }

    void Awake()
    {
        requestId = GetInstanceID();
    }
    void Start()
    {
        _CoC = gameObject.GetComponent<ICollidible>();
        _CoC.DamageRequests.Add(this);
        if (damageApplicationType == DamageApplicationType.Instant || damageApplicationType == DamageApplicationType.Velocity)
        {
            DoDamage();
            KillSelf();
            return;
        }
        
        if (_limit != 0)
        {
            _timer = new SimpleTimer(_tickRate, _limit, true);
        }
        else
        {
            _timer = new SimpleTimer(_tickRate, 0, false);
        }

        _timer.TimerLoopEvent += DoDamage;
        _timer.TimerCompleteEvent += KillSelf;
    }
    void LateUpdate()
    {
        if (_timer != null)
        {
            _timer.Update(Time.deltaTime); //TODO SWITCH TO CHORNOS TIME SCALES
        }
    }

    void KillSelf()
    {
        EffectRequestManager.Instance.RemoveDamageRequestFromList(_CoC.DamageRequests,this);
        if (_timer != null)
        {
            _timer.TimerLoopEvent -= DoDamage;
            _timer.TimerCompleteEvent -= KillSelf;
        }

        Destroy(this);
    }
    /*public void RunDoDamage()
    {
        StartCoroutine(DoDamage());
    }*/

    public void DoDamage()
    {
        
        float _damage = 0;

        switch (_severity)
        {
            case DamageSeverity.Dampened:
                _damage = 1f; //_damageAmount[0];
                break;
            case DamageSeverity.Normal:
                _damage = _damageAmount[1];
                break;
            case DamageSeverity.Amplified:
                _damage = _damageAmount[2];
                break;
        }

        switch (damageApplicationType)
        {
            case DamageApplicationType.Velocity:
                _damage = _damageAmount[0] * _velMultiplier;
                gameObject.GetComponent<ILifeSystem>().DrainLife(_damage,_impactType);
                Debug.Log("Ran into Wall: Doing " + _damage + " amount of damage.");
                break;
            case DamageApplicationType.Sustained:
                //CHECK BOUNDS FOR CONTINUED COLLISION
                bool isSustained = true; //EffectRequestManager.Instance.CheckForSustainedBounds(_CoC,_sourceCoC);
                if (isSustained)
                    gameObject.GetComponent<ILifeSystem>().DrainLife(_damage,_impactType);
                break;
            case DamageApplicationType.DoT:
                var DOTcoroutine = gameObject.GetComponent<ILifeSystem>()
                    .DrainLifeDOT(_damage, _limit, _impactType, _tickRate);
                StartCoroutine(DOTcoroutine);
                break;
            case DamageApplicationType.Delayed:
                var Delayedcoroutine = gameObject.GetComponent<ILifeSystem>()
                    .DrainLifeDelayed(_damage, _limit, _impactType, _tickRate);
                StartCoroutine(Delayedcoroutine);
                break;
            default:
                gameObject.GetComponent<ILifeSystem>().DrainLife(_damage,_impactType);
                break;
        }
        
        
        
    }

}
