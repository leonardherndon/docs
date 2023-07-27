using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class HeadlineTextTrigger : MonoBehaviour {

	public PlayerShip playerShip;
	public Image displayBox;
	public TypeOutScript headlineText;
	public string displayText; 
	private int delayTime = 5;

	void OnTriggerEnter(Collider other) {

		displayText = GameManager.Instance.currentLevelDataObject.Code+"\n"+GameManager.Instance.currentLevelDataObject.levelSceneList[GameManager.Instance.currentSceneIndex].sceneName;

		if (other.name == "PlayerShip") {
			if(!headlineText) {
				headlineText = GameObject.Find ("HeadlineTextBox").GetComponent<TypeOutScript> ();
				//displayBox = GameObject.Find ("DisplayBox").GetComponent<Image> ();
				GetComponent<Renderer>().enabled = false;
				headlineText.reset = true;
				headlineText.FinalText = "";
			}


			//displayBox.color = new Color (displayBox.color.r, displayBox.color.g, displayBox.color.b, 1);
			headlineText.reset = true;
			headlineText.TotalTypeTime = 2.5f;
			headlineText.FinalText = displayText;
			headlineText.On = true;
			StartCoroutine (ClearTextBox ());
		}
	}

	IEnumerator ClearTextBox() {
		yield return new WaitForSeconds(delayTime);
		headlineText.reset = true;
		//displayBox.color = new Color (displayBox.color.r, displayBox.color.g, displayBox.color.b, 0);

	}
}
