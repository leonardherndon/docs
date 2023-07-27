using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FusionTrail : MonoBehaviour {

	private Renderer rend;
	public Gradient myGradient;
	public GradientColorKey[] gck;
	public GradientAlphaKey[] gak;


	// Use this for initialization
	void Start () {

		myGradient = new Gradient();

		// Populate the color keys at the relative time 0 and 1 (0 and 100%)
		gck = new GradientColorKey[2];
		gck[0].color = Color.red;
		gck[0].time = 0.0f;
		gck[1].color = Color.blue;
		gck[1].time = 1.0f;
		
		// Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
		gak = new GradientAlphaKey[2];
		gak[0].alpha = 1.0f;
		gak[0].time = 0.0f;
		gak[1].alpha = 0.0f;
		gak[1].time = 1.0f;

		myGradient.SetKeys (gck, gak);

		rend = gameObject.GetComponent<Renderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		rend.material.DOGradientColor(myGradient,"_TintColor" ,10);
	}
}
