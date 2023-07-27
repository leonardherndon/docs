using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneTrigger : MonoBehaviour
{
	
	[SerializeField] private int sceneNumber = -1;
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerShip>())
		{
			GameManager.Instance.checkPointIndex = 0;
			GameManager.Instance.GoToGameScene(sceneNumber);
			
		}
	}
}
