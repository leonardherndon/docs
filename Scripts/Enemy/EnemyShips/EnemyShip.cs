using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Managers;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class EnemyShip : Hostile {

	//Physics & Movement Variables

	[HideInInspector]
	public Vector3 baseVelocity;
	public float baseSpeedOverride = 2f;
	public float tiltRotatePositive;
	public float tiltRotateNegative;
	public bool isSpeedOverwritten = false;
	public Vector3 overrideVelocity;
	public float shipRotationSpeed = 1f;
	public List<GameObject> shipRenderList;

	public int[] randomNumberQueue;


	protected override void Awake() 
	{
		base.Awake ();

		//For Random Color Generation
		randomNumberQueue = new int[]{0,2,4};

		//Init Ship's Rotation Variables
		tiltRotatePositive = -65f;
		tiltRotateNegative = 65f;
		defaultRot = Quaternion.AngleAxis(0, transform.forward);

		GameManager.Instance.enemyShips.Add (this);
	}

}
