using UnityEngine;
using System.Collections;
using CS_Audio;
using UnityEngine.UI;

public class UIAudioButton : MonoBehaviour
{

	[SerializeField] private AudioClip HighlightClip;
	[SerializeField] public AudioClip SelectedClip;
	void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
	    PlaySelectedClip();
        //Debug.Log("You have clicked the button: "+gameObject.name);
    }

    void PlayHighlightedClip() {
		//Debug.Log ("Highlighted Button: "+gameObject.name);
		if (!HighlightClip) return;
		AudioManager.Instance.uiAudioSource.clip = HighlightClip;
		AudioManager.Instance.uiAudioSource.Play();
	}
    
    void PlaySelectedClip()
    {
	    //Debug.Log ("Play Button Audio Clip");
	    if (!SelectedClip) return;
	    AudioManager.Instance.uiAudioSource.clip = SelectedClip;
	    AudioManager.Instance.uiAudioSource.Play();
    }
}
