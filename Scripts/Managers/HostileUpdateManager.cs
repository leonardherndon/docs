using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileUpdateManager : MonoBehaviour {

//	public static HostileUpdateManager Instance { get; private set; }
//
//	public List<Hostile> _managedHostiles = new List<Hostile>();
//
//	void Awake()
//	{
//		Instance = this;
//	}
//
//	void Update()
//	{
//		for (int i = 0; i < _managedHostiles.Count; ++i)
//		{
//			_managedHostiles[i].ManagedUpdate();
//		}
//	}
//
//	void FixedUpdate()
//	{
//		for (int i = 0; i < _managedHostiles.Count; ++i)
//		{
//			_managedHostiles[i].ManagedFixedUpdate();
//		}
//	}
//
//	void LateUpdate()
//	{
//		for (int i = 0; i < _managedHostiles.Count; ++i)
//		{
//			_managedHostiles[i].ManagedLateUpdate();
//		}
//	}
//
//	public void Register(Hostile hostile)
//	{
//		_managedHostiles.Add(hostile);
//	}
//
//	public void Unregister(Hostile hostile)
//	{
//		_managedHostiles.Remove(hostile);
//	}
//
}
