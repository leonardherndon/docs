using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class WarningScreen : MonoBehaviour {

    public float warningTimer;
	// Use this for initialization
	void Start () {
        //AudioManager.Instance.PlayClip(1, 12);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Time.time >= warningTimer + 2.5f)
        {
            gameObject.SetActive(false);
        }
	}
    private void OnEnable()
    {
        warningTimer = Time.time;
        AudioManager.Instance.PlayClipWrap(1, 12);

    }
}
