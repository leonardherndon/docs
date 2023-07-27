using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public List<StatusEffectDataBlock> dataBlockList = new List<StatusEffectDataBlock>();
    public PlayerShip playerShip;

    void Awake()
    {
        playerShip = GameManager.Instance.playerShip;
    }

    private void Start()
    {
        ApplyGlobalStatusEffects();
    }

    public void ApplyGlobalStatusEffects()
    {
        foreach (StatusEffectDataBlock block in dataBlockList)
        {
        
            var effect = playerShip.gameObject.AddComponent<StatusEffectRequest>();
            //playerShip.CoC.StatusEffectRequests.Add(effect);
            effect.SourceObjectId = GetInstanceID();
            effect.CreateRequest(gameObject.name,block,playerShip.GetComponent<CollisionController>());
            //Debug.Log("Request Created by: " + gameObject.name);
        }
    }
}
