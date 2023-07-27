using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


/// <summary>
/// Lane warning icon.
/// Having an issue where only the first instance works. All others do not appear.
/// </summary>


public class LaneWarningIcon : MonoBehaviour {

	public int laneID;

	// Use this for initialization
	void Start () {
		//Debug.Log ("Lane Warning Icon Spawned");
	}

	public void Pulse() {
		//Debug.Log("Pulsing: " + gameObject.name);
		transform.DOScale (1.5f, 0.25f).SetLoops (5, LoopType.Yoyo).OnComplete(Hide);

	}

	void Hide() {
		//Debug.Log("Hiding: " + gameObject.name);
		gameObject.SetActive (false);
	}
}
