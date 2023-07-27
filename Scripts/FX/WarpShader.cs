using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpShader : MonoBehaviour {


	private Camera thisCamera;
	public Shader shaderEffect;

	// Use this for initialization
	void Start () {
		thisCamera = GetComponent<Camera> ();
		thisCamera.RenderWithShader (shaderEffect, "RenderType");
	}

}
