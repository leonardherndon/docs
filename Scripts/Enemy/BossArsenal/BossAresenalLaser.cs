using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAresenalLaser : MonoBehaviour
{

    [SerializeField] private float _delayTime;

    [SerializeField] private BoxCollider _boxCollider;

    [SerializeField] private ParticleSystem _particleSystem;

    [SerializeField] private bool _collisionActive = false;
    // Start is called before the first frame update
    void Start()
    {
        if(!_boxCollider)
        _boxCollider = GetComponent<BoxCollider>();
        if(!_particleSystem)
        _particleSystem = GetComponent<ParticleSystem>();
        
        StartCoroutine(Deploy());
    }

    public IEnumerator Deploy()
    {
        yield return new WaitForSeconds(_delayTime);
        _boxCollider.enabled = true;

        while (_particleSystem.isPlaying)
        {
            yield return null;
        }
        
        DestroyImmediate(gameObject);
    }

    /*public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.playerShip.CoC.TestCollisionHostile(gameObject.GetComponent<ICollidible>(),gameObject.GetComponent<ICollidible>().DamageApplier,gameObject.GetComponent<ICollidible>().StatusEffectApplier);
        }
    }*/
}
