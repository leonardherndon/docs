using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLightTrigger : MonoBehaviour {
    
    public Color[] sceneColors = new Color[2];
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HueManager.Instance.SetSceneLights(sceneColors[0], sceneColors[1]);
            HueManager.Instance.RestoreAllLights(2);
        }
    }


}
