using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChromaShift.Scripts;
using UnityEngine;
using ChromaShift.Scripts.ObjectAttributeSystem;

public class EffectRequestManager : MonoBehaviour
{
    private static EffectRequestManager _instance;
    
    public static EffectRequestManager Instance {
        get 
        {
            if (_instance == null) 
            {
                _instance = FindObjectOfType (typeof(EffectRequestManager)) as EffectRequestManager;


                if (_instance == null)
                {
                    GameObject go = new GameObject("_damageStatusRequestManager");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<EffectRequestManager>();
                }
            }
            return _instance;
        }
    }
    
    public int AddStatusEffectToObject(GameObject targetObject, GameObject originObject, StatusEffectDataBlock sEA = null)
    {
        if (sEA == null)
            return 0;
        var statusEffectRequest = targetObject.AddComponent<StatusEffectRequest>();
        var id = targetObject.GetInstanceID();
        statusEffectRequest.SourceObjectId = id;
        statusEffectRequest.CreateRequest(originObject.name, sEA);

        return statusEffectRequest.RequestId;
    }
    
    public bool CurrentlyHasEffect(List<StatusEffectRequest> list, StatusEffectType type)
    {
        foreach (StatusEffectRequest item in list.ToList())
        {
            if (item.DataBlock.statusEffectType == type)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveEffectRequestFromList(List<StatusEffectRequest> list, StatusEffectRequest effectRequest)
    {
        foreach (StatusEffectRequest item in list.ToList())
        {
            if (item.RequestId == effectRequest.RequestId)
            {
                list.Remove(item);
            }
        }
    }
    
    public void KillEffectRequestByID(List<StatusEffectRequest> list, int requestID)
    {
        foreach (StatusEffectRequest item in list.ToList())
        {
            if (item.RequestId == requestID)
            {
                item.KillSelf();
            }
        }
    }
    
    public void KillAllEffectRequest(List<StatusEffectRequest> list)
    {
        foreach (StatusEffectRequest item in list.ToList())
        {
            item.KillSelf();
        }
    }
    
    public void RemoveEffectRequestFromListByID(List<StatusEffectRequest> list, int requestID)
    {
        foreach (StatusEffectRequest item in list.ToList())
        {
            if (item.RequestId == requestID)
            {
                list.Remove(item);
            }
        }
    }
    
    public void RemoveDamageRequestFromList(List<DamageRequest> list, DamageRequest damageRequest)
    {
        foreach (DamageRequest item in list.ToList())
        {
            if (item.RequestId == damageRequest.RequestId)
            {
                list.Remove(item);
            }
        }
    }
    public bool CheckForSustainedBounds(ICollidible objectA, ICollidible objectB)
    {
        //TODO FIND A BETTER WAY TO ATTACH GAME OBJECT THAN USING CSM MONOBEHAVIOUR
        var thisCollider = objectA.ObjectCollider;
        var otherCollider = objectB.ObjectCollider;
        
        if(thisCollider.GetType() == typeof(MeshCollider) || otherCollider.GetType() == typeof(MeshCollider))
        {
            Debug.Log("Mesh Collider situation");
            if (!otherCollider.bounds.Contains(thisCollider.transform.position)) return false;
            return true;
        }
        
        if (!thisCollider.bounds.Intersects(otherCollider.bounds)) return false; 
            return true;
    }
}
