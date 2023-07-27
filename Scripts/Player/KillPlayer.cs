using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{

	[SerializeField] private float delay;
	[SerializeField] private ImpactType type;

	public void KillPlayerWrap()
	{
		StartCoroutine(DestroyPlayer(delay,type));
	}

	public IEnumerator DestroyPlayer(float delay = 0, ImpactType type = ImpactType.Energy)
	{
		yield return new WaitForSeconds(delay);
		
		GameManager.Instance.playerShip.LS.DrainLife(GameManager.Instance.playerShip.LS.TotalHP,ImpactType.Special);
	}
}
