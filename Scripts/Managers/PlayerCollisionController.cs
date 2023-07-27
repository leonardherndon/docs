using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Managers;
using ChromaShift.Scripts.Player;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Serialization;


public class PlayerCollisionController : MonoBehaviour, ICollidible
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
    
    
    [Header ("DEPENDENCIES")]
    [SerializeField] private GameObject _gObject;
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
    

    [Header("COLOR")]
    public bool _hueActive = true;

    [FormerlySerializedAs("collisionColorMatchType")] [FormerlySerializedAs("_colorMatchType")] [SerializeField] private ColorMatchType _collisionColorMatchType;
    public ColorMatchType CollisionColorMatchType
    {
        get => _collisionColorMatchType;
        set => _collisionColorMatchType = value;
    }

    
    [Header ("THREAT")]
    public bool _doesEarlyWarning;
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
    

    [Header ("DAMAGE")]
    [SerializeField] private bool _doesVelocityDamage = false;

    public bool DoesVelocityDamage
    {
        get => _doesVelocityDamage;
        set => _doesVelocityDamage = value;
    }
    [FormerlySerializedAs("_collisionDeathThreshold")] [SerializeField] private Vector2 _velocityDeathThreshold = new Vector2(25f,35f);
    
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

    [FormerlySerializedAs("_immunityType")] public DamageImmunityType _damageImmunityType;
    
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
    
    [FormerlySerializedAs("_statusEffectImmunity")]
    [Header ("STATUS EFFECTS")]
    [SerializeField] private bool _statusEffectImmunityAll = false;
    public bool StatusEffectImmunity
    {
        get => _statusEffectImmunityAll;
        set => _statusEffectImmunityAll = value;
    }
    
    [FormerlySerializedAs("_statusEffectImmunityType")] [SerializeField] private StatusEffectType _statusEffectImmunityType;
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
        //Debug.Log("Object Collision: " + gameObject.name + " | " + other.gameObject.name);
        
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
       //Debug.Log("Object Trigger: " + gameObject.name + " | " + other.gameObject.name);
       
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
       var dA = colliderObject.DamageDataBlocks;
       var sEA = colliderObject.StatusEffectDataBlocks;

       if (_doesNotReceiveCollision)
       {
           Debug.Log(gameObject.name + "This GameObject does not receive damage.");
           return;
       }
       
       switch (_objectTag)
       {
           case ObjectTag.Pickup:
               if (colliderObject.ThreatGroup == ThreatGroup.Player)
               {
                   HandleCollisionFriendly(colliderObject.GObject, sEA);
                   return;
               }
               break;
           default:
               break;
       }
       
       if (colliderObject.DoesVelocityDamage)
       {
           if(_damageImmunityType == DamageImmunityType.Velocity)
               return;
           var velMultiplier = CalculateVelocityImpact(otherCollision, colliderObject);
           _lifeSystem.ReceiveVelocityDamage(otherCollision, colliderObject.ImpactType, dA, colliderObject, velMultiplier);
           return;
       }
       
       switch (colliderObject.ThreatGroup)
       {
           case ThreatGroup.Friendly:
               HandleCollisionFriendly(colliderObject.GObject, sEA);
               break;
           case ThreatGroup.Hostile:
               HandleCollision(colliderObject.GObject, dA, sEA);
               break;
           case ThreatGroup.Neutral:
               HandleCollision(colliderObject.GObject, dA, sEA);
               break;
           case ThreatGroup.Player:
               if (_threatGroup == ThreatGroup.Friendly)
               {
                   HandleCollisionFriendly(colliderObject.GObject, sEA);
                   break;
               }

               if(_threatGroup != ThreatGroup.Player) 
                   HandleCollision(colliderObject.GObject, dA, sEA);
               break;
           default:
               return;
       }
   }

   public void HandleCollisionFriendly(GameObject other, List<StatusEffectDataBlock> sEA)
   {
       var otherCoC = other.GetComponent<CollisionController>();
       
       switch (otherCoC.FriendlyType)
        {
            case FriendlyType.FusionCore:
                
                var coreIndex = otherCoC.CSM.CurrentColor;
                
                HueManager.Instance.Flash(0, 2, coreIndex);
                
                //Check to see if you already have that core.
                for (int x = 0; x < CSM.CoreInventory.Length; x++)
                {
                    if (CSM.CoreInventory[x].ColorIndex == coreIndex && CSM.CoreInventory[x].IsActive == true)
                    {
                        otherCoC.GetWrecked();
                        return;
                    }
                }

                switch (coreIndex)
                {
                    case GameColor.Red:
                        CSM.CoreInventory[0].IsActive = true;
                        break;
                    case GameColor.Green:
                        CSM.CoreInventory[1].IsActive = true;
                        break;
                    case GameColor.Blue:
                        CSM.CoreInventory[2].IsActive = true;
                        break;
                }
                
                otherCoC.GetWrecked();

                break;
            case FriendlyType.Field:
                //HandleCollisionFriendly(other, FriendlyType.Field);
                break;
            case FriendlyType.Gate:
                other.gameObject.GetComponent<Collider>().enabled = false;
                //StartCoroutine (GameManager.Instance.DelayGame (2));
                GameStateManager.Instance.SwitchState(GameStateManager.Instance.AreaCompleteState);
                other.gameObject.GetComponent<AudioSource>().Play();
                break;
        }
   }

   public void HandleCollision(GameObject other, List<DamageDataBlock> damageBlockList, List<StatusEffectDataBlock> sEA = null)
   {
       Debug.Log(gameObject.name +" is testing Collision Handle Hostile: [other] " + other.name);
       var otherCoC = other.GetComponent<CollisionController>();
       int otherCoreCount = otherCoC.CSM.CoreInventory.Length;
       var otherDamageGroup = otherCoC.DamageGroup;
       bool passedCollisionTest = CollisionColorTest(other);
       string sourceName = otherCoC.gameObject.name;
       
       if (passedCollisionTest)
       {
           Debug.Log(gameObject.name + ": Passed Collision Color Test.");
           switch (otherDamageGroup)
           {
               case DamageGroup.Physical:
                   if(_damageImmunityType == DamageImmunityType.Physical)
                       return;
                   if (EffectRequestManager.Instance.CurrentlyHasEffect(_statusEffectRequests,
                       StatusEffectType.Shielded))
                   {
                       GotDamaged?.Invoke(otherCoC.ImpactType, damageBlockList, otherCoC, DamageSeverity.Dampened);
                       break;
                   }
                   
                   if (!RemoveMatchingCores(otherCoreCount, otherCoC))
                   {
                       GotDamaged?.Invoke(otherCoC.ImpactType, damageBlockList, otherCoC);
                   }
                   else
                       GotDamaged?.Invoke(otherCoC.ImpactType, damageBlockList, otherCoC, DamageSeverity.Dampened);
                   break;
               case DamageGroup.Deflectable:
                   return;
                   //if(_immunityType == DamageImmunityType.Deflectable)
                   //return;
                   //GotDamaged?.Invoke(otherCoC.ImpactType, dA, otherCoC, DamageSeverity.Dampened);
                   //ReceiveStatusEffect(sEA, otherCoC);
                   break;
               case DamageGroup.Key:
                   //DOES NO DAMAGE
                   RemoveMatchingCores(otherCoreCount, otherCoC);
                   break;
               case DamageGroup.Death:
                   //DOES NO DAMAGE
                   break;
               case DamageGroup.Null:
                   //DOES NO DAMAGE
                   break;
           }
       }
       else //FAILING COLOR CHECK
       {
           Debug.Log(gameObject.name + ": Failed Collision Color Test.");
           Debug.Log(gameObject.name + ": Object Immunity:" + _damageImmunityType);
           switch (otherDamageGroup)
           {
               case DamageGroup.Physical:
                   if(_damageImmunityType == DamageImmunityType.Physical || _damageImmunityType == DamageImmunityType.All)
                       return;
                   if (!RemoveMatchingCores(otherCoreCount, otherCoC))
                       GotDamaged?.Invoke(otherCoC.ImpactType, damageBlockList, otherCoC, DamageSeverity.Amplified);
                   else
                       GotDamaged?.Invoke(otherCoC.ImpactType, damageBlockList, otherCoC, DamageSeverity.Normal);
                   if(!_statusEffectImmunityAll)
                        ReceiveStatusEffect(sEA, otherCoC, sourceName);
                   break;
               case DamageGroup.Deflectable:
                   if(_damageImmunityType == DamageImmunityType.Deflectable || _damageImmunityType == DamageImmunityType.All)
                       return;
                   if(!_statusEffectImmunityAll)
                        ReceiveStatusEffect(sEA, otherCoC, sourceName);
                   RemoveMatchingCores(otherCoreCount, otherCoC);
                   break;
                   //Removed some Zombie code related to killing the player if they didn't have enough matching cores.
               case DamageGroup.Key:
                   if(_damageImmunityType == DamageImmunityType.Key || _damageImmunityType == DamageImmunityType.All)
                       return;
                   Wrecked?.Invoke();
                   break;
               case DamageGroup.Death:
                   if (_damageImmunityType == DamageImmunityType.Death || _damageImmunityType == DamageImmunityType.All)
                   {
                       //Debug.Log("Object is immune to death.");
                       return;
                   }
                   if(_lifeSystem == null)
                       return;
                   Wrecked?.Invoke();
                   break;
               case DamageGroup.Null:
                   if(!_statusEffectImmunityAll)
                        ReceiveStatusEffect(sEA, otherCoC, sourceName);
                   break;
           }
            
       }

       //Play Special FX
       HueManager.Instance.Flash(0, 2, otherCoC.CSM.CurrentColor);
       CheckCoreInventory();
   }
   
   public float  CalculateVelocityImpact(Collision other, ICollidible otherCoC)
   {
       if (_damageImmunityType == DamageImmunityType.Velocity)
       {
           Debug.Log("Immune To Wall Damage. Ignore Damage");
           return 0;
       }

       var xVel = Mathf.Abs(other.contacts[0].normal.x * other.relativeVelocity.x);
       var yVel = Mathf.Abs(other.contacts[0].normal.y * other.relativeVelocity.y);
       var xVelThreshold = Mathf.Abs(other.contacts[0].normal.x * _velocityDeathThreshold.x);
       xVelThreshold = Mathf.Clamp(xVelThreshold, 1,_velocityDeathThreshold.x );
       var yVelThreshold = Mathf.Abs(other.contacts[0].normal.y * _velocityDeathThreshold.y);
       yVelThreshold = Mathf.Clamp(yVelThreshold, 1,_velocityDeathThreshold.y);
       var velAvg = (xVel + yVel) / 2f;
       var theshAvg = (xVelThreshold + yVelThreshold) / 2f;
               
       float velMultiplier = velAvg / theshAvg;
       if (velMultiplier < 0.4f)
           velMultiplier = 0.05f;
       return velMultiplier;
   }
   
   public void ReceiveStatusEffect(List<StatusEffectDataBlock> StatusBlockList = null, ICollidible otherCoC = null, string sourceName = null)
   {
       foreach (StatusEffectDataBlock statusBlock in StatusBlockList)
       {
           Debug.Log(gameObject.name + ": Receive Status Effect.");

           var id = 0;
           if (_statusEffectImmunityAll)
               return;

           
           if (!CompareStatusEffectsToList(statusBlock))
           {
               Debug.Log(statusBlock.statusEffectType.ToString() + " effect already exists on this object. return.");
               return;
           }
           
           if (otherCoC == null)
           {
               Debug.LogWarning("Other Object does not have a ICollidible. Please Confirm this is correct");
               return;
           }
           
           var statusEffectRequest = gameObject.AddComponent<StatusEffectRequest>();

           if (sourceName == null)
               sourceName = "NULL";

           statusEffectRequest.CreateRequest(sourceName, statusBlock, otherCoC);
       }
   }
   
   
   public bool CompareStatusEffectsToList(StatusEffectDataBlock block)
   {
       foreach (StatusEffectRequest currentRequest in StatusEffectRequests)
       {
           //Debug.Log(currentRequest.ID + " | " + newRequest.ID);
           if (currentRequest.SourceObjectId == collisionObjectId)
           {
               //Debug.Log("Effect Already in List. Returning False.");
               return false;
           }
           
           if (currentRequest.StatusEffectType == block.statusEffectType)
           {
               return false;
           }
       }

       //.Log("Safe To Add Status Effect: " + newRequest.ID);
       return true;
   }
   
   public bool CompareStatusEffectsToList(StatusEffectRequest newRequest)
   {
       foreach (StatusEffectRequest currentRequest in StatusEffectRequests)
       {
           //Debug.Log(currentRequest.ID + " | " + newRequest.ID);
           if (currentRequest.SourceObjectId == collisionObjectId)
           {
               //Debug.Log("Effect Already in List. Returning False.");
               return false;
           }
           
           if (currentRequest.StatusEffectType == newRequest.StatusEffectType)
           {
               return false;
           }
       }

       //.Log("Safe To Add Status Effect: " + newRequest.ID);
       return true;
   }


   private bool CollisionColorTest(GameObject other)
   {
       var otherCoC = other.GetComponent<CollisionController>();

       switch (otherCoC.CollisionColorMatchType)
       {
           case ColorMatchType.Normal:
               return ColorManager.Instance.isColorStrongerOrEqual(gameObject, other);
               break;
           case ColorMatchType.Negative:
               return ColorManager.Instance.isColorWeaker(gameObject,other);
               break;
           case ColorMatchType.ExactMatch:  //hard color pass
               return ColorManager.Instance.isColorEqual(gameObject,other);
               break;
           default:
               return false;
               break;
       }
       return false;
   }
   
    public bool RemoveMatchingCores(int tempCoreCount, ICollidible other)
    {

        foreach (var core in other.CSM.CoreInventory)
        {
            if (other.CSM.CoreInventory.Length == 0)
            {
                return true;
            }
            
            if (CSM.CoreInventory.Length <= 0)
            {
                return false;
            }
            
            var isUnmatchedCore = true;

            for (int i = 0; i < CSM.CoreInventory.Length; i++)
            {
                if (CSM.CoreInventory[i].ColorIndex == core.ColorIndex)
                {
                    if (core.IsActive)
                    {
                        Debug.Log("Attempt to remove core.");
                        // CSM.CoreInventory[i].IsActive = false;
                        CSM.DeactivateFusionCoreByColorIndex(ColorManager.Instance.ConvertIndexToGameColor(i));
                        isUnmatchedCore = false;
                    }
                }
            }

            if (isUnmatchedCore)
            {
                return false;
            }
        }

        return true;
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
