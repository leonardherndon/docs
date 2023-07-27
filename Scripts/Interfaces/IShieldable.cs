using UnityEngine;
using System.Collections;

public interface IShieldable<P,L> {
	
	void ActivateShield(P charge, L setPowerLevel);
	
	IEnumerator ChargeShield();

	IEnumerator EnterShield();

	IEnumerator RechargeShieldAbility();
	
	void ExitShield();

}
