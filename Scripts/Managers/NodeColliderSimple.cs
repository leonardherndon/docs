using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Player;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Serialization;



public class NodeColliderSimple : MonoBehaviour, ICollidible
{

    [Header("COLLISION")] 
    
    [SerializeField] private bool _isPlayerShip;

    public bool IsPlayerShip
    {
        get => _isPlayerShip;
        set => _isPlayerShip = value;
    }
    [SerializeField] private bool _doesNotReceiveCollision = false;
    [SerializeField] private int collisionObjectId;

    [SerializeField] private bool _isLightNode;

    public bool IsLightNode
    {
        get => _isLightNode;
        set => _isLightNode = value;
    }

    public int CollisionObjectId
    {
        get => collisionObjectId;
        set => collisionObjectId = value;
    }


    [Header("DEPENDENCIES")] [SerializeField]
    private GameObject _gObject;

    public GameObject GObject
    {
        get => _gObject;
        set => _gObject = value;
    }

    [SerializeField] private ChromaShiftManager _cSM;

    public ChromaShiftManager CSM
    {
        get => _cSM;
        set => _cSM = value;
    }
    
    [SerializeField] private Collider _objectCollider;
    public Collider ObjectCollider
    {
        get => _objectCollider;
        set => _objectCollider = value;
    }

    [SerializeField] private ILifeSystem _lifeSystem;

    public ILifeSystem LifeSystem
    {
        get => _lifeSystem;
        set => _lifeSystem = value;
    }


    [Header("COLOR")] public bool _hueActive = true;
    [FormerlySerializedAs("collisionColorMatchType")] [FormerlySerializedAs("_colorMatchType")] [SerializeField] private ColorMatchType _collisionColorMatchType;
    public ColorMatchType CollisionColorMatchType
    {
        get => _collisionColorMatchType;
        set => _collisionColorMatchType = value;
    }

    [Header("THREAT")] public bool _doesEarlyWarning;
    [SerializeField] private ObjectTag _objectTag;

    public ObjectTag ObjectTag
    {
        get => _objectTag;
        set => _objectTag = value;
    }

    [SerializeField] private ThreatGroup _threatGroup;

    public ThreatGroup ThreatGroup
    {
        get => _threatGroup;
        set => _threatGroup = value;
    }

    [SerializeField] private FriendlyType _friendlyType;

    public FriendlyType FriendlyType
    {
        get => _friendlyType;
        set => _friendlyType = value;
    }


    [Header("DAMAGE")] [SerializeField] private bool _doesVelocityDamage = false;

    public bool DoesVelocityDamage
    {
        get => _doesVelocityDamage;
        set => _doesVelocityDamage = value;
    }

    [FormerlySerializedAs("_collisionDeathThreshold")] [SerializeField]
    private Vector2 _velocityDeathThreshold = new Vector2(25f, 35f);

    [SerializeField] private DamageGroup _damageGroup;

    public DamageGroup DamageGroup
    {
        get => _damageGroup;
        set => _damageGroup = value;
    }

    [SerializeField] private ImpactType _impactType;

    public ImpactType ImpactType
    {
        get => _impactType;
        set => _impactType = value;
    }

    [FormerlySerializedAs("_immunityType")]
    public DamageImmunityType _damageImmunityType;

    [SerializeField] private List<DamageDataBlock> damageDataBlocks;

    public List<DamageDataBlock> DamageDataBlocks
    {
        get => damageDataBlocks;
        set => damageDataBlocks = value;
    }

    [SerializeField] private List<DamageRequest> _damageRequests;

    public List<DamageRequest> DamageRequests
    {
        get => _damageRequests;
        set => _damageRequests = value;
    }

    [Header("STATUS EFFECTS")] [SerializeField]
    private bool _statusEffectImmunity = false;

    public bool StatusEffectImmunity
    {
        get => _statusEffectImmunity;
        set => _statusEffectImmunity = value;
    }

    [FormerlySerializedAs("_statusEffectImmunityType")] [SerializeField]
    private StatusEffectType _statusEffectImmunityType;

    public StatusEffectType StatusEffectImmunityType
    {
        get => _statusEffectImmunityType;
        set => _statusEffectImmunityType = value;
    }

    [SerializeField] private List<StatusEffectDataBlock> statusEffectBlocks;

    public List<StatusEffectDataBlock> StatusEffectDataBlocks
    {
        get => statusEffectBlocks;
        set => statusEffectBlocks = value;
    }

    [SerializeField] private List<StatusEffectRequest> _statusEffectRequests;

    public List<StatusEffectRequest> StatusEffectRequests
    {
        get => _statusEffectRequests;
        set => _statusEffectRequests = value;
    }

    //EVENTS

    public event OnEventLostRedCore OnLostRedCore = delegate { };
    public event OnEventLostGreenCore OnLostGreenCore = delegate { };
    public event OnEventLostBlueCore OnLostBlueCore = delegate { };
    public event OnEventGainedRedCore OnGainedRedCore = delegate { };
    public event OnEventGainedGreenCore OnGainedGreenCore = delegate { };
    public event OnEventGainedBlueCore OnGainedBlueCore = delegate { };
    public event OnEventLumenTransferLight OnLumenTransferLight = delegate { };
    public event OnEventLumenTransferDark OnLumenTransferDark = delegate { };
    public event ICollidible.DamageHandler GotDamaged;
    public event ICollidible.VelocityDamageHandler GotVelocityDamaged;
    public event ICollidible.WreckedHandler Wrecked;
    
    void Awake()
    {
        collisionObjectId = GetInstanceID();
    }
    void Start ()
    {
        InitCollisionController();
    }

    public void InitCollisionController()
    {
        _gObject = gameObject;
        _cSM = gameObject.GetComponent<ChromaShiftManager>();
        _lifeSystem = gameObject.GetComponent<ILifeSystem>();
        EffectRequestManager.Instance.KillAllEffectRequest(_statusEffectRequests);
        
        if (_lifeSystem == null)
            gameObject.GetComponent<ILifeSystem>();
        if (damageDataBlocks.Count == 0)
            damageDataBlocks.Add(Resources.Load<DamageDataBlock>("DamageStatusAppliers/da_null"));
        if (statusEffectBlocks.Count == 0)
            statusEffectBlocks.Add(Resources.Load<StatusEffectDataBlock>("DamageStatusAppliers/sa_null"));
        _statusEffectRequests = new List<StatusEffectRequest>();
        _damageRequests = new List<DamageRequest>();
        
        CheckCoreInventory();
    } 
    
    public void CheckCoreInventory()
    {
        if (_cSM.CheckForCore(GameColor.Red, CSM.CoreInventory))
            OnGainedRedCore();
        else 
            OnLostRedCore();
			
        if (_cSM.CheckForCore(GameColor.Green, CSM.CoreInventory))
            OnGainedGreenCore();
        else 
            OnLostGreenCore();
			
        if (_cSM.CheckForCore(GameColor.Blue, CSM.CoreInventory))
            OnGainedBlueCore();
        else 
            OnLostBlueCore();
    }
    
    
   public void OnCollisionEnter(Collision other)
    {
        //Debug.Log("No Physical Collision is used with Node Colliders. Please use OnTrigger");
        return;

        var collideObject = other.gameObject.GetComponent<ICollidible>();
        if (collideObject == null)
        {
            Debug.LogWarning("No Collidible attached to: "+ other.gameObject.name);
            return;
        }
        
        CalcCollision(collideObject, other);
    }
   
   public void OnTriggerEnter(Collider other)
   {
       //Debug.Log("Node Object Trigger: " + gameObject.name + " | " + other.gameObject.name);
       
       var collideObject = other.gameObject.GetComponent<ICollidible>();
       if (collideObject == null)
       {
           Debug.LogWarning("No Collidible attached to: " + other.gameObject.name);
           return;
       }

       CalcCollision(collideObject);
   }

   public void CalcCollision(ICollidible colliderObject, Collision otherCollision = null)
   {
      
       if (_doesNotReceiveCollision)
       {
           //Debug.Log(gameObject.name + "This GameObject does not receive damage.");
           return;
       }

       HandleCollision(colliderObject.GObject);

   }

   public void HandleCollision(GameObject other, List<DamageDataBlock> damageBlockList = null, List<StatusEffectDataBlock> sEA = null)
   {
       //Debug.Log(gameObject.name +" is testing Collision Handle Hostile: [other] " + other.name);
       var otherCoC = other.GetComponent<CollisionController>();

       switch (otherCoC.CSM.LumenMode)
       {
           case LumenMode.Light:
               SetMatchingCores(otherCoC);
               break;
           case LumenMode.Dark:
               RemoveAllCores();
               break;
       }
       
       
       CSM.ChromaShift(CSM.SetColorFromActiveCores());
       //Play Special FX
       HueManager.Instance.Flash(0, 2, otherCoC.CSM.CurrentColor);
       
   }

   public void ReceiveStatusEffect(List<StatusEffectDataBlock> StatusBlockList = null, ICollidible otherCoC = null, string sourceName = null)
   {
      
   }
   
   
   public bool CompareStatusEffectsToList(StatusEffectRequest newRequest)
   {
       return false;
   }
   
   public bool CompareStatusEffectsToList(StatusEffectDataBlock block)
   {
       return false;
   }


   public void SetMatchingCores(ICollidible other)
   {
       //Debug.Log(gameObject.name + "Add Matching Cores");
       FusionCore[] cores = GameManager.Instance.EMPTYFC;
       cores = other.CSM.GetActiveCoresFromColor(other.CSM.CurrentColor);
       CSM.CoreInventory = cores;
       OnLumenTransferLight();
       
   }
   
   public void RemoveAllCores()
   {
       //Debug.Log(gameObject.name + "Add Matching Cores");
       FusionCore[] cores = GameManager.Instance.EMPTYFC;
       CSM.CoreInventory = cores;
       OnLumenTransferDark();
   }

   private void OnTriggerExit(Collider other)
    {
        if (_statusEffectRequests.Count <= 0)
            return;
        
        foreach (StatusEffectRequest request in _statusEffectRequests)
        {
            if (request.SourceCoC == other.GetComponent<CollisionController>())
            {
                request.CheckSustained();
            }
        }
    }
    
   public void GetWrecked()
   {
       Wrecked?.Invoke();
   }
}
