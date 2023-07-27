using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Managers;
using UnityEngine;
using Chronos;

public class PlayerShield : MonoBehaviour {

	public PlayerShip playerShip;
	public float activateDissolveStart = -5f;
	public float activateDissolveEnd = 13f;
	public Renderer shieldRender;
	private Clock clock;


	void Awake() {
		shieldRender = GetComponent<Renderer> ();
		playerShip = GetComponentInParent<PlayerShip> ();
		clock = Timekeeper.instance.Clock("Player");
	}

	// Use this for initialization
	void Start () {
		if(shieldRender.material.HasProperty("_LinearDissolve"))
			shieldRender.material.SetFloat ("_LinearDissolve", activateDissolveStart);
	}


	void OnEnable () {
		if (GameStateManager.Instance.CurrentState.StateType == GameStateType.Gameplay) {
			if (shieldRender.material.HasProperty ("_LinearDissolve"))
				shieldRender.material.SetFloat ("_LinearDissolve", activateDissolveStart);
			StartCoroutine (ActivateShieldEffect ());
		}
	}

	void OnDisable () {
		if(shieldRender.material.HasProperty("_LinearDissolve"))
			shieldRender.material.SetFloat ("_LinearDissolve", activateDissolveStart);
	}

	public IEnumerator ActivateShieldEffect() {

		//SETS THE COLOR OF THE SHIELD
		//Debug.Log(playerShip.absorbColorIndex);
		if(shieldRender.material.HasProperty("_SpecColor"))
			shieldRender.material.SetColor ("_SpecColor", ColorManager.Instance.colorArray[playerShip.boostDefenseColorIndex]);
		if(shieldRender.material.HasProperty("_FresnelColor"))
			shieldRender.material.SetColor ("_FresnelColor", ColorManager.Instance.colorArray [playerShip.boostDefenseColorIndex]);
		if(shieldRender.material.HasProperty("_EdgeColor"))
			shieldRender.material.SetColor ("_EdgeColor", ColorManager.Instance.colorArray [playerShip.boostDefenseColorIndex]);


		//LERPS LINEAR DISSOLVE TO HAVE SHIELD ACTIVATION WHIPE EFFECT
		while (shieldRender.material.GetFloat ("_LinearDissolve") < activateDissolveEnd - 1.5f) {
		//Debug.Log ("[Activating Shield: " + gameObject.name + "] || Current Linear Dissolve: " + shieldRender.material.GetFloat ("_LinearDissolve"));
			float tempNumber = Mathf.Lerp (shieldRender.material.GetFloat ("_LinearDissolve"), activateDissolveEnd, 0.5f * (5 * clock.deltaTime * Timekeeper.instance.Clock ("Player").timeScale) );
			shieldRender.material.SetFloat ("_LinearDissolve", tempNumber);
			yield return null;
		}
		//Debug.Log ("ActivateShieldEffect Finished");
	}
		
}
