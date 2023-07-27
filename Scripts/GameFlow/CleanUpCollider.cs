using UnityEngine;
using System.Collections;

public class CleanUpCollider : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void OnCollisionEnter(Collision other) {
		
		//if (other.gameObject.GetComponent<Hostile>()) {
			other.gameObject.SetActive (false);
			Destroy (other.gameObject);
		//}

	}

	void OnTriggerEnter(Collider other) {

		//if (other.gameObject.GetComponent<Hostile>()) {
			other.gameObject.SetActive (false);
			Destroy (other.gameObject);
		//}

	}


}
