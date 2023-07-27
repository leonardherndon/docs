using UnityEngine;
using System.Collections;

public class TileBrush : MonoBehaviour {

	public Vector3 brushSize = Vector3.zero;
	public int tileID = 0;
	public SpriteRenderer renderer2D;

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position, brushSize);
	}

	public void UpdateBrush(Sprite sprite) {
		renderer2D.sprite = sprite;
	}
}
