using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectController : MonoBehaviour
{

	[SerializeField] private AudioSource _audioSource;

	[SerializeField] private AudioClip _audioClip;

	[SerializeField] private GameObject[] hitObjects;

	[SerializeField] private bool destroyOnHit = true;
	// Use this for initialization
	void Start () {
		if (!_audioSource)
		{
			_audioSource = gameObject.GetComponent<AudioSource>();
		}
		
	}

	private void OnCollisionEnter(Collision other)
	{
		if(_audioClip)
			_audioSource.PlayOneShot(_audioClip);
		
		var hitEffect = Instantiate(hitObjects[0], transform.position, Quaternion.identity);
		hitEffect.transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
		
		if(destroyOnHit)
			Destroy(gameObject);
	}
}
