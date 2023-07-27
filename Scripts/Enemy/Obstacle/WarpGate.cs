using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;

public class WarpGate : MonoBehaviour {

	
	public static WarpGate instance;
	public float gateOffset;
	public int[] GateCoreReq = new int[2]; //Fusion Core Color Indexes
	public int[] availableCoreColors = new int[3]{0,2,4}; //Red, Blue, Yellow
	public float currentSuccessChance; 
	public List<GameObject> gateParts = new List<GameObject>();
	public const float GATECHARGEAMOUNT = 6.25f;
	public const float MAXPERCENT = 100f;
	public float timeToGate = 10f;
	public float timeToGateRemaining;
	public float enterGateSpeed = 30f;
//	public FusionCoreInventoryManager FCIM;
	public Text gateCores;
	public Text succesRate;
	public bool isPositionLocked = true;
	float gatePosition1;
	float gatePosition2;
	//public CameraFilterPack_Color_Chromatic_Plus ATG_warpFX;
	public ProCamera2D mainCamera;
//	public GameObject blackOutPanel;

	public void Awake () {
		instance = this;
	}
	// Use this for initialization
	void Start () {
		
		//gateCores = GameUIManager.Instance.UIElements[11].GetComponent<Text>();
		//succesRate = GameUIManager.Instance.UIElements[13].GetComponent<Text>();
		//ATG_warpFX = Camera.main.GetComponent<CameraFilterPack_Color_Chromatic_Plus> ();
		mainCamera = GameManager.Instance.mainCamera;
		GenerateGate ();
	}
	
	// Update is called once per frame
	void Update () {
		timeToGateRemaining -= Time.deltaTime;
	}

	void FixedUpdate() {

		gatePosition1 = ( (LaneManager.Instance.laneArray [4].y - LaneManager.Instance.laneArray [6].y) * (Mathf.Abs(currentSuccessChance-100)/100) ) + LaneManager.Instance.laneArray [6].y;
		gatePosition2 = ( (LaneManager.Instance.laneArray [8].y - LaneManager.Instance.laneArray [6].y) * (Mathf.Abs(currentSuccessChance-100)/100) ) + LaneManager.Instance.laneArray [6].y;
		//ATG_warpFX.Offset  = -((Mathf.Abs(currentSuccessChance-100f) / 100f) * 0.1f);
		//Debug.Log ("GatePos1: " + gatePosition1);
		//Debug.Log ("GatePos2: " + gatePosition2);
		if (isPositionLocked)
			transform.position = new Vector3 (GameManager.Instance.playerShip.transform.position.x + gateOffset, LaneManager.Instance.laneArray[6].y, 0f);

		gateParts [0].transform.position = transform.position;
//		gateParts[0].transform.DOMove(new Vector3 (transform.position.x, transform.position.x, 0f),2f);
//		gateParts[0].transform.DOMove(new Vector3 (transform.position.x, gatePosition1, 0f),2f);
//		gateParts[1].transform.DOMove(new Vector3 (transform.position.x, gatePosition2, 0f),2f);


		if (currentSuccessChance > MAXPERCENT) {
			currentSuccessChance = MAXPERCENT;
		}

		if ((timeToGateRemaining <= 0 || currentSuccessChance >= 100))
			StartCoroutine(SetGateLock (false));
	}


	public void GenerateGate (){
		for (int i = 0; i < GateCoreReq.Length; i++) {
			timeToGateRemaining = timeToGate;
			GateCoreReq [i] = availableCoreColors [Random.Range (0, availableCoreColors.Length)];
		}
//		blackOutPanel.SetActive (true);
	}
	public IEnumerator SetGateLock(bool toggle) {
		yield return new WaitForSeconds(0.5f);
		isPositionLocked = toggle;
//		mainCamera.RemoveCameraTarget (GameManager.Instance.playerShip.hoverCollider);
//		mainCamera.AdjustCameraTargetInfluence (GameManager.Instance.playerShip.transform, 1f, 1, 0);
		GameManager.Instance.playerShip.speedOverride = enterGateSpeed;
		GameManager.Instance.playerShip.isSpeedOverwritten = true;
	}

	public IEnumerator OnApproach() {

		//Debug.Log ("Start Approach Delay: " + Time.time);
		yield return new WaitForSeconds(2.0f);
		//Debug.Log ("End Approach Delay: " + Time.time);

//		FCIM = GameManager.Instance.playerShip.FCIM;

		//Debug.Log ("On Approach to Gate");

		//CalculateWarpSucceessChance();

		currentSuccessChance = 0;

		foreach (Transform childTransform in transform)
		{
			childTransform.gameObject.SetActive(true);

		}
		gateParts[0].transform.position = new Vector3 (gatePosition1, 0f, transform.position.z);
		gateParts[1].transform.position = new Vector3 (gatePosition2, 0f, transform.position.z);
	}

	public void ChargeGate(){
		if (currentSuccessChance < MAXPERCENT) {
			//Debug.Log ("Charge Gate");
			//if (currentSuccessChance != null) {
				//Debug.Log ("Gate Position (ABS): " + gatePosition2);
			//}
			currentSuccessChance = currentSuccessChance + GATECHARGEAMOUNT;
		}
	}
	
}
