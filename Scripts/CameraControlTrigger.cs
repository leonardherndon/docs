using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraControlTrigger : MonoBehaviour
{
    
    [Header("[CAMERA ZOOM")] 
    [SerializeField] private bool adjustCameraZoomToggle;
    public float cameraZoomDistance = 65;
    public float cameraZoomSpeed = 1f;
    
void OnTriggerEnter(Collider other) {
		
	if (other.name != "PlayerShip") return;
	Debug.Log ("Player Control Trigger | Collision Detected with " + other.name);

	if (adjustCameraZoomToggle)
	{
		GameManager.Instance.mainCamera.transform.DOMoveZ(-cameraZoomDistance, cameraZoomSpeed);
	}
		
	}
}
