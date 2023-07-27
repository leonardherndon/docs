using UnityEngine;
using System.Collections;
using DG.Tweening;


public enum EntryMethod {SlideFromVertical, SlideFromHorizontal, Instant};
public enum ExitMethod {SlideUp, SlideDown, SlideLeft, SlideRight, Instant};


public class GameUIPanel : MonoBehaviour {

	public Camera UICamera;
	public float xPos;
	public float yPos;

	public EntryMethod entry;
	public ExitMethod exit;

	// Use this for initialization
	void Awake () {

		switch(exit) {
			
		case ExitMethod.SlideUp:
			transform.DOLocalMoveY(679f,0);
			break;
			
		case ExitMethod.SlideDown:
			transform.DOLocalMoveY(-679f,0);
			break;
			
		case ExitMethod.SlideLeft:
			transform.DOLocalMoveX(-1323f,0);
			break;
			
		case ExitMethod.SlideRight:
			transform.DOLocalMoveX(1323f,0);
			break;
		
		case ExitMethod.Instant:
			break;	
		}
	}

	void OnEnable() {
		EnterScene ();
	}
	// Update is called once per frame
	void Update () {
	
	}

	public void EnterScene() {


		switch(entry) {

			case EntryMethod.SlideFromVertical:
				transform.DOLocalMoveY(0,1);
				break;

			case EntryMethod.SlideFromHorizontal:
				transform.DOLocalMoveX (0,1);
				break;

			case EntryMethod.Instant:
				transform.DOLocalMove(new Vector3(0,0,0),0);
				break;
		
		}
	}

	public void ExitScene() {
		
		switch(exit) {
			
		case ExitMethod.SlideUp:
			transform.DOLocalMoveY(+679f,1);
			break;
			
		case ExitMethod.SlideDown:
			transform.DOLocalMoveY(-679f,1);
			break;
			
		case ExitMethod.SlideLeft:
			transform.DOLocalMoveX(-1323f,1);
			break;
			
		case ExitMethod.SlideRight:
			transform.DOLocalMoveX(1323f,1);
			break;
				
		case ExitMethod.Instant:
			gameObject.SetActive(false);
			break;
			
		}
	}
	
}
