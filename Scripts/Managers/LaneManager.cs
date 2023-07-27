  using UnityEngine;
using System.Collections;

public class LaneManager : MonoBehaviour {

	private static LaneManager _instance;
	public int[] currentAnchorLanes = new int[2];
	public int NUMBEROFLANES = 23;
	public Vector3[] laneArray;
	
	//Singleton pattern implementation
	public static LaneManager Instance {
		get 
		{
			if (_instance == null) 
			{
				_instance = Object.FindObjectOfType (typeof(LaneManager)) as LaneManager;

				if (_instance == null)
				{
					GameObject go = new GameObject("_lanemanager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<LaneManager>();
				}
			}
			return _instance;
		}
	}

}
