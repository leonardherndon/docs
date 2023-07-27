using System;
using ChromaShift.Scripts;
using UnityEngine;
using Chronos;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class BaseBehaviour : SerializedMonoBehaviour {

	[OdinSerialize] public ICollidible CoC;
	[OdinSerialize] public ILifeSystem LS;
	public ChromaShiftManager CSM;
	public Timeline BaseTimeLine;
	protected virtual void CommonInitialization()
	{
		GameObject myGameObject = gameObject;
		string gameObjectName = myGameObject.name;

		if (CoC == null)
		{
			Debug.LogError("No Collision Controller Attached to " + gameObjectName + ". Please attach to enable functionality");
			return;
		}
		if (LS == null)
		{
			Debug.LogError("No Life System Controller Attached to " + gameObjectName + ". Please attach to enable functionality");
			return;
		}
		
		if (CSM == null)
		{
			Debug.LogError("No ChromaShift Manager Attached to " + gameObjectName + ". Please attach to enable functionality");
			return;
		}
		
		if (BaseTimeLine == null)
		{
			Debug.LogError("No TimeLine Attached to " + GetComponent<GameObject>().name + ". Please attach to enable functionality");
			return;
		}
	}

	
}
