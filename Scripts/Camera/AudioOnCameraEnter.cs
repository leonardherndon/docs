using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class AudioOnCameraEnter : MonoBehaviour
{


	[SerializeField] private bool _isVisible;
	[SerializeField] private bool _triggered;
	[SerializeField] private Bounds _theBounds;

	[SerializeField] private AudioSource _as;
	// Use this for initialization
	void Start () {
		_theBounds = gameObject.GetComponent<Collider>().bounds;
		_as = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (_triggered)
			return;
		
		if (!_isVisible)
		{
			GameManager.Instance.cameraPlanes = GeometryUtility.CalculateFrustumPlanes(GameManager.Instance.mainCamera.GameCamera);
			_isVisible = GeometryUtility.TestPlanesAABB(GameManager.Instance.cameraPlanes, _theBounds);
		}
		else
		{
			_triggered = true;
			_as.PlayOneShot(_as.clip);	
		}
		
		
	}
}
