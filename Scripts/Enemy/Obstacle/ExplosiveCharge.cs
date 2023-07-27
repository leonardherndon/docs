using UnityEngine;
using System.Collections;
using Chronos;

public class ExplosiveCharge : Hostile {

	public float lifeTime;

	protected override void Start() {
		base.Start ();
		StartCoroutine(DelayExplosion());
	}
	
	// Update is called once per frame
	public override void FixedUpdate () {

		base.FixedUpdate();

		if (MoC.activationType == MovementActivationType.TimeAlive) {
			//Debug.Log ("time.time: " + time.time + " || MoC.ObjectTimeAlive: " + MoC.objectTimeAlive);
			if (BaseTimeLine.time >= MoC.TimedSwitchNumber) {
				if (gameObject.GetComponent<BoxCollider> ().enabled == false)
					gameObject.GetComponent<BoxCollider> ().enabled = true;
			}
		}
		
	}
	
	public IEnumerator DelayExplosion()
	{
		yield return new WaitForSeconds(lifeTime);
		//KillObject();
	}

	void OnCollisionEnter (Collision other) {

		if (other.gameObject.name == gameObject.name) {
			Physics.IgnoreCollision (gameObject.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
			return;
		}

		//base.OnCollisionEnter(other);
	}
}
