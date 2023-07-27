using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ChromaShift.Scripts;
using ChromaShift.Scripts.ObjectAttributeSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Chronos;

public class StatusEffectRequest : MonoBehaviour, IEffectRequest
{
    [SerializeField] private string _sourceName;
    
    
    [SerializeField] private int statusRequestId;
    public int RequestId
    {
        get => statusRequestId;
        set => statusRequestId = value;
    }

    [SerializeField] private int sourceObjectId;
    public int SourceObjectId
    {
        get => sourceObjectId;
        set => sourceObjectId = value;
    }
    
    [FormerlySerializedAs("statusEffect")] [FormerlySerializedAs("_statusEffectGroup")] [SerializeField] private StatusEffectType statusEffectType;

    public StatusEffectType StatusEffectType
    {
        get => statusEffectType;
        set => statusEffectType = value;
    }
    [FormerlySerializedAs("_damageStatusApplicationType")] [SerializeField]
    private EffectApplicationType effectApplicationType = EffectApplicationType.Null;
    public EffectApplicationType EffectApplicationType
    {
        get => effectApplicationType;
        set => effectApplicationType = value;
    }
    [SerializeField] private int _limit = 0;
    [SerializeField] private float _threshold = 0;
    [SerializeField] private float _tickRate = 0;
    public SimpleTimer _timer;
    [SerializeField] private ICollidible _CoC;
    [SerializeField] public ICollidible SourceCoC;
    [SerializeField] private StatusEffectDataBlock _dataBlock;
    [SerializeField] private bool _effectApplied;
    public StatusEffectDataBlock DataBlock
    {
        get => _dataBlock;
    }

    private AttributeController _attributeController;

    private void Awake()
    {
        RequestId = GetInstanceID();
    }

    public void CreateRequest(string sourceName, StatusEffectDataBlock dataBlock,
        ICollidible otherCollidible = null)
    {

        //SourceCoC = otherCollidible;
        //SourceObjectId = otherCollidible.CollisionObjectId;
        //_sourceName = sourceName;
        statusEffectType = dataBlock.statusEffectType;
        effectApplicationType = dataBlock.applicationType;
        _limit = dataBlock.limit;
        _threshold = dataBlock.threshold;
        _tickRate = dataBlock.tickRate;
        _dataBlock = dataBlock;
        _attributeController = GetComponent<AttributeController>();
        _dataBlock.attrMods.Clear();
        
        for (var i = 0; i < _dataBlock.attrModReference.Count; i++)
            
        {
            AttributeModDataBlock mod = Instantiate(_dataBlock.attrModReference[i]);
            mod.sourceObject = this;
            _dataBlock.attrMods.Add(mod);
        }

        _CoC = gameObject.GetComponent<ICollidible>();
        var safeToAdd = _CoC.CompareStatusEffectsToList(this);
        if(safeToAdd)
            _CoC.StatusEffectRequests.Add(this);


        switch (effectApplicationType)
        {
            case EffectApplicationType.Permanent:
                PrepEffect();
                GameManager.Instance.OnKillEffectEvent += KillEffect;
                return;
            case EffectApplicationType.Sustained:
                DoEffect();
                PrepEffect();
                break;
            case EffectApplicationType.DoT:
                PrepEffect();
                break;
            case EffectApplicationType.Null:
                Debug.LogWarning("Status Effect Set to NULL. Please Check.");
                break;
        }
        
        _timer = _limit != 0 ? new SimpleTimer(_tickRate, _limit, true) : new SimpleTimer(_tickRate, 0, false);
        _timer.TimerLoopEvent += PrepEffect;
        _timer.TimerCompleteEvent += KillSelf;
        //Debug.Log("Status Request [" + iD + "} Created.");
    }

    void LateUpdate()
    {
        if (_timer != null)
        {
            _timer.Update(Timekeeper.instance.Clock("Actives").fixedDeltaTime);
        }
        
//        CheckSustained();
    }

    void KillEffect(string sourceId)
    {
        if (sourceId == _sourceName)
        {
            KillSelf();
        }
    }
    
    public void KillSelf()
    {
        //Debug.Log("Status Effect Kill Self");
        
        KillAttributeMods();
        EffectRequestManager.Instance.RemoveEffectRequestFromList(_CoC.StatusEffectRequests, this);

        if (_timer != null)
        {
            _timer.TimerLoopEvent -= PrepEffect;
            _timer.TimerCompleteEvent -= KillSelf;
        }

        Destroy(this);
    }

    public void CheckSustained()
    {
        bool isSustained = true; //EffectRequestManager.Instance.CheckForSustainedBounds(_CoC, SourceCoC);
        
        if (!isSustained)
        {
            KillSelf();
            return;
        }
        
        //Debug.Log(SourceCoC.GObject.name + "Effect Sustained: " + iD);
    }
    
     public bool isSustained()
     {
         bool isSustained = true; //EffectRequestManager.Instance.CheckForSustainedBounds(_CoC, SourceCoC);
            
            if (!isSustained)
            {
                return false;
            }
            return true;
            //Debug.Log(SourceCoC.GObject.name + "Effect Sustained: " + iD);
        }
    
    public void PrepEffect()
    {

        switch (effectApplicationType)
        {
            case EffectApplicationType.Velocity:
                //TODO CHECK IF CURRENTLY MOVING FASTER THAN VELOCITY LIMIT
                if (_CoC.GObject.GetComponent<Rigidbody>().velocity.x >= _threshold || -_CoC.GObject.GetComponent<Rigidbody>().velocity.x <= -_threshold)
                    DoEffect();
                break;
            case EffectApplicationType.Sustained:
                if(!isSustained())
                    KillSelf();
                    break;
            default:
                DoEffect();
                break;

        }
    }

    public void DoEffect()
    {
        //Debug.Log("Status Effect Applied");
        ApplyAttributeMods(_dataBlock.attrMods);
    }

    void ApplyAttributeMods(List<AttributeModDataBlock> attrMods)
    {
        foreach (AttributeModDataBlock mod in attrMods)
        {
            mod.sourceRequestId = statusRequestId;
            _attributeController.AddAttributeMod(mod);
        }
    }
    

    public void KillAttributeMods()
    {

        foreach (KeyValuePair<AttributeType, List<AttributeModDataBlock>> modlist in _attributeController.attributeModList.ToList())
        {
            Debug.Log("Running Kill Attribute Check on: " + modlist.Key);
            foreach(AttributeModDataBlock mod in modlist.Value.ToList())
            {
                if (mod.sourceRequestId == statusRequestId)
                {
                    _attributeController.RemoveAttributeMod(mod);
                }
            }
        }
        
    }
}