using System;
using System.Collections;
using System.Collections.Generic;
using uGUIPanelManager;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneTrigger : MonoBehaviour
{
	[SerializeField] private GameObject cutScene;
	public PlayableDirector director;

	void OnTriggerEnter(Collider other)
	{
		director = cutScene.GetComponent<PlayableDirector>();
		//Debug.Log ("CuTrigger Collision Happened with: " + other.gameObject.tag);
		if (other.gameObject.CompareTag("Player"))
		{
			director.Play();
		}
	}

	

}
