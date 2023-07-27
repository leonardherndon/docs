using UnityEngine;
using System.Collections;

public interface IBoostable {

    void ChargeBoost();

	IEnumerator EnterBoost();

	IEnumerator RechargeBoost();

	void ExitBoost();
}
