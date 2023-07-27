using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;

public class DebrisField : MonoBehaviour {

	public ParticleSystem debrisField;
	public ProCamera2D camera;
	int newRate;
	int pState;
	int pStateCache;

	private void Start()
	{
		camera = GetComponentInParent<ProCamera2D>();
	}

	void LateUpdate() {
		var em = debrisField.emission;
		pState = 3;
		

		if(pState == pStateCache && pState != 0) {
			return;
		}
		
		if (GameManager.Instance.gameState == GameStateType.Gameplay) {
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[debrisField.particleCount];
			int count = debrisField.GetParticles (particles);
			for (int i = 0; i < count; i++) {
				Vector3 particleVelocity;
				particleVelocity = new Vector3 (-300, 0, 0); 
				particles [i].velocity = particleVelocity;

			}

			debrisField.SetParticles (particles, count);

			if (newRate != 100) {
				newRate = 100;
				em.rateOverTime = newRate;
			}
			
			pStateCache = pState;
		}


		if (GameManager.Instance.gameState == GameStateType.ActiveATG) {
			if (newRate != 2000) {
				newRate = 2000;
				em.rateOverTime = newRate;
			}
		}
	}
		
}
