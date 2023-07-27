using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HueTrigger : MonoBehaviour {

    public int lightID;

	public void OnTriggerEnter(Collider other)
    {
	    CollisionController oCoC = other.GetComponent<CollisionController>();
	    
	    if(oCoC == null) return;
	    if(!oCoC._hueActive || oCoC.CSM.CurrentColor == GameColor.Grey) return;
           // Debug.Log("HUE: trigger hit.: 2 | " + 3 + " | -1 | " + lightID);
           HueManager.Instance.Flash(lightID, 2, oCoC.CSM.CurrentColor, -1);
	}

    public void OnTriggerExit(Collider other)
    {
	    CollisionController oCoC = other.GetComponent<CollisionController>();
	    
	    if(oCoC == null) return;
	    if(!oCoC._hueActive || oCoC.CSM.CurrentColor == GameColor.Grey) return;
			// Debug.Log("HUE: trigger hit.: 2 | " + 3 + " | -1 | " + lightID);
			HueManager.Instance.RestoreLight(lightID, 2);
    }
}
