using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINextButton : MonoBehaviour {

    Button btn;
    public Button nextBtn;

	// Use this for initialization
	void Start () {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
	
	// Update is called once per frame
	void TaskOnClick()
    {
        nextBtn.Select();
        nextBtn.OnSelect(null);
    }
}
