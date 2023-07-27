using UnityEngine;
using System.Collections;

public interface IPhaseable {

	IEnumerator ActivatePhase();

	void ExitPhase();

	IEnumerator RechargePhase();
}
