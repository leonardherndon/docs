using UnityEngine;
using System.Collections;

public class StickWithPlayer : MonoBehaviour {

	public float playerPositionOffset = 0f;
	public PlayerShip playerShip;

	public void Awake()
	{
		SetTarget();
	}
	void Update () {
		if (playerShip) { 
			if (GameManager.Instance.gameState == GameStateType.Gameplay || GameManager.Instance.gameState == GameStateType.ActiveATG) {
				transform.position = new Vector3 (GameManager.Instance.playerShip.transform.position.x + playerPositionOffset, GameManager.Instance.playerShip.transform.position.y, GameManager.Instance.playerShip.transform.position.z);
			}
		}
	}
	
	public void SetTarget()
	{
		if (!playerShip && GameManager.Instance.gameState == GameStateType.Gameplay) {
			playerShip = GameObject.Find ("PlayerShip").GetComponent<PlayerShip> ();
		}
	}
}
